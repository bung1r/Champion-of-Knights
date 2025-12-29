using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnPlatformManager : MonoBehaviour
{
    public static SpawnPlatformManager Instance { get; private set; }
    [SerializeField] private List<SpawnPlatform> spawnPlatforms = new List<SpawnPlatform>();
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;

        foreach (Transform child in transform)
        {
            SpawnPlatform platform = child.GetComponent<SpawnPlatform>();
            if (platform != null)
            {
                spawnPlatforms.Add(platform);
            }
        }
    }

    public List<SpawnPlatform> GetClosestPlatformsFromPos(UnityEngine.Vector3 position, int numPlatforms)
    {
        List<PlatformDistance> platformDistances = new List<PlatformDistance>();
        foreach (SpawnPlatform platform in spawnPlatforms)
        {
            // calculate distance to position
            float distance = UnityEngine.Vector3.Distance(position, platform.transform.position);
            platformDistances.Add(new PlatformDistance(platform, distance));
        }
        platformDistances.Sort((a, b) => a.distance.CompareTo(b.distance));
        List<SpawnPlatform> closestPlatforms = new List<SpawnPlatform>();
        for (int i = 0; i < numPlatforms && i < platformDistances.Count; i++)
        {
            closestPlatforms.Add(platformDistances[i].platform);
        }
        return closestPlatforms;
    }

    public void SpawnEnemies(List<GameObject> enemyPrefabs, UnityEngine.Vector3 position)
    {
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            // get the 3 closest platforms to position
            List<SpawnPlatform> closestPlatforms = GetClosestPlatformsFromPos(position, 3);
            
            // choose random platform from closest platforms
            SpawnPlatform chosenPlatform = closestPlatforms[Random.Range(0, closestPlatforms.Count)];
            chosenPlatform.SpawnEnemy(enemyPrefab);
        }
    }

}

public class PlatformDistance
{
    public SpawnPlatform platform;
    public float distance;

    public PlatformDistance(SpawnPlatform platform, float distance)
    {
        this.platform = platform;
        this.distance = distance;
    }
}