using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Dodge")]
    public class Dodge : SkillEffect {

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

            if (stateEffect.CharacterControl.isPlayerControl)
                VirtualInputManager.Instance.ClearAllInputsInBuffer ();
                //stateEffect.CharacterControl.DodgeTrigger = true;
            animator.SetBool (TransitionParameter.ForcedTransitionDodge.ToString (), false);

            animator.SetBool (TransitionParameter.Dodge.ToString (), false);
            //animator.SetBool (TransitionParameter.Dodge.ToString (), false);
            stateEffect.CharacterControl.CommandDodge = false;
            //animator.SetBool (TransitionParameter.Move.ToString (), false);

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetBool (TransitionParameter.Move.ToString (), false);

        }

    }
}