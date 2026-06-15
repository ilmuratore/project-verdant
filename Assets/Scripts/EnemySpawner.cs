using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{


    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform player;

    [Header("Rules")]
    public int maxEnemiesAlive = 3;
    public float spawnDelay = 3f;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }


    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            CleanEnemyList();

            if (aliveEnemies.Count < maxEnemiesAlive )
            {
                SpawnEnemy();
            }
                
        }
    }

    private void SpawnEnemy()
    {
        if(enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab non assegnato");
            return;
        }
        if(spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Nessun spawn point valido");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedSpawn = spawnPoints[randomIndex];

        GameObject enemy = Instantiate(
            enemyPrefab,
            selectedSpawn.position,
            Quaternion.identity

            );

        Enemy_AI ai = enemy.GetComponent<Enemy_AI>();
        if( ai != null)
        {
            ai.player = player;
        }
        if(ai == null)
        {
            Debug.LogWarning("Player non presente");
        }

        aliveEnemies.Add(enemy);

    }

    private void CleanEnemyList()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
    }
}
