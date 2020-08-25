using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum GrapplerType {
        STOP_ANIMATION,
        DEAL_DAMAGE

    }

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/GrapplingSkill")]
    public class GrapplingSkill : SkillEffect {
        [Range (0f, 1f)]
        public float GrapplingBeginTime = 0.2f;
        [Range (0f, 1f)]
        public float GrapplingEndTime = 0.6f;

        public GrapplerType Type;
        public float Range = 1f;
        public float Damage = 1f;
        public float FreezeStartTiming = 0.1f;
        public List<Grappler> FinishedGrapplers = new List<Grappler> ();

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetBool (TransitionParameter.AttackExecute.ToString (), false);
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.GRAPPLER);
            Grappler grappler = obj.GetComponent<Grappler> ();
            grappler.Init (this, stateEffect.CharacterControl);
            obj.SetActive (true);
            AttackManager.Instance.CurrentGrappler.Add (grappler);
            if (stateEffect.CharacterControl.gameObject.GetComponent<ManualInput>() != null)
                stateEffect.CharacterControl.ExecuteTrigger = true;
            //animator.SetBool (TransitionParameter.ForcedTransition.ToString (), false);
            //Debug.Log ("Enter " + stateInfo.normalizedTime.ToString());

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            RegisterGrappler (stateEffect, animator, animatorStateInfo);
            DeregisterGrappler (stateEffect, animator, animatorStateInfo);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            foreach (Grappler info in AttackManager.Instance.CurrentGrappler) {
                if (!info.IsFinished && info.IsRegistered && info.Skill == this) {
                    info.IsFinished = true;
                    info.IsRegistered = false;
                    FinishedGrapplers.Add (info);
                    //Debug.Log (this.name + " deregistered: " + stateInfo.normalizedTime);

                }
            }
            foreach (Grappler info in FinishedGrapplers) {
                if (AttackManager.Instance.CurrentGrappler.Contains (info)) {
                    AttackManager.Instance.CurrentGrappler.Remove (info);
                    PoolObject pobj = info.GetComponent<PoolObject> ();
                    PoolManager.Instance.ReturnToPool (pobj);
                    info.Clear ();
                }
            }
            FinishedGrapplers.Clear ();
            animator.SetBool (TransitionParameter.Move.ToString (), false);
            //animator.SetInteger (TransitionParameter.CheckCombo.ToString (), 0);

        }
        public void RegisterGrappler (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= GrapplingBeginTime && stateInfo.normalizedTime < GrapplingEndTime) {
                foreach (Grappler info in AttackManager.Instance.CurrentGrappler) {
                    if (!info.IsRegistered && info.Skill == this) {
                        Debug.Log (this.name + " registered: " + stateInfo.normalizedTime);
                        info.Register ();
                        //CameraManager.Instance.ShakeCamera(0.2f);
                    }
                }
            }

        }
        public void DeregisterGrappler (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= GrapplingEndTime) {
                foreach (Grappler info in AttackManager.Instance.CurrentGrappler) {
                    //if (!info.IsFinished && info.IsRegistered && info.Skill == this) {
                    if (info.Target == null && info.IsRegistered && info.Skill == this) {
                        info.IsFinished = true;
                        info.IsRegistered = false;
                        FinishedGrapplers.Add (info);
                        Debug.Log (this.name + " deregistered (missed) : " + stateInfo.normalizedTime);

                    }
                }

            }

        }

    }
}