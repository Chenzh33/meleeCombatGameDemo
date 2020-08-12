using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/TransitionIndexer")]
    public class TransitionIndexer : SkillEffect {

        [Range (0.01f, 1f)]
        public float AllowTransitionStartTime = 0.2f;
        [Range (0.01f, 1f)]
        public float AllowTransitionEndTime = 0.2f;
        public int Index;
        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
           

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
         
            if (stateInfo.normalizedTime >= AllowTransitionStartTime && stateInfo.normalizedTime < AllowTransitionEndTime) {
                animator.SetBool (TransitionParameter.CheckCombo.ToString (), true);
            } else {
                animator.SetBool (TransitionParameter.CheckCombo.ToString (), false);
            }


        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
        

        }
      
        
    }
}