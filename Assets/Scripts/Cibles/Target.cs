using UnityEngine;

public class Target : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Projectile>() != null)
        {
            Destroy(gameObject);
        }
    }
}