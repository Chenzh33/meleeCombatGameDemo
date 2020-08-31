using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Dead")]
    public class Dead : SkillEffect {
        [Range(0f, 3f)]
        public float TurnOnRagdollTiming = 0.9f;
        [Range(0f, 3f)]
        public float DestroyTiming = 3f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime > TurnOnRagdollTiming && !stateEffect.CharacterControl.CharacterData.IsRagdollOn) {
                stateEffect.CharacterControl.TurnOnRagdoll();
            }
            if (stateInfo.normalizedTime > DestroyTiming) {
                stateEffect.CharacterControl.DestroyObject();
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}