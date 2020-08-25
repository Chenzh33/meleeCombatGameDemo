using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/SkipAnimation")]
    public class SkipAnimation : SkillEffect {

        [Range (0f, 1f)]
        public float SkipLength = 0.6f;

        public string PrevStateName;

        public float SpeedUp = 10f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateEffect.CharacterControl.CharacterData.GetPrevState () == Animator.StringToHash (PrevStateName))
            {
                animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), SpeedUp);
                Debug.Log("trigger skip animation!");

            }

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime > SkipLength && animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) > 1.0f)
                animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1.0f);

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1.0f);
        }

    }
}