using UnityEngine;

public class PhysicsTrigger : MonoBehaviour
{
    /* VARIABLES */
    public GameObject CollidingObject;
    public string ColliderTag = "None";
    public bool IsColliding = false;


    /* FUNCTIONS */
    private void OnTriggerEnter(Collider collider)
    {
        IsColliding = true;
        CollidingObject = collider.gameObject;
    }

    private void OnTriggerExit(Collider collider)
    {
        IsColliding = false;
        CollidingObject = null;
    }
}
