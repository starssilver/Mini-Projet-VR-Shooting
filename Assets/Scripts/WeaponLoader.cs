using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WeaponLoader : MonoBehaviour
{
    [Header("Addressable Keys")]
    [SerializeField] private string minigunPCVR_Key = "Minigun_PCVR";
    [SerializeField] private string minigunQuest_Key = "Minigun_Quest";

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool loadOnStart = true;

    private AsyncOperationHandle<GameObject> loadHandle;
    private GameObject loadedWeapon;

    void Start()
    {
        Debug.Log("=== WeaponLoader Start ===");
        Debug.Log($"Spawn Point assigné : {spawnPoint != null}");

        if (loadOnStart)
        {
            LoadWeapon();
        }
    }

    public void LoadWeapon()
    {
        // Déterminer quelle arme charger selon la plateforme
        string keyToLoad = GetPlatformSpecificKey();
        Debug.Log($"[WeaponLoader] Plateforme détectée : {Application.platform}");
        Debug.Log($"[WeaponLoader] Key à charger : {keyToLoad}");

        // Charger l'asset via Addressables
        loadHandle = Addressables.LoadAssetAsync<GameObject>(keyToLoad);
        loadHandle.Completed += OnWeaponLoaded;
    }

    string GetPlatformSpecificKey()
    {
#if UNITY_ANDROID
        return minigunQuest_Key;
#elif UNITY_STANDALONE
            return minigunPCVR_Key;
#else
            // En éditeur, charger la version PCVR par défaut
            return minigunPCVR_Key;
#endif
    }

    void OnWeaponLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // Instancier l'arme
            GameObject weaponPrefab = handle.Result;

            Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

            loadedWeapon = Instantiate(weaponPrefab, position, rotation);

            Debug.Log($"Arme chargée et instanciée : {loadedWeapon.name}");
        }
        else
        {
            Debug.LogError($"Échec du chargement de l'arme : {handle.OperationException}");
        }
    }

    void OnDestroy()
    {
        // Libérer la mémoire quand l'objet est détruit
        if (loadHandle.IsValid())
        {
            Addressables.Release(loadHandle);
            Debug.Log("Handle Addressable libéré");
        }
    }

    // Pour décharger manuellement l'arme
    public void UnloadWeapon()
    {
        if (loadedWeapon != null)
        {
            Destroy(loadedWeapon);
            loadedWeapon = null;
        }

        if (loadHandle.IsValid())
        {
            Addressables.Release(loadHandle);
        }

        Debug.Log("Arme déchargée");
    }
}