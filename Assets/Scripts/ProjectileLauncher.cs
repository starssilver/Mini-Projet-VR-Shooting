using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fireRate = 0.1f;

    private ParticleSystem muzzleFlash;
    private GameObject impactEffect;
    private XRGrabInteractable grabInteractable;
    private ParticleEffectManager effectManager;

    void Start()
    {
        // Récupérer le manager d'effets
        effectManager = GetComponent<ParticleEffectManager>();

        if (effectManager != null)
        {
            muzzleFlash = effectManager.GetMuzzleFlash();
            impactEffect = effectManager.GetImpactEffect();
        }

        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.activated.AddListener(OnActivate);
            grabInteractable.deactivated.AddListener(OnDeactivate);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop();
        }
    }

    private void OnActivate(ActivateEventArgs args)
    {
        InvokeRepeating(nameof(Fire), 0f, fireRate);
    }

    private void OnDeactivate(DeactivateEventArgs args)
    {
        CancelInvoke(nameof(Fire));
    }

    void Fire()
    {
        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetShooter(gameObject);
            projectileScript.SetImpactEffect(impactEffect); // Passer le bon effet selon la plateforme
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop();
            muzzleFlash.Play();
        }
    }
}