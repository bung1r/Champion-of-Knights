using UnityEngine;

public class SpawnPlatform : MonoBehaviour
{
    public int maxSpawn = 3;
    void Awake()
    {

    }
    public void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-transform.localScale.x/2, transform.localScale.x/2), transform.localScale.y + 0.5f, Random.Range(-transform.localScale.z/2, transform.localScale.z/2));
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        RoundManager.Instance.AddEnemy(enemy);
    }
}