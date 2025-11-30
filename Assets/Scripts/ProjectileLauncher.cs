using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fireRate = 0.1f;

    private XRGrabInteractable grabInteractable;
    private float nextFireTime = 0f;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.activated.AddListener(OnActivate);
            grabInteractable.deactivated.AddListener(OnDeactivate);
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
        Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}