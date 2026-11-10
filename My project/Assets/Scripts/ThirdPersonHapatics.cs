using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.PS5;
using UnityEngine.Serialization;

using DualSenseGamePad = UnityEngine.InputSystem.PS5.DualSenseGamepad;

public class Third_person_hapatics : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_PS5
    // The one DualSense device used by this haptics component.
    DualSenseGamePad m_Gamepad;
    private Coroutine stopHapticsCoroutine;
    private bool vibrationModeConfigured;
#endif

    // Each material has a different vibration profile.
    [SerializeField] private HapticProfile marbleImpactHaptics = new HapticProfile(25f, 85f, 8f);
    [SerializeField] private HapticProfile glassImpactHaptics = new HapticProfile(5f, 10f, 4f);
    [SerializeField] private HapticProfile metalImpactHaptics = new HapticProfile(85f, 35f, 25f);

    private void Start()
    {
#if UNITY_EDITOR || UNITY_PS5
        TryConfigureDualSense();
        m_Gamepad.SetVibrationMode(VibrationMode.Compatible);
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

    private void Update()
    {
        m_Gamepad.SetVibrationMode(VibrationMode.Compatible);
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
        Debug.Log("triggering haptics");
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
        Debug.Log("triggered motor");
        stopHapticsCoroutine = StartCoroutine(StopHapticsAfterDelay(profile.duration));
#endif
    }

#if UNITY_EDITOR || UNITY_PS5
    private void TryConfigureDualSense()
    {
            m_Gamepad = DualSenseGamepad.GetBySlotIndex(0);

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
            Debug.Log("stopped motor");
        }

        stopHapticsCoroutine = null;
    }
    /*
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
    */
#endif

}