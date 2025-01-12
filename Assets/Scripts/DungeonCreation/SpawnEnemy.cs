using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;



    public void SpawnRandomEnemy()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject spawnPoint in spawnPoints)
        {
            GameObject enemyPrefab = GetRandomEnemyPrefab();
            if (enemyPrefab != null)
            {
                Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            }
        }


    }

    private GameObject GetRandomEnemyPrefab()
    {
        if (enemyPrefabs.Count > 0)
        {
            return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        }
        Debug.LogWarning("No enemy prefabs available to spawn!");
        return null;
    }
}
