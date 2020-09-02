using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ZoomCamera")]
    public class ZoomOut : SkillEffect {
        [Range (0f, 10f)]
        public float ZoomTiming = 2f;

        public AnimationCurve ZoomSpeedGraph;
        public float ZoomSpeed;


        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime >= ZoomTiming) {
                CameraManager.Instance.ZoomCameraPerFrame(ZoomSpeedGraph.Evaluate(stateInfo.normalizedTime) * ZoomSpeed * Time.deltaTime);
            }
            

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}