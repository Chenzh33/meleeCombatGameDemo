using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/GetHit")]
    public class GetHit : SkillEffect {

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (stateEffect.CharacterControl.CharacterData.GetHitTime <= 0f) {
                stateEffect.CharacterControl.CharacterData.GetHitTime = 0f;
                animator.CrossFade ("Idle", 0.1f);
            }

        }

        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateEffect.CharacterControl.CharacterData.GetHitTime > 0f) {
                stateEffect.CharacterControl.CharacterData.GetHitTime -= Time.deltaTime;
            } else if (stateEffect.CharacterControl.CharacterData.GetHitTime < 0f){
                stateEffect.CharacterControl.CharacterData.GetHitTime = 0f;
                animator.CrossFade ("Idle", 0.1f);
            }
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}