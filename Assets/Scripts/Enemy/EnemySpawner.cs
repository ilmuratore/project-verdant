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
    public Transform player;

    [Header("Timing")]
    [Tooltip("Pausa prima di far partire l'ondata successiva")]
    public float pausaTraOndate = 1.5f;

    [Header("State")]
    [SerializeField] private SpawnerState stato = SpawnerState.Inattivo;

    private QuestData quest;
    private int indiceOndataCorrente = 0;

    private Dictionary<int, GameObject> occupiedPoints = new Dictionary<int, GameObject>();

    
    public void AvviaQuest(QuestData questData)
    {
        if (stato == SpawnerState.InCorso) return;
        quest = questData;
        indiceOndataCorrente = 0;
        StartCoroutine(GestisciOndate());
    }

    private IEnumerator GestisciOndate()
    {
        while (indiceOndataCorrente < quest.ondate.Count)
        {
            Ondata ondate = quest.ondate[indiceOndataCorrente];
            SpawnaOndata(ondate);

            yield return new WaitUntil(() => OndataAzzerata());
            indiceOndataCorrente++;
            if(indiceOndataCorrente < quest.ondate.Count)
            {
                yield return new WaitForSeconds(pausaTraOndate);
            }
        }
        stato = SpawnerState.Completato;
    }


    private void SpawnaOndata(Ondata ondata)
    {
        for(int i = 0; i < ondata.numeroNemici; i++)
        {
            SpawnSingolo();
        }
    }


    private void SpawnSingolo()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemyPrefab non presente");
            return;
        }
        if(spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Nessun spawnpoint valido");
            return;
        }

        CleanEnemyList();

        List<int> puntiLiberi = new List<int>();
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            if (!occupiedPoints.ContainsKey(i))
            {
                puntiLiberi.Add(i);
            }
        }
        if(puntiLiberi.Count == 0)
        {
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
        if( ai != null)
        {
            ai.player = player;
        } else
        {
            Debug.LogWarning("Enemy_AI non presente sul prefab");
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
            if (coppia.Value == null)
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