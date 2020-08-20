using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum AttackType {
        NULL,
        MUST_COLLIDE,
        AOE,
        PROJECTILE

    }

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Attack")]
    public class Attack : SkillEffect {
        [Range (0.01f, 1f)]
        public float AttackBeginTime = 0.2f;
        [Range (0.01f, 1f)]
        public float AttackEndTime = 0.6f;

        public AttackType attackType = AttackType.MUST_COLLIDE;
        public int MaxTargetNum = 5;
        public float Range = 2f;
        public float Damage = 1f;
        public float KnockbackForce = 10f;
        public float HitReactDuration = 0.1f;

        //[Range (0.01f, 1f)]
        //public float ComboInputStartTime = 0.3f;
        //[Range (0.01f, 1f)]
        //public List<float> ComboInputInterval = new List<float> {0f, 1f};
        //public float ComboInputEndTime = 0.7f;

        //public List<AttackType> AttackParts = new List<AttackPartType> ();
        public List<AttackInfo> FinishedAttacks = new List<AttackInfo> ();

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetBool (TransitionParameter.AttackMelee.ToString (), false);
            //animator.SetInteger (TransitionParameter.CheckCombo.ToString (), 0);
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.ATTACK_INFO);
            AttackInfo atkInfo = obj.GetComponent<AttackInfo> ();
            atkInfo.Init (this, null, stateEffect.CharacterControl);
            obj.SetActive (true);
            AttackManager.Instance.CurrentAttackInfo.Add (atkInfo);
            stateEffect.CharacterControl.AttackTrigger = true;
            //animator.SetBool (TransitionParameter.ForcedTransition.ToString (), false);
            //Debug.Log ("Enter " + stateInfo.normalizedTime.ToString());

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (!CheckInTransitionBetweenSameState(stateEffect.CharacterControl, animator)) {
                //Debug.Log ("not in transition");
                RegisterAttack (stateEffect, animator, animatorStateInfo);
                DeregisterAttack (stateEffect, animator, animatorStateInfo);
            }
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            //Debug.Log ("Exit " + stateInfo.normalizedTime.ToString ());
            foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                if (!info.IsFinished && info.IsRegistered && info.AttackSkill == this) {
                    info.IsFinished = true;
                    info.IsRegistered = false;
                    FinishedAttacks.Add (info);
                    //Debug.Log (this.name + " deregistered: " + stateInfo.normalizedTime);

                }
            }
            foreach (AttackInfo info in FinishedAttacks) {
                if (AttackManager.Instance.CurrentAttackInfo.Contains (info)) {
                    AttackManager.Instance.CurrentAttackInfo.Remove (info);
                    PoolObject pobj = info.GetComponent<PoolObject> ();
                    PoolManager.Instance.ReturnToPool (pobj);
                    info.Clear ();
                }
            }
            FinishedAttacks.Clear ();
            animator.SetBool (TransitionParameter.Move.ToString (), false);
            //animator.SetInteger (TransitionParameter.CheckCombo.ToString (), 0);

        }
        public void RegisterAttack (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= AttackBeginTime && stateInfo.normalizedTime < AttackEndTime) {
                //if (stateInfo.normalizedTime >= AttackBeginTime && stateInfo.normalizedTime < AttackEndTime) {
                //Debug.Log ("about to register");
                //Debug.Log (stateInfo.normalizedTime);
                foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                    if (!info.IsRegistered && info.AttackSkill == this) {
                        Debug.Log (this.name + " registered: " + stateInfo.normalizedTime);
                        info.Register ();
                        //CameraManager.Instance.ShakeCamera(0.2f);
                    }
                }
            }

        }
        public void DeregisterAttack (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= AttackEndTime) {
                foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                    if (!info.IsFinished && info.IsRegistered && info.AttackSkill == this) {
                        info.IsFinished = true;
                        info.IsRegistered = false;
                        FinishedAttacks.Add (info);
                        Debug.Log (this.name + " deregistered: " + stateInfo.normalizedTime);

                    }
                }

            }

        }

    }
}