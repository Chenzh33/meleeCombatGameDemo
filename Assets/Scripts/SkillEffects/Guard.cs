using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Guard")]
    public class Guard : SkillEffect {
        public bool IsGuardPrep;
        public int MaxBlockFrame = 30;
        public float MaxBlockAttackCount = 4;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if(IsGuardPrep)
            {
                stateEffect.CharacterControl.CharacterData.IsGuarding = true;
                stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock = 0;
                stateEffect.CharacterControl.CharacterData.BlockCount = MaxBlockAttackCount;
            }
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock < MaxBlockFrame)
                stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock++;

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if(!IsGuardPrep)
            {
                stateEffect.CharacterControl.CharacterData.IsGuarding = false;
                //stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock = 0;
                stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock = MaxBlockFrame;
                stateEffect.CharacterControl.CharacterData.BlockCount = 0f;
                animator.SetBool(TransitionParameter.Move.ToString(), false);
            }
        }

    }
}