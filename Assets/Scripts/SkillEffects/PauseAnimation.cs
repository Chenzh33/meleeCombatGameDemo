using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/PauseAnimation")]
    public class PauseAnimation : SkillEffect {
        [Range (0f, 1f)]
        public float PauseTiming = 0.95f;
        [Range (0f, 1f)]
        public float ResumeTiming = 1.0f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= PauseTiming) {
                if (!stateEffect.CharacterControl.CharacterData.IsAnimationPause)
                    animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0.05f);

            }
            if (stateInfo.normalizedTime >= ResumeTiming) {
                if (stateEffect.CharacterControl.CharacterData.IsAnimationPause)
                    animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1f);

            }
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1f);
        }

    }
}