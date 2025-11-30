using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;

    private GameObject impactEffect;
    private Rigidbody rb;
    private GameObject shooter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    public void SetShooter(GameObject weapon)
    {
        shooter = weapon;
    }

    public void SetImpactEffect(GameObject effect)
    {
        impactEffect = effect;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignorer l'arme qui a tiré
        if (collision.gameObject == shooter)
        {
            return;
        }

        // Ignorer les autres projectiles
        if (collision.gameObject.GetComponent<Projectile>() != null)
        {
            return;
        }

        // Créer l'effet d'impact
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }

        Destroy(gameObject);
    }
}