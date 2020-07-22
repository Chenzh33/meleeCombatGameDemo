using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo
{
    [CreateAssetMenu(fileName = "New State", menuName = "SkillEffects/MoveForward")]
    public class MoveForward : SkillEffect
    {

        public AnimationCurve SpeedGraph;
        public float Speed;
        public bool LockDirection;
        private Vector3 faceDirection;
        public override void OnEnter(StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo)
        {
            faceDirection = animator.transform.forward;

        }
        public override void UpdateEffect(StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo)
        {
            stateEffect.playerControl.MoveForward(faceDirection, Speed, SpeedGraph.Evaluate(animatorStateInfo.normalizedTime));

        }
        public override void OnExit(StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo)
        {

        }
    }
}