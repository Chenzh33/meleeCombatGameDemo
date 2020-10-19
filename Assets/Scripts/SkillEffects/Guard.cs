using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {
    public enum GuardState {
        Begin,
        HoldOn,
        End
    }

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Guard")]
    public class Guard : SkillEffect {
        //public bool IsGuardPrep;
        public GuardState State;
        public int MaxBlockFrame = 30;
        public float MaxBlockAttackCount = 4;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (State == GuardState.Begin) {
                stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock = 0;
                if (stateEffect.CharacterControl.isPlayerControl)
                    VirtualInputManager.Instance.ClearAllInputsInBuffer ();
            }
            if (State != GuardState.End) {
                stateEffect.CharacterControl.CharacterData.IsGuarding = true;
                stateEffect.CharacterControl.CharacterData.BlockCount = MaxBlockAttackCount;
            } else {
                stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock = MaxBlockFrame;
                stateEffect.CharacterControl.CharacterData.BlockCount = 0f;
                animator.SetBool (TransitionParameter.Move.ToString (), false);
            }
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock < MaxBlockFrame)
                stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock++;

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            //if (!IsGuardPrep) {
            stateEffect.CharacterControl.CharacterData.IsGuarding = false;

/*
            if (State == GuardState.End) {
                //stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock = 0;

            }
            */

        }

    }
}