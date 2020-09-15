using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/GetHit")]
    public class GetHit : SkillEffect {

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
          

        }

        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateEffect.CharacterControl.CharacterData.GetHitTime > 0f) {
                stateEffect.CharacterControl.CharacterData.GetHitTime -= Time.deltaTime;
            } 
            else if (stateEffect.CharacterControl.CharacterData.GetHitTime < 0f && !animator.IsInTransition (0)) {
                stateEffect.CharacterControl.CharacterData.GetHitTime = 0f;
                //animator.SetInteger(TransitionParameter.TransitionIndexer.ToString(), 2);
                //animator.CrossFade ("Idle", 0.2f, -1, 0f, stateInfo.normalizedTime);
                animator.CrossFade ("Idle", 0.2f);
                //animator.Play("Idle");
            }
            
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}