using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    /* VARIABLES */
    public TextMeshProUGUI EnemyCountText;
    public GameObject EnemySpawnerObject;
    public int EnemiesToUnlock = 0;
    public int RoomNumber = 0;
    private EnemySpawner ESpawnComponent;


    /* FUNCTIONS */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ESpawnComponent = EnemySpawnerObject.GetComponent<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ESpawnComponent.UnlockedRooms >= RoomNumber)
        {
            Destroy(this.gameObject);
        }
        else if (ESpawnComponent.UnlockedRooms == RoomNumber - 1)
        {
            EnemyCountText.text = $"{ESpawnComponent.SpawnedEnemies.Count} enemies left before unlock";
        }
        else
        {
            EnemyCountText.text = "Locked";
        }
    }
}
