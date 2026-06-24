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
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnRandomOffset = 0.35f;
    public float pausaTraOndate = 1.5f;

    [SerializeField] private SpawnerState stato = SpawnerState.Inattivo;
    [SerializeField] private Transform spawnedEnemiesRoot;

    private QuestData quest;
    private int indiceOndataCorrente;
    private Coroutine routineOndate;
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        ResolveSpawnPoints();
    }

    public void AvviaQuest(QuestData questData)
    {
        if (stato == SpawnerState.InCorso) return;
        if (questData == null || enemyPrefab == null) return;

        ResolveSpawnPoints();

        quest = questData;
        indiceOndataCorrente = 0;
        stato = SpawnerState.InCorso;
        spawnedEnemies.Clear();

        if (routineOndate != null) StopCoroutine(routineOndate);
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
                if (enemy != null) Destroy(enemy);
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
            SpawnWave(ondata);

            yield return new WaitUntil(() => stato != SpawnerState.InCorso || WaveCleared());

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

    private void SpawnWave(Ondata ondata)
    {
        if (ondata == null) return;

        for (int i = 0; i < ondata.numeroNemici; i++)
        {
            SpawnSingle();
        }
    }

    private void SpawnSingle()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        Transform selectedSpawn = GetRandomSpawnPoint();
        if (selectedSpawn == null) return;

        Vector2 randomOffset = Random.insideUnitCircle * spawnRandomOffset;
        Vector3 spawnPosition = selectedSpawn.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, GetSpawnedEnemiesRoot());
        spawnedEnemies.Add(enemy);
    }

    private Transform GetRandomSpawnPoint()
    {
        List<Transform> valid = new List<Transform>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null) valid.Add(spawnPoint);
        }

        if (valid.Count == 0) return null;
        return valid[Random.Range(0, valid.Count)];
    }

    private bool WaveCleared()
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
                continue;
            }

            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats != null && stats.IsDead)
            {
                spawnedEnemies.RemoveAt(i);
            }
        }
    }

    private void ResolveSpawnPoints()
    {
        if (spawnPoints != null && spawnPoints.Length > 0) return;

        Transform spawnRoot = transform.Find("EnemySpawnPoints");
        if (spawnRoot == null) spawnRoot = transform.Find("EnemySpawns");
        if (spawnRoot == null) return;

        List<Transform> points = new List<Transform>();
        foreach (Transform child in spawnRoot)
        {
            points.Add(child);
        }

        spawnPoints = points.ToArray();
    }

    private Transform GetSpawnedEnemiesRoot()
    {
        if (spawnedEnemiesRoot != null) return spawnedEnemiesRoot;

        GameObject root = new GameObject("SpawnedEnemies");
        root.transform.SetParent(transform);
        spawnedEnemiesRoot = root.transform;
        return spawnedEnemiesRoot;
    }
}
