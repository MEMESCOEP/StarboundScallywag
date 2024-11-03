using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /* VARIABLES */
    public enum AttackTypes {
        MeleeBounce,
        MeleeHit,
        RangedStationary,
        Ranged,
    }

    public Slider HealthBar;
    public AttackTypes AttackType = AttackTypes.MeleeBounce;
    public GameObject Player;
    public Animator AnimationController;
    public float AttackInterval = 1f;
    public float DetectionRange = 25f;
    public float DamageAmount = 5f;
    public float StopDistance = 3f;
    public float MoveSpeed = 100f;
    public float JumpForce = 16f;
    public float Health = 100f;
    private Rigidbody Rb;
    private Vector3 DefaultScale;
    private bool DealtDamage = false;
    private bool IsAttacking = false;
    private bool IsShooting = false;


    /* FUNCTIONS */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rb = GetComponent<Rigidbody>();
        DefaultScale = transform.localScale;
        HealthBar.maxValue = Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0 || transform.position.y <= -1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 15f);

            if (transform.localScale.x <= (Vector3.one * 0.1f).x)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, DefaultScale, Time.deltaTime * 5f);
        }

        HealthBar.value = Health;
    }

    void FixedUpdate()
    {
        if (Health <= 0)
        {
            return;
        }

        float PlayerDistance = Vector3.Distance(Rb.position, Player.transform.position);

        if (PlayerDistance > DetectionRange && AttackType != AttackTypes.RangedStationary)
        {
            if (AttackType == AttackTypes.MeleeHit && (AnimationController.GetBool("IsWalking") == true || AnimationController.GetBool("Attacking") == true))
            {
                AnimationController.SetBool("Attacking", false);
                AnimationController.SetBool("IsWalking", false);
            }
        }
        else if (PlayerDistance > StopDistance && AttackType != AttackTypes.RangedStationary)
        {
            if (AttackType == AttackTypes.MeleeHit && AnimationController.GetBool("IsWalking") == false)
            {
                AnimationController.SetBool("Attacking", false);
                AnimationController.SetBool("IsWalking", true);
                AnimationController.Play("Walking");
            }

            Rb.MovePosition(Vector3.MoveTowards(Rb.position, Player.transform.position, MoveSpeed * Time.fixedDeltaTime));

            // Get the direction from the current position to the target position
            Vector3 direction = Player.transform.position - transform.position;

            // Ignore the vertical component by setting the y component to zero
            direction.y = 0;

            // If the direction is not zero, calculate the rotation
            if (direction != Vector3.zero)
            {
                // Create a rotation that looks in the direction of the modified vector
                Quaternion rotation = Quaternion.LookRotation(direction);

                // Apply the rotation to the current object's transform
                transform.rotation = rotation;
            }
        }
        else if (IsAttacking == false)
        {
            // It's attack time babyyyyy >:D
            IsAttacking = true;
            Invoke(nameof(ResetAttack), AttackInterval);

            switch (AttackType)
            {
                case AttackTypes.MeleeBounce:
                    JumpTowardsPlayer();
                    break;
            }
        }
        else if (PlayerDistance > 1f && AttackType != AttackTypes.RangedStationary && DealtDamage == false)
        {
            DealtDamage = true;

            if (AttackType == AttackTypes.MeleeHit)
            {
                AnimationController.SetBool("Attacking", true);
                AnimationController.SetBool("IsWalking", false);
                AnimationController.Play("Attacking");
            }

            Player.GetComponent<PlayerMovement>().Health -= DamageAmount;
        }
        else if (AttackType == AttackTypes.RangedStationary && IsShooting == false)
        {
            IsShooting = true;
            StartCoroutine("ShootProjectile");
            //Invoke(nameof(ResetShooting), 3f);
        }
    }

    private void JumpTowardsPlayer()
    {
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Vector3 jumpVector = new Vector3(direction.x, 1, direction.z).normalized * JumpForce;
        Rb.AddForce(jumpVector, ForceMode.Impulse);
        Player.GetComponent<PlayerMovement>().Health -= DamageAmount;
    }

    private void ResetAttack()
    {
        DealtDamage = false;
        IsAttacking = false;
    }

    private void ResetShooting()
    {
        IsShooting = false;
    }

    private IEnumerator ShootProjectile()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            yield return new WaitForSeconds(2.5f);
            GameObject Projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Projectile.AddComponent<Rigidbody>();
            Projectile.GetComponent<Rigidbody>().useGravity = false;
            Projectile.transform.position = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);

            // Calculate direction to target
            Vector3 direction = (Player.transform.position - Projectile.transform.position).normalized;

            // Apply force to the Rigidbody
            Projectile.GetComponent<Rigidbody>().AddForce(direction * 50f, ForceMode.Impulse);
        }

        IsShooting = false;
    }

    public void HurtEnemy(float DamageAmount)
    {
        transform.localScale = DefaultScale * 1.5f;
        Health -= DamageAmount;
    }
}
