using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem destructionEffect_PCVR;
    [SerializeField] private ParticleSystem destructionEffect_Quest;

    [Header("Target Settings")]
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private bool autoRespawn = true;

    private Renderer targetRenderer;
    private Collider targetCollider;
    private Material originalMaterial;
    private Vector3 originalScale;
    private Quaternion originalRotation;

    void Awake()
    {
        // Sauvegarder les références
        targetRenderer = GetComponent<Renderer>();
        targetCollider = GetComponent<Collider>();

        // Sauvegarder l'état initial
        if (targetRenderer != null)
        {
            originalMaterial = targetRenderer.material;
        }
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
    }

    void OnEnable()
    {
        // Réinitialiser l'état de la cible à chaque activation
        ResetTarget();
    }

    void ResetTarget()
    {
        // Réinitialiser l'échelle et la rotation
        transform.localScale = originalScale;
        transform.localRotation = originalRotation;

        // Réinitialiser le matériau
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.material = originalMaterial;
        }

        // Réactiver le collider
        if (targetCollider != null)
        {
            targetCollider.enabled = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Vérifier si c'est un projectile
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            OnHit();
        }
    }

    public void OnHit()
    {
        // Jouer l'effet de destruction
        PlayDestructionEffect();

        // Désactiver le collider immédiatement
        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }

        // Lancer le respawn si auto-respawn activé
        if (autoRespawn)
        {
            StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            ReturnToPool();
        }
    }

    void PlayDestructionEffect()
    {
        ParticleSystem effectToPlay = null;

#if UNITY_ANDROID
        effectToPlay = destructionEffect_Quest;
#elif UNITY_STANDALONE
            effectToPlay = destructionEffect_PCVR;
#else
            effectToPlay = (destructionEffect_PCVR != null) ? destructionEffect_PCVR : destructionEffect_Quest;
#endif

        if (effectToPlay != null)
        {
            // Créer une copie indépendante de l'effet
            ParticleSystem effect = Instantiate(effectToPlay, transform.position, Quaternion.identity);
            effect.transform.SetParent(null);
            effect.gameObject.SetActive(true);
            effect.Play();

            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        // Optionnel : animation de disparition (scale down)
        float elapsed = 0f;
        float shrinkDuration = 0.2f;
        Vector3 startScale = transform.localScale;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        // Attendre le délai de respawn
        yield return new WaitForSeconds(respawnDelay - shrinkDuration);

        // Demander un respawn au spawner
        if (TargetSpawner.Instance != null)
        {
            TargetSpawner.Instance.RespawnTarget(gameObject);
        }
        else
        {
            // Fallback :  retourner directement au pool
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        if (TargetPool.Instance != null)
        {
            TargetPool.Instance.ReturnTarget(gameObject);
        }
        else
        {
            // Fallback si le pool n'existe pas
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        // Arrêter toutes les coroutines en cours
        StopAllCoroutines();
    }
}