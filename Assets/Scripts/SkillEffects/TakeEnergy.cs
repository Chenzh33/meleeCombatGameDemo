using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/TakeEnergy")]
    public class TakeEnergy : SkillEffect {
        public float EnergyTakingAmount = 2.0f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if(stateEffect.CharacterControl.CharacterData.Energy >= EnergyTakingAmount)
            {
                stateEffect.CharacterControl.TakeEnergy(EnergyTakingAmount, this);

            }
            else
            {
                animator.CrossFade("NotEnoughEnergy", 0.25f);

            }

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
           

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}