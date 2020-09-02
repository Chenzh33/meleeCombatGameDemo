using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/TurnOnHoldVFX")]
    public class TurnOnHoldVFX : SkillEffect {
        [Range(0f, 3f)]
        public float TurnOnVFXTiming = 0f;
        /*
        [Range(0f, 3f)]
        public float TurnOffVFXTiming = 3f;
        */

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            //if (stateInfo.normalizedTime > TurnOnVFXTiming && stateInfo.normalizedTime <= TurnOffVFXTiming && stateEffect.CharacterControl.VFXHold.isPaused) {
            if (stateInfo.normalizedTime > TurnOnVFXTiming && stateEffect.CharacterControl.VFXHold.isPaused) {
                stateEffect.CharacterControl.VFXHold.Play(true);
            }
            /*
            if (stateInfo.normalizedTime > TurnOffVFXTiming && !stateEffect.CharacterControl.VFXHold.isPaused) {
                stateEffect.CharacterControl.VFXHold.Pause(true);
                stateEffect.CharacterControl.VFXHold.Clear();
            }
            */

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (!stateEffect.CharacterControl.VFXHold.isPaused)
            {
                stateEffect.CharacterControl.VFXHold.Pause(true);
                stateEffect.CharacterControl.VFXHold.Clear();
            }
        }

    }
}