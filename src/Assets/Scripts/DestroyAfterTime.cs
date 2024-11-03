using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    /* VARIABLES */
    public float TimeBeforeDestroy = 0.25f;
    private bool IsAlive = true;


    /* FUNCTIONS */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsAlive == true)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * TimeBeforeDestroy);

            if (transform.localScale.x >= 0.95f)
            {
                IsAlive = false;
            }
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * TimeBeforeDestroy);

            if (transform.localScale.x <= 0.05f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
