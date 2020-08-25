using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Invincible")]
    public class Invincible : SkillEffect {
        [Range (0f, 10f)]
        public float InvincibleBeginTiming = 0f;
        [Range (0f, 10f)]
        public float InvincibleEndTiming = 1f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (InvincibleBeginTiming == 0f)
                if (!stateEffect.CharacterControl.CharacterData.IsInvincible)
                    stateEffect.CharacterControl.CharacterData.IsInvincible = true;

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (!stateEffect.CharacterControl.CharacterData.IsInvincible && stateInfo.normalizedTime >= InvincibleBeginTiming && stateInfo.normalizedTime < InvincibleEndTiming) {
                stateEffect.CharacterControl.CharacterData.IsInvincible = true;
            }
            if (stateEffect.CharacterControl.CharacterData.IsInvincible && stateInfo.normalizedTime >= InvincibleEndTiming) {
                stateEffect.CharacterControl.CharacterData.IsInvincible = false;
            }
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateEffect.CharacterControl.CharacterData.IsInvincible)
                stateEffect.CharacterControl.CharacterData.IsInvincible = false;

        }

    }
}