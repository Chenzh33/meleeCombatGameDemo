using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/TurnSomeAngle")]
    public class TurnSomeAngle : SkillEffect {

        public float TurnAngle = 1080f;
        //public AnimationCurve AngularSpeedGraph;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            float angle = TurnAngle * Time.deltaTime / stateInfo.length;
            animator.transform.root.rotation *= Quaternion.AngleAxis (angle, Vector3.up);
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            float angle = TurnAngle * Time.deltaTime / stateInfo.length;
            animator.transform.root.rotation *= Quaternion.AngleAxis (angle, Vector3.up);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}