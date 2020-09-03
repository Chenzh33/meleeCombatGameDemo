using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/TurnOnTrail")]
    public class TurnOnTrail : SkillEffect {
        [Range(0f, 3f)]
        public float TurnOnTrailTiming = 0.9f;
        [Range(0f, 3f)]
        public float TurnOffTrailTiming = 3f;
        [Range(0f, 3f)]
        public float TurnOffTrailTimingMask = 1f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime > TurnOnTrailTiming && stateInfo.normalizedTime <= TurnOffTrailTiming && stateEffect.CharacterControl.VFXTrail.isPaused) {
                stateEffect.CharacterControl.VFXTrail.Play(true);
            }
            if (stateInfo.normalizedTime > TurnOffTrailTiming && !stateEffect.CharacterControl.VFXTrail.isPaused && stateInfo.normalizedTime < TurnOffTrailTimingMask) {
                stateEffect.CharacterControl.VFXTrail.Pause(true);
                stateEffect.CharacterControl.VFXTrail.Clear();
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (!stateEffect.CharacterControl.VFXTrail.isPaused)
            {
                stateEffect.CharacterControl.VFXTrail.Pause(true);
                stateEffect.CharacterControl.VFXTrail.Clear();
            }
        }

    }
}