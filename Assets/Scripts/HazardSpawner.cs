using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public GameObject[] hazardPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 10f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomHazard();
        }
    }

    void SpawnRandomHazard()
    {
        if (hazardPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        GameObject prefab = hazardPrefabs[Random.Range(0, hazardPrefabs.Length)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, point.position, point.rotation);
    }
}