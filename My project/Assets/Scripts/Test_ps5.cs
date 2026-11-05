using UnityEngine;
using UnityEngine.InputSystem.PS5;

public class Test_ps5 : MonoBehaviour
{
    private DualSenseGamepad pad;

    private void Start()
    {
        pad = DualSenseGamepad.current;
        ApplyTriggerEffects();
        Debug.Log("trigger effact applied");
    }
    private void ApplyTriggerEffects()
    {
        var effect = new TriggerEffectParam(TriggerEffectMask.L2 | TriggerEffectMask.R2);

        effect.left.mode = TriggerEffectMode.Feedback;
        effect.left.feedback.position = 2;
        effect.left.feedback.strength = 2;

        effect.right.mode = TriggerEffectMode.Weapon;
        effect.right.weapon.startPosition = 2;
        effect.right.weapon.endPosition = 8;
        effect.right.weapon.strength = 6;

        pad.SetTriggerEffect(effect);
    }

    private void OnDisable()
    {
        if (pad == null)
        {
            return;
        }

        var effect = new TriggerEffectParam(TriggerEffectMask.L2 | TriggerEffectMask.R2);
        effect.left.mode = TriggerEffectMode.Off;
        effect.right.mode = TriggerEffectMode.Off;
        pad.SetTriggerEffect(effect);
    }

}
