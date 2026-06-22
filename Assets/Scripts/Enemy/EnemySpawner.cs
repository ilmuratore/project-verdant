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

    [Header("Timing")]
    [Tooltip("Pausa prima di far partire l'ondata successiva.")]
    public float pausaTraOndate = 1.5f;

    [Header("State")]
    [SerializeField] private SpawnerState stato = SpawnerState.Inattivo;

    private QuestData quest;
    private int indiceOndataCorrente = 0;
    private Coroutine routineOndate;

    private Dictionary<int, GameObject> occupiedPoints = new Dictionary<int, GameObject>();

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

        occupiedPoints.Clear();

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
            foreach (var coppia in occupiedPoints)
            {
                if (coppia.Value != null)
                {
                    Destroy(coppia.Value);
                }
            }
        }

        occupiedPoints.Clear();
        stato = SpawnerState.Inattivo;
    }

    private IEnumerator GestisciOndate()
    {
        while (stato == SpawnerState.InCorso && indiceOndataCorrente < quest.ondate.Count)
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

        CleanEnemyList();

        List<int> puntiLiberi = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null) continue;

            if (!occupiedPoints.ContainsKey(i))
            {
                puntiLiberi.Add(i);
            }
        }

        if (puntiLiberi.Count == 0)
        {
            Debug.LogWarning("Nessuno spawn point libero.");
            return;
        }

        int scelto = puntiLiberi[Random.Range(0, puntiLiberi.Count)];
        Transform selectedSpawn = spawnPoints[scelto];

        GameObject enemy = Instantiate(
            enemyPrefab,
            selectedSpawn.position,
            Quaternion.identity
        );

        Enemy_AI ai = enemy.GetComponent<Enemy_AI>();

        if (ai == null)
        {
            Debug.LogWarning("Enemy_AI non presente sul prefab nemico.");
        }

        occupiedPoints.Add(scelto, enemy);
    }

    private bool OndataAzzerata()
    {
        CleanEnemyList();
        return occupiedPoints.Count == 0;
    }

    private void CleanEnemyList()
    {
        List<int> daRimuovere = new List<int>();

        foreach (var coppia in occupiedPoints)
        {
            GameObject enemy = coppia.Value;

            if (enemy == null || !enemy.activeInHierarchy)
            {
                daRimuovere.Add(coppia.Key);
            }
        }

        foreach (int indice in daRimuovere)
        {
            occupiedPoints.Remove(indice);
        }
    }
}
