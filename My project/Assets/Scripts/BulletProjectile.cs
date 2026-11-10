using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    // Rigidbody used to move the projectile forward after it is spawned.
    private Rigidbody bulletRigidbody;

    // Reference to the haptics component that owns the DualSense output.
    private Third_person_hapatics hapticsController;

    // Called by ThirdPersonShooterController immediately after the bullet is created.
    public void Initialize(Third_person_hapatics haptics)
    {
        hapticsController = haptics;
    }

    private void Awake()
    {

        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Move the bullet in the direction it was facing when it was spawned.
        float speed = 20f; 
        bulletRigidbody.linearVelocity = transform.forward * speed; 
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log ("Bullet collided with:" + other.name);
        // The target's Unity tag identifies which haptic profile should be used.
        string materialTag = other.tag;

        // Only these material tags produce impact feedback.
        // Other objects are still hit, but they do not vibrate the controller.
        if (materialTag == "Marble" || materialTag == "Glass" || materialTag == "Metal")
        {
            hapticsController?.TriggerImpactHaptics(materialTag);
            Debug.Log($"Bullet hit {materialTag}!");
        }

        // A bullet is removed after its first trigger collision.
        Destroy(gameObject);
    }

}
