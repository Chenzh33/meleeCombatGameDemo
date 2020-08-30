using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/BoxCast")]
    public class BoxCast : SkillEffect {
        public float BoxHalfSize = 1.5f;
        public float HitDistance = 5f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

           }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            RaycastHit rayCastHit;
            int layerMask = 1 << 8;
            bool hit = Physics.BoxCast(stateEffect.CharacterControl.gameObject.transform.position, BoxHalfSize * new Vector3(1.0f, 1.0f, 0.2f),
            stateEffect.CharacterControl.gameObject.transform.forward,
            out rayCastHit,
            stateEffect.CharacterControl.gameObject.transform.rotation,
            HitDistance, layerMask);
            if(hit)
            {
                Debug.Log("box hit!");
                animator.SetBool("BoxCast", true);

            }
            else
                animator.SetBool("BoxCast", false);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetBool("BoxCast", false);
        }
      

    }
}