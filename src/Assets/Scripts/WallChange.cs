using UnityEngine;

public class WallChange : MonoBehaviour
{
    /* VARIABLES */
    public Material BreakOutMaterialAccent;
    public Material BreakOutMaterialWall;
    public Material DefaultMaterialAccent;
    public Material DefaultMaterialWall;
    public GameObject FinalBoss;


    /* FUNCTIONS */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject Wall in GameObject.FindGameObjectsWithTag("Wall"))
        {
            var MeshRendererComponent = Wall.GetComponent<MeshRenderer>();

            if (MeshRendererComponent == null)
            {
                continue;
            }

            MeshRendererComponent.material = DefaultMaterialWall;
        }

        foreach (GameObject Accent in GameObject.FindGameObjectsWithTag("Accents"))
        {
            var MeshRendererComponent = Accent.GetComponent<MeshRenderer>();

            if (MeshRendererComponent == null)
            {
                continue;
            }

            MeshRendererComponent.material = DefaultMaterialAccent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (FinalBoss == null)
        {
            foreach (GameObject Wall in GameObject.FindGameObjectsWithTag("Wall"))
            {
                var MeshRendererComponent = Wall.GetComponent<MeshRenderer>();

                if (MeshRendererComponent == null)
                {
                    continue;
                }

                MeshRendererComponent.material = BreakOutMaterialWall;
            }

            foreach (GameObject Accent in GameObject.FindGameObjectsWithTag("Accents"))
            {
                var MeshRendererComponent = Accent.GetComponent<MeshRenderer>();

                if (MeshRendererComponent == null)
                {
                    continue;
                }

                MeshRendererComponent.material = BreakOutMaterialAccent;
            }

            Destroy(this.gameObject);
        }
    }
}
