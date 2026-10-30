using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 20f; 
        bulletRigidbody.linearVelocity = transform.forward * speed; 
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

}
