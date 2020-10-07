using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Displacement")]
    public class Displacement : SkillEffect {
        public AnimationCurve speedGraph;
        public bool ignoreSpeedMultiplier;
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
            RaycastHit hit;
            int layermask = 1 << 10;
            //Gizmos.DrawWireCube(control.transform.position + 1f * moveDirection, new Vector3(1.0f,1.0f,0.2f));
            if (!Physics.BoxCast (control.transform.position, new Vector3(0.2f,1.0f,0.2f), moveDirection, out hit, control.transform.rotation, 1f, layermask)) {
                Vector3 deltaMoveAmount = moveDirection * speed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime;
                if (!ignoreSpeedMultiplier)
                    deltaMoveAmount = deltaMoveAmount * animator.GetFloat(TransitionParameter.SpeedMultiplier.ToString());
                control.transform.Translate (deltaMoveAmount, Space.World);
            }
        }
    }
}