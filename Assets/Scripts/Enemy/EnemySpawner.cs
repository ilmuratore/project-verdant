using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnerState
{
    Inattivo,
    InCorso,
    Completato
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Tooltip("Offset casuale piccolo intorno allo spawn point. Utile quando i nemici sono più degli spawn point.")]
    public float spawnRandomOffset = 0.35f;

    [Header("Timing")]
    [Tooltip("Pausa prima di far partire l'ondata successiva.")]
    public float pausaTraOndate = 1.5f;

    [Header("State")]
    [SerializeField] private SpawnerState stato = SpawnerState.Inattivo;

    private QuestData quest;
    private int indiceOndataCorrente = 0;
    private Coroutine routineOndate;
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    public void AvviaQuest(QuestData questData)
    {
        if (stato == SpawnerState.InCorso) return;

        if (questData == null)
        {
            Debug.LogWarning("QuestData non assegnata allo spawner.");
            return;
        }

        quest = questData;
        indiceOndataCorrente = 0;
        stato = SpawnerState.InCorso;
        spawnedEnemies.Clear();

        if (routineOndate != null)
        {
            StopCoroutine(routineOndate);
        }

        routineOndate = StartCoroutine(GestisciOndate());
    }

    public void FermaSpawner(bool distruggiNemiciSpawnati)
    {
        if (routineOndate != null)
        {
            StopCoroutine(routineOndate);
            routineOndate = null;
        }

        if (distruggiNemiciSpawnati)
        {
            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
        }

        spawnedEnemies.Clear();
        stato = SpawnerState.Inattivo;
    }

    private IEnumerator GestisciOndate()
    {
        while (stato == SpawnerState.InCorso && quest != null && indiceOndataCorrente < quest.ondate.Count)
        {
            Ondata ondata = quest.ondate[indiceOndataCorrente];
            SpawnaOndata(ondata);

            yield return new WaitUntil(() => stato != SpawnerState.InCorso || OndataAzzerata());

            if (stato != SpawnerState.InCorso)
            {
                routineOndate = null;
                yield break;
            }

            indiceOndataCorrente++;

            if (indiceOndataCorrente < quest.ondate.Count)
            {
                yield return new WaitForSeconds(pausaTraOndate);
            }
        }

        stato = SpawnerState.Completato;
        routineOndate = null;
    }

    private void SpawnaOndata(Ondata ondata)
    {
        if (ondata == null)
        {
            Debug.LogWarning("Ondata non valida.");
            return;
        }

        for (int i = 0; i < ondata.numeroNemici; i++)
        {
            SpawnSingolo();
        }
    }

    private void SpawnSingolo()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemyPrefab non presente.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Nessun spawn point valido.");
            return;
        }

        List<Transform> validSpawnPoints = new List<Transform>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                validSpawnPoints.Add(spawnPoint);
            }
        }

        if (validSpawnPoints.Count == 0)
        {
            Debug.LogWarning("Nessun spawn point valido.");
            return;
        }

        Transform selectedSpawn = validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
        Vector2 randomOffset = Random.insideUnitCircle * spawnRandomOffset;
        Vector3 spawnPosition = selectedSpawn.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(enemy);

        Enemy_AI ai = enemy.GetComponent<Enemy_AI>();
        if (ai == null)
        {
            Debug.LogWarning("Enemy_AI non presente sul prefab nemico.");
        }
    }

    private bool OndataAzzerata()
    {
        CleanEnemyList();
        return spawnedEnemies.Count == 0;
    }

    private void CleanEnemyList()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = spawnedEnemies[i];

            if (enemy == null || !enemy.activeInHierarchy)
            {
                spawnedEnemies.RemoveAt(i);
            }
        }
    }
}
