using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/DodgeCooldown")]
    public class DodgeCooldown : SkillEffect {

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            //animator.SetBool (TransitionParameter.Move.ToString (), false);
            stateEffect.CharacterControl.DodgeCoolDown();

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}