using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ShakeCamera")]
    public class ShakeCamera : SkillEffect {
        [Range (0.01f, 1f)]
        public float ShakeBeginTime = 0.4f;

        [Range (0.01f, 1f)]
        public float ShakeEndTime = 0.6f;
        private bool IsShaking;


        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            IsShaking = false;
           

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime >= ShakeBeginTime && stateInfo.normalizedTime < ShakeEndTime)
            {
                if(!IsShaking)
                {
                    CameraManager.Instance.ResetTrigger();
                    CameraManager.Instance.ShakeCamera((ShakeEndTime - ShakeBeginTime) * stateInfo.length);
                    IsShaking = true;
                }
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            IsShaking = false;
        }
     
    }
}