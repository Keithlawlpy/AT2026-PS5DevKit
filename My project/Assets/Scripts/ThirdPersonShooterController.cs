using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.PS5;

#if UNITY_EDITOR
// In the Unity Editor, a DualSense connected to the computer is exposed as DualSenseGamepadPC.
using DualSenseController = UnityEngine.InputSystem.PS5.DualSenseGamepadPC;
#elif UNITY_PS5
// In a PS5 build, the controller is exposed as the native DualSenseGamepad type.
using DualSenseController = UnityEngine.InputSystem.PS5.DualSenseGamepad;
#endif

public class ThirdPersonShooterController : MonoBehaviour
{
    // Existing third-person camera and aiming references.
    [SerializeField] private CinemachineCamera aimVirtualCamera;
    [SerializeField] private float normalSenitivity;
    [SerializeField] private float aimSenitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform bulletSpawnPoint;

    // Each material has its own motor strengths and vibration duration.
    // Low frequency feels deeper; high frequency feels sharper.
    [SerializeField] private HapticProfile marbleImpactHaptics = new HapticProfile(0.25f, 0.85f, 0.08f);
    [SerializeField] private HapticProfile glassImpactHaptics = new HapticProfile(0.05f, 1f, 0.04f);
    [SerializeField] private HapticProfile metalImpactHaptics = new HapticProfile(0.85f, 0.35f, 0.25f);

    // References used by the movement, input, and haptic systems.
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    // Tracks the coroutine that turns the current vibration off.
    private Coroutine hapticStopCoroutine;
#if UNITY_EDITOR || UNITY_PS5
    // Platform-specific DualSense device selected by the preprocessor alias above.
    private DualSenseController dualSense;

    // Prevents the PS5 vibration mode from being configured repeatedly.
    private bool vibrationModeConfigured;
#endif

    // Serializable data that appears as an editable profile in the Unity Inspector.
    [System.Serializable]
    private struct HapticProfile
    {
        [Range(0f, 1f)] public float lowFrequencyMotor;
        [Range(0f, 1f)] public float highFrequencyMotor;
        [Min(0f)] public float duration;

        public HapticProfile(float lowFrequencyMotor, float highFrequencyMotor, float duration)
        {
            this.lowFrequencyMotor = lowFrequencyMotor;
            this.highFrequencyMotor = highFrequencyMotor;
            this.duration = duration;
        }
    }

    private void Awake()
    {
        // Find the required components on the player object.
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
#if UNITY_EDITOR || UNITY_PS5
        // Try to select the DualSense when the scene starts.
        // TryConfigureDualSense is called again when an impact occurs in case
        // the controller was connected after Play Mode began.
        TryConfigureDualSense();
#endif
    }

    private void Update()
    {
        // Enable the aim camera and reduce look sensitivity while aiming.
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSenitivity);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSenitivity);
        }

        // Cast a ray from the center of the screen to find the point being aimed at.
        Vector2 screenCenterpPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterpPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            
        }

        Vector3 worldAimTarget = raycastHit.point;

        if (starterAssetsInputs.shoot)
        {
            // Spawn the projectile toward the point found by the aiming raycast.
            Vector3 aimDirection = (worldAimTarget - bulletSpawnPoint.position).normalized;
            Transform projectileTransform = Instantiate(pfBulletProjectile, bulletSpawnPoint.position, Quaternion.LookRotation(aimDirection, Vector3.up));
            BulletProjectile projectile = projectileTransform.GetComponent<BulletProjectile>();

            if (projectile != null)
            {
                // Give the bullet access to this controller so it can report material impacts.
                projectile.Initialize(this);
            }

            // Reset the one-shot input so one press creates one bullet.
            starterAssetsInputs.shoot = false;


        }
    }

    public void TriggerImpactHaptics(string materialTag)
    {
        // Select the vibration profile based on the tag of the object hit by the bullet.
        switch (materialTag)
        {
            case "Marble":
                TriggerHaptics(marbleImpactHaptics);
                break;
            case "Glass":
                TriggerHaptics(glassImpactHaptics);
                break;
            case "Metal":
                TriggerHaptics(metalImpactHaptics);
                break;
        }
    }

    private void TriggerHaptics(HapticProfile profile)
    {
        // Forward the profile values to the motor-control method.
        TriggerHaptics(profile.lowFrequencyMotor, profile.highFrequencyMotor, profile.duration);
    }

    private void TriggerHaptics(float lowFrequencyMotor, float highFrequencyMotor, float duration)
    {
#if UNITY_EDITOR || UNITY_PS5
        // Use the platform-specific DualSense device rather than a generic controller.
        TryConfigureDualSense();
        DualSenseController controller = dualSense;
        if (controller == null)
        {
            return;
        }

        // Stop the previous timer so a new impact can control the full pulse duration.
        if (hapticStopCoroutine != null)
        {
            StopCoroutine(hapticStopCoroutine);
        }

        // Send the two motor strengths to the controller.
        controller.SetMotorSpeeds(lowFrequencyMotor, highFrequencyMotor);
        hapticStopCoroutine = StartCoroutine(StopHaptics(controller, duration));
#endif
    }

#if UNITY_EDITOR || UNITY_PS5
    private void TryConfigureDualSense()
    {
        // Find the current Editor or PS5 DualSense device.
        if (dualSense == null)
        {
            dualSense = DualSenseController.current;
        }

        if (dualSense == null || vibrationModeConfigured)
        {
            return;
        }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
        // Compatible2 allows SetMotorSpeeds to control the DualSense vibration motors.
        dualSense.SetVibrationMode(VibrationMode.Compatible2);
#endif
        vibrationModeConfigured = true;
    }
#endif

#if UNITY_EDITOR || UNITY_PS5
    private IEnumerator StopHaptics(DualSenseController controller, float duration)
    {
        // Keep the motors active only for this material's configured duration.
        yield return new WaitForSecondsRealtime(duration);

        if (controller != null)
        {
            // Zero values stop both vibration motors.
            controller.SetMotorSpeeds(0f, 0f);
        }

        hapticStopCoroutine = null;
    }
#endif


}
