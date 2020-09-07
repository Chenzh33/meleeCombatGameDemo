using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class DamageDetector : MonoBehaviour {
        CharacterControl control;
        //public float HitReactDuration = 0.1f;

        void Awake () {
            control = this.GetComponent<CharacterControl> ();
        }

        void Start () {

        }

        void FixedUpdate () {
            if (AttackManager.Instance.CurrentAttackInfo.Count > 0)
                CheckAttack ();
            if (AttackManager.Instance.CurrentGrappler.Count > 0)
                CheckGrappler ();

        }

        private void CheckAttack () {
            foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                if (!info.IsRegistered || info.IsFinished)
                    continue;
                if (info.Targets.Contains (control) || control.CharacterData.IsInvincible || control.CharacterData.IsDead)
                    continue;
                if (info.Attacker == control || info.Attacker.CharacterData.Team == control.CharacterData.Team)
                    continue;
                if (info.CurrentTargetNum >= info.MaxTargetNum) {
                    info.IsFinished = true;
                    continue;
                }
                //Debug.Log ("check attack");
                if (info.Type == AttackType.MUST_COLLIDE) {
                    if (IsCollidedWithAttackParts (info)) {
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
        private bool IsCollidedWithAttackParts (AttackInfo info) {
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
        private bool IsCollidedWithAttackPoint (Grappler info) {
            /*
            TriggerDetector trigger = control.GetTriggerDetector ();
            foreach (Collider c in trigger.CollidingParts) {
                if (info.Attacker.GetAttackPoint () == c)
                    return true;
            }
            return false;
            */

            /*
            int layerMask = 1 << 9;
            RaycastHit hit;
            //if (Physics.Raycast (info.Attacker.gameObject.transform.position, info.Attacker.gameObject.transform.forward, out hit, Mathf.Infinity, layerMask)) {
            if (Physics.BoxCast(info.Attacker.GetAttackPoint ().gameObject.transform.position, 
            info.Range * new Vector3(1.0f,1.0f,0.2f), info.Attacker.gameObject.transform.forward, 
            out hit, info.Attacker.gameObject.transform.rotation, info.Range, layerMask)) {
                //Debug.DrawRay (info.Attacker.transform.position, info.Attacker.transform.forward * hit.distance, Color.yellow, info.Range);
                //if (hit.distance <= info.Range) {
                Debug.Log("Did Hit");
                return true;
                //}
            }
            else
            {
                Debug.DrawRay(info.Attacker.transform.position, info.Attacker.transform.forward * 100, Color.white);
                Debug.Log("Did not Hit");
                return false;
            }
            */

            Vector3 dist = control.gameObject.transform.position - info.Attacker.GetAttackPoint ().gameObject.transform.position;
            dist.y = 0f;
            if (dist.magnitude <= info.Range)
                return true;
            else
                return false;

        }

        private bool IsInProjectileRange (AttackInfo info) {
            Vector3 distVec = this.gameObject.transform.position - info.ProjectileObject.gameObject.transform.position;
            float dist = new Vector3 (distVec.x, 0f, distVec.z).magnitude;
            if (dist <= info.Range) {
                return true;
            }
            return false;

        }
        private bool IsInRange (AttackInfo info) {
            //Vector3 distVec = this.gameObject.transform.position - info.Attacker.GetAttackPoint ().gameObject.transform.position;
            //Vector3 distVec = this.gameObject.transform.position - info.Attacker.GetProjectileSpawnPoint().gameObject.transform.position;
            //Debug.Log(info.AttackCenter);
            Vector3 distVec = this.gameObject.transform.position - info.gameObject.transform.position;
            float dist = new Vector3 (distVec.x, 0f, distVec.z).magnitude;
            if (dist <= info.Range) {
                return true;
            }
            return false;
        }
        private void ProcessDamage (AttackInfo info) {
            //Debug.Log ("HIT !!!");
            info.Targets.Add (control);
            info.CurrentTargetNum++;
            Vector3 dirVector = gameObject.transform.position - info.Attacker.gameObject.transform.position;
            Vector3 hitVector = (new Vector3 (dirVector.x, 0, dirVector.z)).normalized;
            if (info.IsAttackForward) {
                hitVector = info.Attacker.transform.forward;
                hitVector.y = 0f;
            }
            if (info.IsAOEAttackTowardsCenter && info.Type == AttackType.AOE) {
                hitVector = info.gameObject.transform.position - gameObject.transform.position;
                hitVector.y = 0f;
                //Debug.Log("AOE hit! " + Time.time.ToString());
            }

            control.FaceTarget = -hitVector;
            control.TurnToTarget (0f, 0f);

            //Debug.Log(hitVector);
            //Debug.DrawRay(gameObject.transform.position, hitVector * 5f, Color.red, 0.5f);

            control.TakeDamage (info.Damage);
            if (!control.CharacterData.IsStunned)
                control.TakeStun (info.Stun);
            control.TakeKnockback (info.KnockbackForce * hitVector, info.HitReactDuration);
            control.CharacterData.FormerAttackTarget = null;

            //CameraManager.Instance.ShakeCamera (info.HitReactDuration);
            //control.Dead ();
        }

        private void CheckGrappler () {
            foreach (Grappler info in AttackManager.Instance.CurrentGrappler) {
                if (!info.IsRegistered || info.IsFinished)
                    continue;
                if (info.Target != null || control.CharacterData.IsInvincible)
                    continue;
                if (control.CharacterData.IsDead || control.CharacterData.IsGrappled)
                    continue;
                if (info.Attacker == control || info.Attacker.CharacterData.Team == control.CharacterData.Team)
                    continue;
                if (info.Type == GrapplerType.STOP_ANIMATION) {
                    Debug.Log ("Test Collision !!!");
                    if (IsCollidedWithAttackPoint (info)) {
                        Debug.Log ("Collision occurs !!!");
                        ProcessGrappling (info);

                    }

                }
            }
        }
        private void ProcessGrappling (Grappler info) {
            //if (info.Target == null) {
            info.Target = control;
            Debug.Log ("Grappler HIT !!!");
            control.CharacterData.IsGrappled = true;
            control.HitReactionAndFreeze (info.FreezeStartTiming);

            Vector3 dirVector = control.gameObject.transform.position - info.Attacker.gameObject.transform.position;
            Vector3 hitVector = (new Vector3 (dirVector.x, 0, dirVector.z)).normalized;
            control.FaceTarget = -hitVector;
            control.TurnToTarget (0f, 0f);

            info.Attacker.CharacterController.Move (info.Attacker.gameObject.transform.forward * 0.6f);
            Vector3 AttackerFront = info.Attacker.gameObject.transform.forward * 1.2f;
            AttackerFront.y = 0f;
            Vector3 diff = control.gameObject.transform.position - info.Attacker.gameObject.transform.position;
            diff.y = 0f;
            Vector3 dir = AttackerFront - diff;
            control.CharacterController.Move (dir);

            //info.Attacker.gameObject.transform.position = info.Attacker.gameObject.transform.position + info.Attacker.gameObject.transform.forward * 0.6f;
            //Vector3 AttackerFront = info.Attacker.gameObject.transform.position + info.Attacker.gameObject.transform.forward * 1.2f;
            //control.gameObject.transform.position = new Vector3 (AttackerFront.x, control.gameObject.transform.position.y, AttackerFront.z);
            //Debug.DrawRay(info.Attacker.gameObject.transform.position, info.Attacker.gameObject.transform.forward * 10f, Color.red, 1.0f);
            control.gameObject.transform.parent = info.Attacker.Animator.gameObject.transform;
            //control.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0f);
            info.GrapplingHit ();

            //}
            //control.Dead ();
        }

    }
}