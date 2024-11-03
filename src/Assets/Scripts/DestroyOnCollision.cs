using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    /* VARIABLES */
    public float TimeToWaitBeforeDestroyEnabled = 0.5f;
    private bool DestroyEnabled = false;


    /* FUNCTIONS */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(SetDestroyEnabled), TimeToWaitBeforeDestroyEnabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetDestroyEnabled()
    {
        DestroyEnabled = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (DestroyEnabled == true)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (DestroyEnabled == true)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (DestroyEnabled == true)
        {
            Destroy(this.gameObject);
        }
    }
}
