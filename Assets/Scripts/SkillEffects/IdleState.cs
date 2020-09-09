using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/IdleState")]
    public class IdleState : SkillEffect {

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            animator.SetBool (TransitionParameter.IdleState.ToString (), true);

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetBool (TransitionParameter.IdleState.ToString (), false);

        }

    }
}