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
        if (other.CompareTag("Marble"))
        {
            Debug.Log("Bullet hit Marble!");
        }
        else if(other.CompareTag("Metal"))
        {
            Debug.Log("Bullet hit Metal!");
        }
        else if (other.CompareTag("Glass"))
        {
            Debug.Log("Bullet hit Glass!");
        }
        else
        {
            Debug.Log("Bullet hit something else!");
        }


        Destroy(gameObject);
    }

}
