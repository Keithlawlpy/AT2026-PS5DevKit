using UnityEngine;
using UnityEngine.InputSystem.PS5;
using UnityEngine.InputSystem.Users;

public class Third_person_hapatics : MonoBehaviour
{
    DualSenseGamepad m_Gamepad;


    //different haptics
    [SerializeField] private HapticProfile marbleImpactHaptics = new HapticProfile(0.25f, 0.85f, 0.08f);
    [SerializeField] private HapticProfile glassImpactHaptics = new HapticProfile(0.05f, 1f, 0.04f);
    [SerializeField] private HapticProfile metalImpactHaptics = new HapticProfile(0.85f, 0.35f, 0.25f);

    void Start() //or some other function to trigger the effect
    {

        Debug.Log("Motor Speeds set to 1f, 1f");
    }


    void Update()
    {

        var m_Gamepad = DualSenseGamepad.GetBySlotIndex(0);

        //Vibration mode only needs to be set once
        m_Gamepad.SetVibrationMode(VibrationMode.Compatible);

        m_Gamepad.SetMotorSpeeds(1f, 1f);

        Debug.Log("Motor Running");
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




}