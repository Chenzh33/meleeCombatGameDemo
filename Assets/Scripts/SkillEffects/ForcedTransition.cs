using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ForcedTransition")]
    public class ForcedTransition : SkillEffect {
        [Range (0.01f, 2f)]
        public float transitionTime = 0.8f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (animatorStateInfo.normalizedTime >= transitionTime)
                animator.SetBool (TransitionParameter.ForcedTransition.ToString (), true);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            animator.SetBool (TransitionParameter.ForcedTransition.ToString (), false);
            animator.SetBool (TransitionParameter.Move.ToString (), false);
        }
    }
}