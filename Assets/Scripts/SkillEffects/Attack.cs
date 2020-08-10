using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum AttackPartType {
        MELEE_WEAPON
    }

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Attack")]
    public class Attack : SkillEffect {
        [Range (0.01f, 1f)]
        public float AttackBeginTime = 0.2f;
        [Range (0.01f, 1f)]
        public float AttackEndTime = 0.6f;

        [Range (0.01f, 1f)]
        public float ComboInputStartTime = 0.3f;
        [Range (0.01f, 2f)]
        public float ComboInputEndTime = 1.5f;

        public List<AttackPartType> AttackParts = new List<AttackPartType> ();
        public List<AttackInfo> FinishedAttacks = new List<AttackInfo> ();

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            animator.SetBool (TransitionParameter.AttackMelee.ToString (), false);
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.ATTACK_INFO);
            AttackInfo atkInfo = obj.GetComponent<AttackInfo> ();
            atkInfo.Init (this, stateEffect.CharacterControl);
            obj.SetActive (true);
            AttackManager.Instance.CurrentAttackInfo.Add (atkInfo);
            //animator.SetBool (TransitionParameter.ForcedTransition.ToString (), false);

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            RegisterAttack (stateEffect, animator, animatorStateInfo);
            DeregisterAttack (stateEffect, animator, animatorStateInfo);
            CheckCombo(stateEffect, animator, animatorStateInfo);

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            foreach (AttackInfo info in FinishedAttacks) {
                if (AttackManager.Instance.CurrentAttackInfo.Contains (info)) {
                    AttackManager.Instance.CurrentAttackInfo.Remove (info);
                    PoolObject pobj = info.GetComponent<PoolObject> ();
                    PoolManager.Instance.ReturnToPool (pobj);
                }
            }
            FinishedAttacks.Clear ();

        }
        public void RegisterAttack (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= AttackBeginTime && stateInfo.normalizedTime < AttackEndTime) {
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
                    if (!info.IsFinished && info.AttackSkill == this) {
                        info.IsFinished = true;
                        info.IsRegistered = false;
                        FinishedAttacks.Add (info);
                        Debug.Log (this.name + " deregistered: " + stateInfo.normalizedTime);

                    }
                }

            }

        }

        public void CheckCombo (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime >= ComboInputStartTime && stateInfo.normalizedTime < ComboInputEndTime) {
                if (stateEffect.CharacterControl.inputDataTop.KeysState[(int) InputKeyStateType.KEY_MELEE_ATTACK_DOWN]) {
                    animator.SetBool (TransitionParameter.AttackMelee.ToString (), true);
                }
            }

        }
    }
}