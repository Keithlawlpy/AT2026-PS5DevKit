using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.PS5;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

using UnityEngine.InputSystem.PS5.ControllerSample;


// In the Unity Editor, a DualSense connected to the computer is exposed as DualSenseGamepadPC.

#if UNITY_EDITOR
using DualSensePad = UnityEngine.InputSystem.PS5.DualSenseGamepadPC;
#elif UNITY_PS5
using DualSensePad = UnityEngine.InputSystem.PS5.DualSenseGamepad;
#endif

public class ThirdPersonShooterController : MonoBehaviour
{
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    // Existing third-person camera and aiming references.
    [SerializeField] private CinemachineCamera aimVirtualCamera;
    [SerializeField] private float normalSenitivity;
    [SerializeField] private float aimSenitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform bulletSpawnPoint;

#if UNITY_EDITOR || UNITY_PS5
    // Platform-specific DualSense device selected by the preprocessor alias above.

    public InputUser inputUser { get; private set; }
    public DualSensePad gamepadDevice { get; private set; } //Device on either platform (PS5 or PC)
    public DualSenseGamepad gamepadDevicePS5 => (Gamepad)gamepadDevice as DualSenseGamepad;  //Device on PS5
#if UNITY_EDITOR
    public DualSenseGamepadPC gamepadDevicePC => (Gamepad)gamepadDevice as DualSenseGamepadPC;  //Device on PC
#endif
    public GamepadControls gamepadControls { get; private set; }
    public IInputActionCollection2 actionCollection => gamepadControls;
#endif

    private void Awake()
    {
        // Find the required components on the player object.
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {

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
                TriggerHaptics();
                break;
            case "Glass":
                TriggerHaptics();
                break;
            case "Metal":
                TriggerHaptics();
                break;
        }
    }
    

    private void TriggerHaptics()
    {

    }


}
