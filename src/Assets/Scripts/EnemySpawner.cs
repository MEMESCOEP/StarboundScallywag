using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /* VARIABLES */
    public List<Transform> Room1SpawnPoints = new List<Transform>();
    public List<Transform> Room2SpawnPoints = new List<Transform>();
    public List<Transform> Room3SpawnPoints = new List<Transform>();
    public List<Transform> Room4SpawnPoints = new List<Transform>();
    public List<Transform> Room5SpawnPoints = new List<Transform>();
    public List<GameObject> EnemyPrefabs = new List<GameObject>();
    public GameObject PlayerObject;
    public int SpawnCountIncrease = 5;
    public int UnlockedRooms = 0;
    public int SpawnCount = 10;
    public List<GameObject> SpawnedEnemies = new List<GameObject>();
    private List<Transform> AllSpawnPoints = new List<Transform>();
    private System.Random RNG = new System.Random();
    private bool FirstTime = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnedEnemies.Count <= 0)
        {
            UnlockNewRoom(FirstTime);
            SpawnEnemies();

            FirstTime = false;
        }

        // Remove empty enemies from the list
        for (int EnemyIndex = SpawnedEnemies.Count; EnemyIndex > 0; EnemyIndex--)
        {
            if (SpawnedEnemies[EnemyIndex - 1] == null)
            {
                print($"Removing enemy at index {EnemyIndex - 1}");
                SpawnedEnemies.RemoveAt(EnemyIndex - 1);
                print($"FirstTime is {FirstTime}.");
            }
        }
    }

    public void UnlockNewRoom(bool FirstTime)
    {
        SpawnedEnemies.Clear();
        AllSpawnPoints = Room1SpawnPoints;

        if (FirstTime == false && UnlockedRooms <= 3)
        {
            UnlockedRooms++;
            SpawnCount += SpawnCountIncrease;
        }

        switch (UnlockedRooms)
        {
            case  1:
                AllSpawnPoints.AddRange(Room2SpawnPoints);
                break;

            case  2:
                AllSpawnPoints.AddRange(Room2SpawnPoints);
                AllSpawnPoints.AddRange(Room3SpawnPoints);
                break;

            case  3:
                AllSpawnPoints.AddRange(Room2SpawnPoints);
                AllSpawnPoints.AddRange(Room3SpawnPoints);
                AllSpawnPoints.AddRange(Room4SpawnPoints);
                break;

            case  4:
                AllSpawnPoints.AddRange(Room2SpawnPoints);
                AllSpawnPoints.AddRange(Room3SpawnPoints);
                AllSpawnPoints.AddRange(Room4SpawnPoints);
                AllSpawnPoints.AddRange(Room5SpawnPoints);
                break;

            default:
                break;
        }
    }

    public void SpawnEnemies()
    {
        for (int SpawnIndex = 0; SpawnIndex < SpawnCount; SpawnIndex++)
        {
            int RandomEnemyIndex = RNG.Next(0, EnemyPrefabs.Count);
            int RandomSpawnPointIndex = RNG.Next(0, AllSpawnPoints.Count);

            GameObject NewEnemy = Instantiate(EnemyPrefabs[RandomEnemyIndex], AllSpawnPoints[RandomSpawnPointIndex].position, Quaternion.identity);
            NewEnemy.GetComponent<EnemyController>().Player = PlayerObject;
            SpawnedEnemies.Add(NewEnemy);
        }
    }
}
