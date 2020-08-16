using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class DamageDetector : MonoBehaviour {
        CharacterControl control;
        public float HitReactDuration = 0.1f;

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
                if (info.Targets.Contains (control))
                    continue;
                if (info.Attacker == control)
                    continue;
                if (info.CurrentTargetNum >= info.MaxTargetNum) {
                    info.IsFinished = true;
                    continue;
                }
                //Debug.Log ("check attack");
                if (info.Type == AttackType.MUST_COLLIDE) {
                    if (IsCollided (info)) {
                        //info.IsFinished = true;
                        ProcessDamage (info);
                    }

                } else if (info.Type == AttackType.AOE) {
                    if (IsInRange (info)) {
                        //info.IsFinished = true;
                        ProcessDamage (info);

                    }
                } else if (info.Type == AttackType.PROJECTILE) {
                    if (IsInProjectileRange (info)) {
                        ProcessDamage (info);
                    }

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

        private bool IsInProjectileRange (AttackInfo info) {
            float distance = (this.gameObject.transform.position - info.ProjectileObject.gameObject.transform.position).magnitude;
            if (distance <= info.Range) {
                return true;
            }
            return false;

        }
        private bool IsInRange (AttackInfo info) {
            return true;
        }
        private void ProcessDamage (AttackInfo info) {
            Debug.Log ("HIT !!!");
            if (!info.Targets.Contains (control)) {
                info.Targets.Add (control);
                info.CurrentTargetNum++;
                Vector3 dirVector = gameObject.transform.position - info.Attacker.gameObject.transform.position;
                Vector3 hitVector = (new Vector3(dirVector.x, 0, dirVector.y)).normalized;
                Debug.Log(hitVector);
                control.TakeDamage (info.Damage);
                control.TakeKnockback (info.KnockbackForce * hitVector, HitReactDuration);
                int randomIndex = Random.Range (0, 3) + 1;
                control.Animator.Play ("HitReact" + randomIndex.ToString(), 0, 0f);
                CameraManager.Instance.ShakeCamera (0.25f);
            }
            //control.Dead ();
        }
    }
}