using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.PS5;

#if UNITY_EDITOR
using DualSensePad = UnityEngine.InputSystem.PS5.DualSenseGamepadPC;
#elif UNITY_PS5
using DualSensePad = UnityEngine.InputSystem.PS5.DualSenseGamepad;
#endif

public class Third_person_hapatics : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_PS5
    // The one DualSense device used by this haptics component.
    private DualSensePad m_Gamepad;
    private Coroutine stopHapticsCoroutine;
    private bool vibrationModeConfigured;
#endif

    // Each material has a different vibration profile.
    [SerializeField] private HapticProfile marbleImpactHaptics = new HapticProfile(0.25f, 0.85f, 0.08f);
    [SerializeField] private HapticProfile glassImpactHaptics = new HapticProfile(0.05f, 1f, 0.04f);
    [SerializeField] private HapticProfile metalImpactHaptics = new HapticProfile(0.85f, 0.35f, 0.25f);

    private void Start()
    {
#if UNITY_EDITOR || UNITY_PS5
        TryConfigureDualSense();
#endif
    }

    [System.Serializable]
    private class HapticProfile
    {
        public float lowFrequencyMotorSpeed;
        public float highFrequencyMotorSpeed;
        public float duration;
        public HapticProfile(float lowFrequencyMotorSpeed, float highFrequencyMotorSpeed, float duration)
        {
            this.lowFrequencyMotorSpeed = lowFrequencyMotorSpeed;
            this.highFrequencyMotorSpeed = highFrequencyMotorSpeed;
            this.duration = duration;
        }
    }


    public void TriggerImpactHaptics(string materialTag)
    {
        // Select exactly one profile based on the tag of the object hit by the bullet.
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
#if UNITY_EDITOR || UNITY_PS5
        if (profile == null)
        {
            return;
        }

        TryConfigureDualSense();

        if (m_Gamepad == null)
        {
            Debug.LogWarning("No DualSense controller is available for haptic feedback.");
            return;
        }

        if (stopHapticsCoroutine != null)
        {
            StopCoroutine(stopHapticsCoroutine);
        }

        m_Gamepad.SetMotorSpeeds(profile.lowFrequencyMotorSpeed, profile.highFrequencyMotorSpeed);
        stopHapticsCoroutine = StartCoroutine(StopHapticsAfterDelay(profile.duration));
#endif
    }

#if UNITY_EDITOR || UNITY_PS5
    private void TryConfigureDualSense()
    {
        if (m_Gamepad == null)
        {
            m_Gamepad = DualSensePad.current;
        }

        if (m_Gamepad == null || vibrationModeConfigured)
        {
            return;
        }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
        m_Gamepad.SetVibrationMode(VibrationMode.Compatible2);
#endif
        vibrationModeConfigured = true;
    }

    private IEnumerator StopHapticsAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (m_Gamepad != null)
        {
            m_Gamepad.SetMotorSpeeds(0f, 0f);
        }

        stopHapticsCoroutine = null;
    }

    private void OnDisable()
    {
        if (stopHapticsCoroutine != null)
        {
            StopCoroutine(stopHapticsCoroutine);
            stopHapticsCoroutine = null;
        }

        if (m_Gamepad != null)
        {
            m_Gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
#endif

}