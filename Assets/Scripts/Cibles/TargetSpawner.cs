using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] targetPrefabs; // Tableau de prefabs
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(5f, 3f, 0f);

    void Start()
    {
        InvokeRepeating(nameof(SpawnTarget), 0f, spawnInterval);
    }

    void SpawnTarget()
    {
        if (targetPrefabs.Length == 0) return;

        // Choisir un prefab aléatoire
        GameObject randomPrefab = targetPrefabs[Random.Range(0, targetPrefabs.Length)];

        // Position aléatoire dans la zone du mur
        Vector3 randomPos = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            0f
        );

        Vector3 spawnPos = transform.position + transform.TransformDirection(randomPos);

        Instantiate(randomPrefab, spawnPos, transform.rotation);
    }
}