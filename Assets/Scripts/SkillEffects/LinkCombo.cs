using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/LinkCombo")]
    public class LinkCombo : SkillEffect {
        public TransitionParameter LinkType = TransitionParameter.AttackMeleeLink;

        [Range (0f, 1f)]
        public float LinkComboWindowBegin;
        [Range (0f, 1f)]
        public float LinkComboWindowEnd;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            animator.SetInteger (LinkType.ToString (), 0); // standby
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (animator.GetInteger (LinkType.ToString ()) == 0) {
                if (CheckLink (stateEffect.CharacterControl)) {
                    if (stateInfo.normalizedTime > LinkComboWindowBegin && stateInfo.normalizedTime <= LinkComboWindowEnd)
                        animator.SetInteger (LinkType.ToString (), 1); // succeed 
                    else
                        animator.SetInteger (LinkType.ToString (), 2); // fail 

                }
            }
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetInteger (LinkType.ToString (), 0); // standby
        }
        public bool CheckLink (CharacterControl control) {
            bool[] input = control.inputKeyStates;
            switch (LinkType) {
                case TransitionParameter.AttackMeleeLink:
                    if (input[(int)InputKeyType.KEY_MELEE_ATTACK * 3])
                        return true;
                    break;
            }
            return false;
        }

    }
}