using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/DirectDamage")]
    public class DirectDamage : SkillEffect {
        [Range (0f, 1f)]
        public float DamageTiming = 0.2f;

        public float Damage = 1f;
        public float KnockbackForce = 10f;
        public float HitReactDuration = 0.1f;
        public float Stun = 1f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            stateEffect.CharacterControl.CharacterData.GrapplingTarget.gameObject.transform.parent = null;
            CharacterControl Target = stateEffect.CharacterControl.CharacterData.GrapplingTarget;
            Target.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1.0f);
            //Target.Animator.Play ("Idle");
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime > DamageTiming && animator.GetBool (TransitionParameter.GrapplingHit.ToString ())) {
                if (stateEffect.CharacterControl.CharacterData.GrapplingTarget != null && !stateEffect.CharacterControl.CharacterData.GrapplingTarget.CharacterData.IsDead) {
                    stateEffect.CharacterControl.Animator.SetBool (TransitionParameter.GrapplingHit.ToString (), false);
                    CharacterControl Target = stateEffect.CharacterControl.CharacterData.GrapplingTarget;
                    //Target.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1.0f);
                    //Target.Animator.Play ("Idle");

                    Target.TakeDamage (Damage);
                    if (!Target.CharacterData.IsStunned)
                        Target.TakeStun (Stun);
                    Vector3 dirVector = Target.transform.position - stateEffect.CharacterControl.transform.position;
                    Vector3 hitVector = (new Vector3 (dirVector.x, 0, dirVector.z)).normalized;
                    Target.TakeKnockback (KnockbackForce * hitVector, HitReactDuration);
                    Target.CharacterData.FormerAttackTarget = null;
                    Target.FaceTarget = -hitVector;
                    Target.TurnToTarget (0f, 0f);
                    //int randomIndex = Random.Range (0, 3) + 1;
                    //Target.Animator.Play ("HitReact" + randomIndex.ToString (), 0, 0f);
                }
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            stateEffect.CharacterControl.Animator.SetBool (TransitionParameter.GrapplingHit.ToString (), false);
            stateEffect.CharacterControl.CharacterData.GrapplingTarget.CharacterData.IsGrappled = false;
        }
        public void RegisterGrappler (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }
        public void DeregisterGrappler (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }

    }
}