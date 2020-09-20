using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {


    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Attack")]
    public class Attack : SkillEffect {
        [Range (0.01f, 1f)]
        public float AttackBeginTime = 0.2f;
        [Range (0.01f, 1f)]
        public float AttackEndTime = 0.6f;

        public AttackType Type = AttackType.MustCollide;
        public int MaxTargetNum = 5;
        public float Range = 2f;
        public float Damage = 1f;
        public float DamageInterval = 1f;
        public float KnockbackForce = 10f;
        public float KnockbackTime = 0.1f;
        public float HitReactDuration = 0.1f;
        public float Stun = 1f;
        public bool IsAttackForward;
        public bool IsAOEAttackTowardsCenter;
        public bool IsAOEAttackAttachToPlayer;
        public bool IsLethalToStunnedEnemy;
        public float AOEAttackCenterOffset = 3.0f;
        public float EnergyGetWhenHit = 0f;
        public float VFXScale = 1f;
        public int PreciselyBlockedFrame = 10;
        public VFXType vfxType = VFXType.Null;
        //public Vector3 AttackPosition;
        //public AOEType AOECenter;
        public List<AttackInfo> FinishedAttacks = new List<AttackInfo> ();

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetBool (TransitionParameter.AttackMelee.ToString (), false);
            stateEffect.CharacterControl.CommandAttack = false;
            //animator.SetInteger (TransitionParameter.CheckCombo.ToString (), 0);
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.AttackInfo);
            AttackInfo atkInfo = obj.GetComponent<AttackInfo> ();
            atkInfo.Init (this, null, stateEffect.CharacterControl);
            obj.SetActive (true);
            AttackManager.Instance.CurrentAttackInfo.Add (atkInfo);
            if (stateEffect.CharacterControl.isPlayerControl)
                VirtualInputManager.Instance.ClearAllInputsInBuffer ();
           

            //stateEffect.CharacterControl.AttackTrigger = true;
            //animator.SetBool (TransitionParameter.ForcedTransition.ToString (), false);
            //Debug.Log ("Enter " + stateInfo.normalizedTime.ToString());

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (!CheckInTransitionBetweenSameState (stateEffect.CharacterControl, animator)) {
                //Debug.Log ("not in transition");
                RegisterAttack (stateEffect, animator, animatorStateInfo);
                DeregisterAttack (stateEffect, animator, animatorStateInfo);
            }
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            //Debug.Log ("Exit " + stateInfo.normalizedTime.ToString ());
            foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                if (info.AttackSkill == this && info.Attacker == stateEffect.CharacterControl) {
                    //info.IsFinished = true;
                    //info.IsRegistered = false;
                    FinishedAttacks.Add (info);
                    //Debug.Log (this.name + " deregistered: " + stateInfo.normalizedTime);

                }
            }
            CleanAttackInfo ();
            /*
            foreach (AttackInfo info in FinishedAttacks) {
                if (AttackManager.Instance.CurrentAttackInfo.Contains (info)) {
                    AttackManager.Instance.CurrentAttackInfo.Remove (info);
                    PoolObject pobj = info.GetComponent<PoolObject> ();
                    PoolManager.Instance.ReturnToPool (pobj);
                    info.Clear ();
                }
            }
            FinishedAttacks.Clear ();
            */
            animator.SetBool (TransitionParameter.Move.ToString (), false);
            //animator.SetInteger (TransitionParameter.CheckCombo.ToString (), 0);

        }
        public void RegisterAttack (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= AttackBeginTime && stateInfo.normalizedTime < AttackEndTime) {
                foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                    if (!info.IsRegistered && info.AttackSkill == this && info.Attacker == stateEffect.CharacterControl) {
                        //Debug.Log (this.name + " registered: " + stateInfo.normalizedTime);
                        //CameraManager.Instance.ShakeCamera(0.2f);
                        info.Register ();
                    }
                }
            }

        }
        public void DeregisterAttack (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= AttackEndTime) {
                bool hasFinishedAttackInfo = false;
                foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                    if (!info.IsFinished && info.IsRegistered && info.AttackSkill == this && info.Attacker == stateEffect.CharacterControl) {
                        //info.IsFinished = true;
                        //info.IsRegistered = false;
                        FinishedAttacks.Add (info);
                        hasFinishedAttackInfo = true;
                        //Debug.Log (this.name + " deregistered: " + stateInfo.normalizedTime);

                    }
                }
                if (hasFinishedAttackInfo)
                    CleanAttackInfo ();

            }

        }

        public void CleanAttackInfo () {
            if (FinishedAttacks.Count > 0) {
                foreach (AttackInfo info in FinishedAttacks) {
                    if (AttackManager.Instance.CurrentAttackInfo.Contains (info)) {
                        AttackManager.Instance.CurrentAttackInfo.Remove (info);
                        PoolObject pobj = info.GetComponent<PoolObject> ();
                        PoolManager.Instance.ReturnToPool (pobj);
                        info.Clear ();
                    }
                }
                FinishedAttacks.Clear ();
            }
        }

    }
}