// Some stupid rigidbody based movement by Dani

using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour {

    //Assingables
    public TextMeshProUGUI HealthDisplay;
    public Transform playerCam;
    public Transform orientation;
    public Collider[] WallrunningTriggers;

    //Other
    public float Health = 100f;
    private Rigidbody rb;
    private List<PhysicsTrigger> Triggers = new List<PhysicsTrigger>();

    //Rotation and look
    public float LerpSpeed = 5f;
    public float sensMultiplier = 1f;
    public float sensitivity = 50f;
    public float CameraTiltAngle = 5f;
    private Vector3 CameraEulerAngles;
    private float xRotation;
    private float LerpRot = 0f;
    private bool WaitForZeroTilt = false;


    //Movement
    public float SpeedMultiplier = 5f;
    public float moveSpeed = 4000;
    public float maxSpeed = 15;
    public bool grounded;
    public LayerMask whatIsGround;
    private int WallrunDir = 0;
    private bool IsWallrunning = false;
    private bool JumpedOffWall = false;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    public float PrevDir = 0f;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    float x, y;
    bool jumping, sprinting, crouching;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Start() {
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GetComponent<MeshRenderer>().enabled = false;

        foreach (var Trigger in WallrunningTriggers)
        {
            Triggers.Add(Trigger.GetComponent<PhysicsTrigger>());
        }
    }


    private void FixedUpdate() {
        Movement();
    }

    private void Update() {
        MyInput();
        Look();

        HealthDisplay.text = $"Health: {Health}";
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        sprinting = Input.GetKey(KeyCode.LeftShift);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch() {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.linearVelocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch() {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement() {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement while in air and not wallrunning
        if (!grounded && IsWallrunning == false) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sprinting
        if (grounded && sprinting && y > 0) {
            multiplier = SpeedMultiplier;
            multiplierV = SpeedMultiplier;
        }

        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);

        // Set y velocity to zero if the player is wall running
        if (IsWallrunning == true && jumping == false && JumpedOffWall == false)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }

    private void Jump() {
        if ((grounded || IsWallrunning) && readyToJump) {
            readyToJump = false;

            //Add jump forces
            if (IsWallrunning == true)
            {
                JumpedOffWall = true;
                Invoke(nameof(ResetWallrunning), 0.25f);
                rb.AddForce(orientation.right * jumpForce * 1.75f * WallrunDir);
                rb.AddForce(Vector2.up * jumpForce * 1.75f);
                rb.AddForce(normalVector * jumpForce * 0.5f);
            }
            else
            {
                rb.AddForce(Vector2.up * jumpForce * 1.5f);
                rb.AddForce(normalVector * jumpForce * 0.5f);
            }

            //If jumping while falling and not wallrunning, reset y velocity.
            Vector3 vel = rb.linearVelocity;
            if (rb.linearVelocity.y < 0.5f && IsWallrunning == false)
                rb.linearVelocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.linearVelocity.y > 0)
                rb.linearVelocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump() {
        readyToJump = true;
    }

    private void ResetWallrunning() {
        IsWallrunning = false;
        JumpedOffWall = false;
    }

    private float desiredX;
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        foreach (var Trigger in Triggers)
        {
            if (grounded == true)
            {
                IsWallrunning = false;
                continue;
            }

            if (Trigger.ColliderTag == "Right" && Trigger.IsColliding == true && Trigger.CollidingObject.transform.tag == "Wall")
            {
                IsWallrunning = true;
                PrevDir = -1;
                WallrunDir = -1;
                rb.useGravity = false;
                break;
            }
            else if (Trigger.ColliderTag == "Left" && Trigger.IsColliding == true && Trigger.CollidingObject.transform.tag == "Wall")
            {
                IsWallrunning = true;
                PrevDir = 1;
                WallrunDir = 1;
                rb.useGravity = false;
                break;
            }
            else
            {
                rb.useGravity = true;
                IsWallrunning = false;
            }
        }

        if (IsWallrunning == false)
        {
            if (x == 0 || WaitForZeroTilt == true)
            {
                LerpRot = Mathf.Lerp(LerpRot, 0, Time.deltaTime * LerpSpeed);

                if (LerpRot < 0.5f)
                {
                    WaitForZeroTilt = false;
                }
            }
            else
            {
                if (PrevDir != x)
                {
                    //WaitForZeroTilt = true;
                }

                LerpRot = Mathf.Lerp(LerpRot, CameraTiltAngle, Time.deltaTime * LerpSpeed);
                PrevDir = x;
            }
        }
        else
        {
            LerpRot = Mathf.Lerp(LerpRot, CameraTiltAngle + 5, Time.deltaTime * (LerpSpeed + 5));
        }

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, LerpRot * -PrevDir);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.linearVelocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.linearVelocity.x, 2) + Mathf.Pow(rb.linearVelocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.linearVelocity.y;
            Vector3 n = rb.linearVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.linearVelocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }

}
