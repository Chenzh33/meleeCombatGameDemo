using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Displacement")]
    public class Displacement : SkillEffect {
        public AnimationCurve speedGraph;
        public float speed = 6.0f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            SetPosition (stateEffect.CharacterControl, animator, animatorStateInfo);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }

        public void SetPosition (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {
            Vector3 moveDirection = control.FaceTarget;
            Vector3 deltaMoveAmount = moveDirection * animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * speed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime;
            control.transform.Translate(deltaMoveAmount, Space.World);

        }
    }
}