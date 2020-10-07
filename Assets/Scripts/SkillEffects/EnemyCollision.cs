using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/EnemyCollision")]
    public class EnemyCollision : SkillEffect {
        public float collisionRange = 1.0f;
        public bool onlyCheckLockedTarget;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            animator.SetBool (TransitionParameter.EnemyCollision.ToString (), false);
            if (CheckCollision(stateEffect, animator, stateInfo))
                animator.SetBool(TransitionParameter.EnemyCollision.ToString(), true);
          
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (CheckCollision(stateEffect, animator, stateInfo))
                animator.SetBool(TransitionParameter.EnemyCollision.ToString(), true);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetBool (TransitionParameter.EnemyCollision.ToString (), false);

        }

        public bool CheckCollision(StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo)
        {
            if(onlyCheckLockedTarget)
            {
                CharacterControl enemy = stateEffect.CharacterControl.CharacterData.FormerAttackTarget;
                if(enemy != null)
                {
                    Vector3 DistVec = enemy.gameObject.transform.position - stateEffect.CharacterControl.gameObject.transform.position;
                    DistVec.y = 0f;
                    if(DistVec.magnitude <= collisionRange)
                        return true;

                }

            }
            else
            {
                // not complete
            }
            return false;

        }

    }
}