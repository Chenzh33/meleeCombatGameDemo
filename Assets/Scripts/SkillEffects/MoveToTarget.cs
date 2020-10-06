using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {
    public enum TargetType {
        FormerTarget,
        FormerTargetProjectileSpawnPoint
    }

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/MoveToTarget")]
    public class MoveToTarget : SkillEffect {
        public AnimationCurve speedGraph;
        public float curveArea;
        public float maxSpeed;
        public TargetType Type;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            //curveArea = AreaUnderCurve (speedGraph, 1.0f, 1.0f);
            CharacterControl control = stateEffect.CharacterControl;
            if (control.CharacterData.FormerAttackTarget != null) {
                Vector3 currDistVec = Vector3.zero;
                if (Type == TargetType.FormerTarget)
                    currDistVec = control.CharacterData.FormerAttackTarget.gameObject.transform.position - control.gameObject.transform.position;
                else if (Type == TargetType.FormerTargetProjectileSpawnPoint)
                    currDistVec = control.CharacterData.FormerAttackTarget.GetProjectileSpawnPoint ().gameObject.transform.position - control.gameObject.transform.position;
                currDistVec.y = 0f;
                float speed = currDistVec.magnitude / (stateInfo.length * curveArea);
                speed = Mathf.Min(speed, maxSpeed);
                control.CharacterData.CurrentDisplacementSpeed = speed;
            } else
                control.CharacterData.CurrentDisplacementSpeed = maxSpeed;

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

            MoveForward (stateEffect.CharacterControl, animator, animatorStateInfo);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            stateEffect.CharacterControl.CharacterData.CurrentDisplacementSpeed = 0f;
        }

        public void MoveForward (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {

            Vector3 moveDirection = control.FaceTarget;
            Vector3 deltaMoveAmount = moveDirection * animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * control.CharacterData.CurrentDisplacementSpeed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime;

            RaycastHit hit;
            int layermask = 1 << 10;
            if (!Physics.BoxCast (control.transform.position, new Vector3 (0.2f, 1.0f, 0.2f), moveDirection, out hit, control.transform.rotation, 1f, layermask)) {
                control.transform.Translate (deltaMoveAmount, Space.World);
            }

        }

    }
}