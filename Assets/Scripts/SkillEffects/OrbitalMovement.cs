using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/OrbitalMovement")]
    public class OrbitalMovement : SkillEffect {
       
        public bool IsCenterFormerTarget;
        public int direction; // -1,0,1, 0 means random {-1,1}
        public AnimationCurve speedGraph;
        public float speed = 6.0f;
        public float smooth = 10.0f;
        public float decayRange = 4.0f;
       


        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            CharacterControl control = stateEffect.CharacterControl;
            Vector3 FaceTarget = animator.transform.root.forward;

            float distDecayFactor = 1.0f;
            if(control.CharacterData.FormerAttackTarget != null)
            {
                Vector3 dir = control.CharacterData.FormerAttackTarget.gameObject.transform.position - control.gameObject.transform.position;
                dir.y = 0f;
                if(dir.magnitude <= decayRange)
                {
                    distDecayFactor = Mathf.Sqrt(dir.magnitude / decayRange);

                }
                FaceTarget = dir.normalized;
            }
            Quaternion rotation = Quaternion.LookRotation(FaceTarget, Vector3.up);
            animator.transform.root.rotation = Quaternion.Slerp(animator.transform.root.rotation, rotation, Time.deltaTime * smooth);

            float deltaAngle = 90f;
            if (direction == 0)
                deltaAngle = deltaAngle * ((Random.Range(0, 2) * 2) - 1);
            else
                deltaAngle = deltaAngle * direction;

            Vector3 moveDirection = Quaternion.AngleAxis(deltaAngle, Vector3.up) * FaceTarget;

            Vector3 deltaMoveAmount = moveDirection * animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * speed * speedGraph.Evaluate (stateInfo.normalizedTime) * Time.deltaTime * distDecayFactor;
            control.CharacterController.Move (deltaMoveAmount);

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
         
        }

    }
}