using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/MovementY")]
    public class MovementY : SkillEffect {
        public AnimationCurve speedGraph;
        public float speed = 6.0f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            MoveY (stateEffect.CharacterControl, animator, animatorStateInfo);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            stateEffect.CharacterControl.gameObject.transform.position = new Vector3(stateEffect.CharacterControl.gameObject.transform.position.x, 0f, stateEffect.CharacterControl.transform.position.z);

        }

        public void MoveY (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {
            Vector3 deltaMoveAmount = Vector3.up * animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * speed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime;
            control.transform.Translate (deltaMoveAmount, Space.World);

        }
    }
}