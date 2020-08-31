using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Stunned")]
    public class Stunned : SkillEffect {

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateEffect.CharacterControl.CharacterData.Armour >= stateEffect.CharacterControl.CharacterData.MaxArmour) {
                stateEffect.CharacterControl.CharacterData.Armour = stateEffect.CharacterControl.CharacterData.MaxArmour;
                stateEffect.CharacterControl.CharacterData.IsStunned = false;
                animator.SetBool (TransitionParameter.Stunned.ToString (), false);
            } else {
                stateEffect.CharacterControl.CharacterData.Armour += stateEffect.CharacterControl.CharacterData.ArmourRegenerationInStun * Time.deltaTime;
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}