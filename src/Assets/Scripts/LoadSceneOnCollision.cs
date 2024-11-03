using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneOnCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene("LevelOne", LoadSceneMode.Single);
    }
}
