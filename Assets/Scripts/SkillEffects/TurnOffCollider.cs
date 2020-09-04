using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/TurnOffCollider")]
    public class TurnOffCollider : SkillEffect {
        [Range (0f, 1f)]
        public float TurnOffTiming = 0.6f;
        [Range (0f, 1f)]
        public float TurnOnTiming = 0.8f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= TurnOffTiming) {
                if (!stateEffect.CharacterControl.CharacterData.IsColliderOff)
                    stateEffect.CharacterControl.TurnOffCollider ();

            }
            if (stateInfo.normalizedTime >= TurnOnTiming) {
                if (stateEffect.CharacterControl.CharacterData.IsColliderOff)
                    stateEffect.CharacterControl.TurnOnCollider ();

            }
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateEffect.CharacterControl.CharacterData.IsColliderOff)
                stateEffect.CharacterControl.TurnOnCollider ();
        }

    }
}