using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ShakeCamera")]
    public class ShakeCamera : SkillEffect {
        [Range (0.01f, 1f)]
        public float ShakeBeginTime = 0.4f;

        [Range (0.01f, 1f)]
        public float ShakeEndTime = 0.6f;

        public List<CameraShaker> FinishedShakers = new List<CameraShaker> ();

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.CAMERA_SHAKER);
            CameraShaker shaker = obj.GetComponent<CameraShaker> ();
            shaker.Init (this, stateEffect.CharacterControl);
            obj.SetActive (true);
            CameraManager.Instance.CurrentCameraShakers.Add (shaker);

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            RegisterShaker (stateEffect, animator, stateInfo);
            DeregisterShaker (stateEffect, animator, stateInfo);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            foreach (CameraShaker info in CameraManager.Instance.CurrentCameraShakers) {
                if (info.skill == this && info.Caster == stateEffect.CharacterControl) {
                    FinishedShakers.Add (info);
                }
            }
            CleanShakers();

        }
        public void RegisterShaker (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= ShakeBeginTime && stateInfo.normalizedTime < ShakeEndTime) {
                foreach (CameraShaker info in CameraManager.Instance.CurrentCameraShakers) {
                    if (!info.IsRegistered && info.skill == this && info.Caster == stateEffect.CharacterControl) {
                        info.Register ();
                    }
                }
            }

        }
        public void DeregisterShaker (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (stateInfo.normalizedTime >= ShakeEndTime) {
                bool hasFinishedShaker = false;
                foreach (CameraShaker info in CameraManager.Instance.CurrentCameraShakers) {
                    if (!info.IsFinished && info.IsRegistered && info.skill == this && info.Caster == stateEffect.CharacterControl) {
                        FinishedShakers.Add (info);
                        hasFinishedShaker = true;
                    }
                }
                if (hasFinishedShaker)
                    CleanShakers ();
            }

        }

        public void CleanShakers () {
            if (FinishedShakers.Count > 0) {
                foreach (CameraShaker info in FinishedShakers) {
                    if (CameraManager.Instance.CurrentCameraShakers.Contains (info)) {
                        CameraManager.Instance.CurrentCameraShakers.Remove (info);
                        info.Dead ();
                    }
                }
                FinishedShakers.Clear ();
                if (CameraManager.Instance.CurrentCameraShakers.Count == 0)
                    CameraManager.Instance.ResetCamera ();
            }
        }
    }
}