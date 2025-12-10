using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    public static TargetSpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int numberOfTargetsToSpawn = 5;

    [Header("Spawn Mode")]
    [SerializeField] private SpawnMode spawnMode = SpawnMode.Random;

    public enum SpawnMode
    {
        Random,      // Position aléatoire parmi les spawn points
        Sequential,  // Dans l'ordre des spawn points
        AllAtOnce    // Toutes les positions en même temps
    }

    private int currentSpawnIndex = 0;
    private List<GameObject> activeTargets = new List<GameObject>();

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnInitialTargets();
        }
    }

    void SpawnInitialTargets()
    {
        if (spawnMode == SpawnMode.AllAtOnce)
        {
            // Spawn une cible à chaque spawn point
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                SpawnTargetAtIndex(i);
            }
        }
        else
        {
            // Spawn le nombre demandé de cibles
            for (int i = 0; i < numberOfTargetsToSpawn; i++)
            {
                SpawnTarget();
            }
        }
    }

    public void SpawnTarget()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Aucun spawn point défini !");
            return;
        }

        int spawnIndex = GetNextSpawnIndex();
        SpawnTargetAtIndex(spawnIndex);
    }

    void SpawnTargetAtIndex(int index)
    {
        if (index < 0 || index >= spawnPoints.Length)
        {
            Debug.LogError($"Index de spawn invalide : {index}");
            return;
        }

        if (TargetPool.Instance == null)
        {
            Debug.LogError("TargetPool. Instance est null !");
            return;
        }

        // Récupérer une cible du pool
        GameObject target = TargetPool.Instance.GetTarget();
        if (target == null)
        {
            Debug.LogWarning("Impossible de récupérer une cible du pool");
            return;
        }

        // Positionner la cible
        Transform spawnPoint = spawnPoints[index];
        target.transform.position = spawnPoint.position;
        target.transform.rotation = spawnPoint.rotation;
        target.transform.SetParent(null); // Détacher du pool parent

        // Ajouter aux cibles actives
        if (!activeTargets.Contains(target))
        {
            activeTargets.Add(target);
        }
    }

    int GetNextSpawnIndex()
    {
        int index = 0;

        switch (spawnMode)
        {
            case SpawnMode.Random:
                index = Random.Range(0, spawnPoints.Length);
                break;

            case SpawnMode.Sequential:
                index = currentSpawnIndex;
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
                break;

            case SpawnMode.AllAtOnce:
                index = currentSpawnIndex;
                currentSpawnIndex++;
                break;
        }

        return index;
    }

    public void RespawnTarget(GameObject oldTarget)
    {
        // Retourner l'ancienne cible au pool
        if (TargetPool.Instance != null)
        {
            TargetPool.Instance.ReturnTarget(oldTarget);
        }

        // Retirer des cibles actives
        activeTargets.Remove(oldTarget);

        // Spawn une nouvelle cible
        SpawnTarget();
    }

    public void DespawnAllTargets()
    {
        for (int i = activeTargets.Count - 1; i >= 0; i--)
        {
            GameObject target = activeTargets[i];
            if (target != null && TargetPool.Instance != null)
            {
                TargetPool.Instance.ReturnTarget(target);
            }
        }
        activeTargets.Clear();
    }

    // Infos debug
    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.green;
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
                Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + spawnPoint.forward * 0.5f);
            }
        }
    }
}