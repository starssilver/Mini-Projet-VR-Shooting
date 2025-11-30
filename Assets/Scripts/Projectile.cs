using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifetime);
    }
}