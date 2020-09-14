using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/SpeedGraph")]
    public class SpeedGraph : SkillEffect{
        public AnimationCurve graph;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

            animator.SetFloat(TransitionParameter.SpeedMultiplier.ToString (), graph.Evaluate(0f));
          

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetFloat(TransitionParameter.SpeedMultiplier.ToString (), graph.Evaluate(stateInfo.normalizedTime));

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetFloat(TransitionParameter.SpeedMultiplier.ToString (), 1.0f);

        }

    }
}