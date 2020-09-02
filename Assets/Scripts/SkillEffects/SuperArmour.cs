using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/SuperArmour")]
    public class SuperArmour : SkillEffect {
        [Range (0f, 10f)]
        public float SuperArmourBeginTiming = 0f;
        [Range (0f, 10f)]
        public float SuperArmourEndTiming = 1f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (SuperArmourBeginTiming == 0f)
                if (!stateEffect.CharacterControl.CharacterData.IsSuperArmour)
                    stateEffect.CharacterControl.CharacterData.IsSuperArmour = true;

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (!stateEffect.CharacterControl.CharacterData.IsSuperArmour && stateInfo.normalizedTime >= SuperArmourBeginTiming  && stateInfo.normalizedTime < SuperArmourEndTiming) {
                stateEffect.CharacterControl.CharacterData.IsSuperArmour = true;
            }
            if (stateEffect.CharacterControl.CharacterData.IsSuperArmour && stateInfo.normalizedTime >= SuperArmourEndTiming) {
                stateEffect.CharacterControl.CharacterData.IsSuperArmour = false;
            }
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateEffect.CharacterControl.CharacterData.IsSuperArmour)
                stateEffect.CharacterControl.CharacterData.IsSuperArmour = false;

        }

    }
}