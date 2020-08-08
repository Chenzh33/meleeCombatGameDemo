using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class DamageDetector : MonoBehaviour {
        CharacterControl control;

        void Awake () {
            control = this.GetComponent<CharacterControl> ();

        }

        void Start () {

        }

        void FixedUpdate () {
            if (AttackManager.Instance.CurrentAttackInfo.Count > 0)
                CheckAttack ();

        }

        private void CheckAttack () {
            foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                if (!info.IsRegistered || info.IsFinished)
                    continue;
                if (info.Attacker == control)
                    continue;
                Debug.Log ("check attack");
                if (IsCollided (info)) {
                    info.IsFinished = true;
                    ProcessDamage ();
                }

            }

        }
        private bool IsCollided (AttackInfo info) {
            //List<TriggerDetector> triggers = control.GetAllTriggers ();
            TriggerDetector trigger = control.GetTriggerDetector ();
            foreach (Collider c1 in trigger.CollidingParts) {
                foreach (Collider c2 in info.Attacker.GetAttackingPart ()) {
                    if (c2 == c1)
                        return true;

                }

            }

            return false;

        }
        private void ProcessDamage () {
            Debug.Log ("HIT !!!");
            control.Dead();
            CameraManager.Instance.ShakeCamera(0.25f);
        }
    }
}