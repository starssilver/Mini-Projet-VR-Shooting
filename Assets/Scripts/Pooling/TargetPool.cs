using UnityEngine;
using System.Collections.Generic;

public class TargetPool : MonoBehaviour
{
    public static TargetPool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 30;
    [SerializeField] private Transform poolParent;

    private Queue<GameObject> availableTargets = new Queue<GameObject>();
    private List<GameObject> allTargets = new List<GameObject>();

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Créer le parent si non assigné
        if (poolParent == null)
        {
            poolParent = new GameObject("TargetPoolContainer").transform;
            poolParent.SetParent(transform);
        }

        // Pré-remplir le pool
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewTarget();
        }

        Debug.Log($"TargetPool initialisé avec {initialPoolSize} cibles");
    }

    GameObject CreateNewTarget()
    {
        GameObject target = Instantiate(targetPrefab, poolParent);
        target.SetActive(false);
        allTargets.Add(target);
        availableTargets.Enqueue(target);
        return target;
    }

    public GameObject GetTarget()
    {
        GameObject target;

        // Réutiliser une cible inactive si disponible
        while (availableTargets.Count > 0)
        {
            target = availableTargets.Dequeue();

            // Vérifier que l'objet existe toujours
            if (target != null)
            {
                target.SetActive(true);
                return target;
            }
        }

        // Si le pool est vide, créer une nouvelle cible (si limite non atteinte)
        if (allTargets.Count < maxPoolSize)
        {
            target = CreateNewTarget();
            target.SetActive(true);
            Debug.LogWarning($"Pool de cibles vide : création d'une nouvelle cible ({allTargets.Count}/{maxPoolSize})");
            return target;
        }

        // Si limite atteinte
        Debug.LogError("Pool de cibles saturé !  Limite max atteinte.");
        return null;
    }

    public void ReturnTarget(GameObject target)
    {
        if (target == null) return;

        target.SetActive(false);
        target.transform.SetParent(poolParent);

        // Remettre dans la queue
        if (!availableTargets.Contains(target))
        {
            availableTargets.Enqueue(target);
        }
    }

    public int GetAvailableCount()
    {
        return availableTargets.Count;
    }

    public int GetTotalCount()
    {
        return allTargets.Count;
    }

    // Infos debug
    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUI.Label(new Rect(10, 40, 300, 20), $"Targets: {availableTargets.Count} disponibles / {allTargets.Count} total");
        }
    }
}