using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.PS5;

#if UNITY_EDITOR
using DualSenseGamePad = UnityEngine.InputSystem.PS5.DualSenseGamepadPC;
#elif UNITY_PS5
using DualSenseGamePad = UnityEngine.InputSystem.PS5.DualSenseGamepad;
#endif

public class Third_person_hapatics : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_PS5
    [Header("Advanced Vibration Audio")]
    [SerializeField] private AudioSource vibrationSource;
    [SerializeField] private AudioClip marbleVibrationClip;
    [SerializeField] private AudioClip glassVibrationClip;
    [SerializeField] private AudioClip metalVibrationClip;

    private DualSenseGamePad m_Gamepad;
    private Coroutine stopHapticsCoroutine;
    private bool vibrationModeConfigured;
#endif

    private void Start()
    {
#if UNITY_EDITOR || UNITY_PS5
        ConfigureVibrationSource();
        TryConfigureDualSense();
#endif
    }

    public void TriggerImpactHaptics(string materialTag)
    {
        AudioClip materialClip = null;

        switch (materialTag)
        {
            case "Marble":
                materialClip = marbleVibrationClip;
                break;
            case "Glass":
                materialClip = glassVibrationClip;
                break;
            case "Metal":
                materialClip = metalVibrationClip;
                break;
        }

        if (materialClip != null)
        {
            TriggerHaptics(materialClip);
        }
    }

    private void TriggerHaptics(AudioClip materialClip)
    {
#if UNITY_EDITOR || UNITY_PS5
        TryConfigureDualSense();

        if (m_Gamepad == null)
        {
            Debug.LogWarning("No DualSense controller is available for haptic feedback.");
            return;
        }

        ConfigureVibrationSource();

        if (stopHapticsCoroutine != null)
        {
            StopCoroutine(stopHapticsCoroutine);
        }

        vibrationSource.Stop();
        vibrationSource.clip = materialClip;
        vibrationSource.PlayOnGamepad(m_Gamepad.slotIndex);
        stopHapticsCoroutine = StartCoroutine(StopHapticsAfterDelay(materialClip.length));
#endif
    }

#if UNITY_EDITOR || UNITY_PS5
    private void ConfigureVibrationSource()
    {
        if (vibrationSource == null)
        {
            vibrationSource = gameObject.AddComponent<AudioSource>();
        }

        vibrationSource.playOnAwake = false;
        vibrationSource.loop = false;
        vibrationSource.gamepadSpeakerOutputType = GamepadSpeakerOutputType.Vibration;
    }

    private void TryConfigureDualSense()
    {
        if (m_Gamepad == null)
        {
            m_Gamepad = DualSenseGamePad.current;
        }

        if (m_Gamepad == null || vibrationModeConfigured)
        {
            return;
        }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
        m_Gamepad.SetVibrationMode(VibrationMode.Advanced);
#endif

        vibrationModeConfigured = true;
    }

    private IEnumerator StopHapticsAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (vibrationSource != null)
        {
            vibrationSource.Stop();
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

        if (vibrationSource != null)
        {
            vibrationSource.Stop();
        }
    }
#endif

}