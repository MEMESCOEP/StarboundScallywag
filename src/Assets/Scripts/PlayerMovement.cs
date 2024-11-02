using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* VARIABLES */
    public enum MouseButtons {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    [Header("Assignables")]
    public Rigidbody PlayerRB;
    public Camera PlayerCamera;

    [Header("Health")]
    public float PlayerHealthOvercharge = 50f; // For powerups if we ever get a chance to add them (Allows for HP to go over max health by x)
    public float PlayerDefaultMaxHealth = 100f; // Max health without powerups
    public float PlayerHealth = 100f;

    [Header("Movement")]
    public float SpeedMultiplier = 1f;
    public float MovementSpeed = 100f;

    [Header("Controls")]
    // Mouse controls
    public Vector2 MouseSensitivity = new Vector2(1000, 500);
    public Vector2 MaxLookAngle = new Vector2(-90, 90);
    public MouseButtons MouseFireButton = MouseButtons.Left;
    public MouseButtons MouseZoomButton = MouseButtons.Right;

    // Keyboard controls
    public KeyCode ForwardKey = KeyCode.W;
    public KeyCode BackwardKey = KeyCode.S;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode RightKey = KeyCode.D;
    public KeyCode CrouchKey = KeyCode.LeftControl;
    public KeyCode SprintKey = KeyCode.LeftShift;

    // Private variables
    Transform Orientation;
    float CameraRotationX = 0f;
    float CameraRotationY = 0f;


    /* FUNCTIONS */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("[== PLAYER ==]");
        print("[INFO] >> Locking mouse pointer to game window...");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check input and move / rotate accordingly
        float MouseInputX = Input.GetAxisRaw("Mouse X") * MouseSensitivity.x * Time.deltaTime;
        float MouseInputY = Input.GetAxisRaw("Mouse Y") * MouseSensitivity.x * Time.deltaTime;

        if (Input.GetKey(ForwardKey))
        {
            PlayerRB.AddForce(PlayerCamera.transform.forward * (MovementSpeed * SpeedMultiplier) * Time.deltaTime);
        }

        // Rotate camera according to mouse input (also clamp the camera's rotation on the Y axis so that the player can't bend their head inside their body. THey aren't a demon. I think-)
        CameraRotationY += MouseInputX;
        CameraRotationX -= MouseInputY;
        CameraRotationX = Mathf.Clamp(CameraRotationX, MaxLookAngle.x, MaxLookAngle.y);
        PlayerCamera.transform.rotation = Quaternion.Euler(CameraRotationX, CameraRotationY, 0f);
        transform.rotation = Quaternion.Euler(0f, CameraRotationY, 0f);
    }
}
