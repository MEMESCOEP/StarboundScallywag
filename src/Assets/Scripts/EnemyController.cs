using System.Collections;
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

    public AttackTypes AttackType = AttackTypes.MeleeBounce;
    public float DamageAmount = 5f;
    public float StopDistance = 3f;
    public float MoveSpeed = 100f;
    public float JumpForce = 16f;
    public float Health = 100f;
    private GameObject Player;
    private Rigidbody Rb;
    private bool DealtDamage = false;
    private bool IsAttacking = false;
    private bool IsShooting = false;


    /* FUNCTIONS */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 5f);

        if (Health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(Rb.position, Player.transform.position) > StopDistance && AttackType != AttackTypes.RangedStationary)
        {
            Rb.MovePosition(Vector3.MoveTowards(Rb.position, Player.transform.position, MoveSpeed * Time.fixedDeltaTime));
        }
        else if (IsAttacking == false)
        {
            // It's attack time babyyyyy >:D
            IsAttacking = true;
            Invoke(nameof(ResetAttack), 1f);

            switch (AttackType)
            {
                case AttackTypes.MeleeBounce:
                    //Rb.AddForce(Vector2.up * 550f);
                    JumpTowardsPlayer();
                    break;
            }
        }
        else if (Vector3.Distance(Rb.position, Player.transform.position) > 1f && AttackType != AttackTypes.RangedStationary && DealtDamage == false)
        {
            DealtDamage = true;
            Player.GetComponent<PlayerMovement>().Health -= 10;
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
        while (true)
        {
            yield return new WaitForSeconds(2.5f);
            GameObject Projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Projectile.AddComponent<Rigidbody>();
            Projectile.GetComponent<Rigidbody>().useGravity = false;
            Projectile.transform.position = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
            Debug.DrawLine(Projectile.transform.position, Player.transform.position, Color.red, 5f);

            // Calculate direction to target
            Vector3 direction = (Player.transform.position - Projectile.transform.position).normalized;

            // Apply force to the Rigidbody
            Projectile.GetComponent<Rigidbody>().AddForce(direction * 50f, ForceMode.Impulse);
        }

        IsShooting = false;
    }

    public void HurtEnemy(float DamageAmount)
    {
        transform.localScale = Vector3.one * 1.5f;
        Health -= DamageAmount;
    }
}
