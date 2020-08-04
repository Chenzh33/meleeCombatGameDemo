using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {
    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ForcedTransition")]
    public class Attack : SkillEffect {
        [Range (0.01f, 1f)]
        public float attackBeginTime = 0.4f;
        public float attackEndTime = 0.6f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
    }
}