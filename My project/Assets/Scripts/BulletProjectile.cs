using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    // Rigidbody used to move the projectile forward after it is spawned.
    private Rigidbody bulletRigidbody;

    // Reference to the player controller that created this bullet.
    // The projectile uses it to request material-specific haptic feedback.
    private ThirdPersonShooterController shooterController;

    // Called by ThirdPersonShooterController immediately after the bullet is created.
    public void Initialize(ThirdPersonShooterController controller)
    {
        shooterController = controller;
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
        // The target's Unity tag identifies which haptic profile should be used.
        string materialTag = other.tag;

        // Only these material tags produce impact feedback.
        // Other objects are still hit, but they do not vibrate the controller.
        if (materialTag == "Marble" || materialTag == "Glass" || materialTag == "Metal")
        {
            shooterController?.TriggerImpactHaptics(materialTag);
            Debug.Log($"Bullet hit {materialTag}!");
        }

        // A bullet is removed after its first trigger collision.
        Destroy(gameObject);
    }

}
