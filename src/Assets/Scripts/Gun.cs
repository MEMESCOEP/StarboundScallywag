using UnityEngine.UI;
using UnityEngine;

public class Gun : MonoBehaviour
{
    /* VARIABLES */
    public GameObject BulletPrefab;
    public GameObject Crosshair;
    public Transform BarrelEnd;
    public Camera PlayerCamera;
    public float GunDamage = 10f;
    public float GunRange = 500f;
    private RectTransform CrosshairRectTransform;
    private RaycastHit RayHit;
    private RawImage CrosshairImageComponent;
    private Vector3 RayOrigin = new Vector3(0.5f, 0.5f, 0f); // Center of the screen
    private Ray HitRay;
    private bool AimingAtEnemy = false;


    /* FUNCTIONS */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CrosshairImageComponent = Crosshair.GetComponent<RawImage>();
        CrosshairRectTransform = Crosshair.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        HitRay = PlayerCamera.ViewportPointToRay(RayOrigin);
        CrosshairRectTransform.localScale = Vector3.Lerp(CrosshairRectTransform.localScale, Vector3.one * 0.25f, 5f * Time.deltaTime);

        if (Physics.Raycast(HitRay, out RayHit, GunRange))
        {
            if (RayHit.transform.CompareTag("Enemy"))
            {
                CrosshairImageComponent.color = Color.red;
                AimingAtEnemy = true;
            }
            else
            {
                CrosshairImageComponent.color = Color.green;
                AimingAtEnemy = false;
            }
        }
        else
        {
            CrosshairImageComponent.color = Color.green;
            AimingAtEnemy = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Instantiate the bullet at the fire point's position and rotation
            GameObject bullet = Instantiate(BulletPrefab, BarrelEnd.position, BarrelEnd.rotation);
            Vector3 endPoint = BarrelEnd.position + BarrelEnd.forward * GunRange;
            bullet.GetComponent<LineRenderer>().SetPositions(new Vector3[] {BarrelEnd.position, endPoint});

            if (AimingAtEnemy == true && RayHit.transform.gameObject != null)
            {
                RayHit.transform.gameObject.GetComponent<EnemyController>().HurtEnemy(GunDamage);
                CrosshairRectTransform.localScale = Vector3.one * 0.55f;
            }
            else
            {
                CrosshairRectTransform.localScale = Vector3.one * 0.35f;
            }
        }
    }
}
