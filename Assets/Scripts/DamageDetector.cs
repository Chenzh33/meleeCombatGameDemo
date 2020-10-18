using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class DamageDetector : MonoBehaviour {
        CharacterControl control;
        List<AttackInfo> extraAttackInfo = new List<AttackInfo> ();
        //public float HitReactDuration = 0.1f;

        void Awake () {
            control = this.GetComponent<CharacterControl> ();
        }

        void Start () {

        }

        void FixedUpdate () {
            if (AttackManager.Instance.CurrentAttackInfo.Count > 0) {
                CheckReflection ();
                CheckAttack ();
            }
            if (AttackManager.Instance.CurrentGrappler.Count > 0)
                CheckGrappler ();

        }

        private void CheckAttack () {
            //List<AttackInfo> currentAttackInfoCache = AttackManager.Instance.CurrentAttackInfo;
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
                if (info.Type == AttackType.MustCollide) {
                    if (IsCollidedWithAttackParts (info)) {
                        //info.IsFinished = true;
                        ProcessDamage (info);
                    }

                } else if (info.Type == AttackType.AOE) {
                    if (IsInRange (info)) {
                        //info.IsFinished = true;
                        ProcessDamage (info);

                    }
                } else if (info.Type == AttackType.Projectile) {
                    if (IsInRange (info)) {
                        ProcessDamage (info);
                    }

                }
            }
            if (extraAttackInfo.Count > 0) {

                foreach (AttackInfo info in extraAttackInfo) {
                    AttackManager.Instance.CurrentAttackInfo.Add (info);
                }
                extraAttackInfo.Clear ();
            }

        }

        private void CheckReflection () {
            //List<AttackInfo> currentAttackInfoCache = AttackManager.Instance.CurrentAttackInfo;
            foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                if (!info.IsRegistered || info.IsFinished)
                    continue;
                if (info.Attacker == control && info.CanReflectProjectile) {
                    foreach (AttackInfo enemyInfo in AttackManager.Instance.CurrentAttackInfo) {
                        if (enemyInfo.Type != AttackType.Projectile || !enemyInfo.CanBeReflected || !enemyInfo.IsRegistered || enemyInfo.IsFinished)
                            continue;
                        if (enemyInfo.Attacker.CharacterData.Team != info.Attacker.CharacterData.Team)
                            if (IsInReflectionRange (enemyInfo, info)) {
                                Vector3 dir = enemyInfo.Attacker.gameObject.transform.position - info.ProjectileObject.gameObject.transform.position;
                                dir.y = 0f;
                                ReflectProjectile (enemyInfo, enemyInfo.gameObject.transform.position, dir.normalized);
                            }
                    }

                }

            }
            if (extraAttackInfo.Count > 0) {

                foreach (AttackInfo info in extraAttackInfo) {
                    AttackManager.Instance.CurrentAttackInfo.Add (info);
                }
                extraAttackInfo.Clear ();
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

            if (info.Mode == GrapplerTargetChosingMode.LockedTargetOnly) {
                if (control != info.Attacker.CharacterData.FormerAttackTarget)
                    return false;
            }
            //Debug.Log("Check Grappling in range!");
            Vector3 dist = control.gameObject.transform.position - info.Attacker.GetProjectileSpawnPoint ().gameObject.transform.position;
            dist.y = 0f;
            if (dist.magnitude <= info.Range)
                return true;
            else
                return false;

        }

        /*
                private bool IsInProjectileRange (AttackInfo info) {
                    Vector3 distVec = this.gameObject.transform.position - info.ProjectileObject.gameObject.transform.position;
                    float dist = new Vector3 (distVec.x, 0f, distVec.z).magnitude;
                    if (dist <= info.Range) {
                        return true;
                    }
                    return false;

                }
                */
        private bool IsInRange (AttackInfo info) {
            //Vector3 distVec = this.gameObject.transform.position - info.Attacker.GetAttackPoint ().gameObject.transform.position;
            //Vector3 distVec = this.gameObject.transform.position - info.Attacker.GetProjectileSpawnPoint().gameObject.transform.position;
            //Debug.Log(info.AttackCenter);
            Vector3 distVec = this.gameObject.transform.position - info.gameObject.transform.position;
            float dist = new Vector3 (distVec.x, 0f, distVec.z).magnitude;
            float range = info.Range;
            if (info.IsAOERangeChangeWithScaling) {
                range = range * info.ProjectileObject.GetCurrentScale ();

            }
            if (dist <= range) {
                return true;
            }
            return false;
        }
        private bool IsInReflectionRange (AttackInfo enemyInfo, AttackInfo info) {
            //Vector3 distVec = this.gameObject.transform.position - info.Attacker.GetAttackPoint ().gameObject.transform.position;
            //Vector3 distVec = this.gameObject.transform.position - info.Attacker.GetProjectileSpawnPoint().gameObject.transform.position;
            //Debug.Log(info.AttackCenter);
            Vector3 distVec = info.ProjectileObject.gameObject.transform.position - enemyInfo.gameObject.transform.position;

            float dist = new Vector3 (distVec.x, 0f, distVec.z).magnitude;
            float range = info.Range;
            if (info.IsAOERangeChangeWithScaling) {
                range = range * info.ProjectileObject.GetCurrentScale ();

            }
            if (dist <= range) {
                Debug.Log (range);
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
                if (info.IsAOEAttackAttachToPlayer)
                    hitVector = info.Attacker.transform.forward;
                else
                    hitVector = info.gameObject.transform.forward;
                hitVector.y = 0f;
            }
            if (info.IsAOEAttackTowardsCenter && info.Type == AttackType.AOE) {
                hitVector = info.gameObject.transform.position - gameObject.transform.position;
                hitVector.y = 0f;
                //Debug.Log("AOE hit! " + Time.time.ToString());
            }
            if (info.IsAOEAttackTowardsCenter && info.Type == AttackType.Projectile) {
                hitVector = info.gameObject.transform.position - gameObject.transform.position;
                hitVector.y = 0f;
                //Debug.Log("AOE hit! " + Time.time.ToString());
            }
            control.FaceTarget = -hitVector;
            control.TurnToTarget (0f, 0f);

            if (info.FreezeFrames > 0) {
                control.FreezeForFrames (info.FreezeFrames);
                info.Attacker.FreezeForFrames (info.FreezeFrames);

            }

            //Debug.Log(hitVector);
            //Debug.DrawRay(gameObject.transform.position, hitVector * 5f, Color.red, 0.5f);

            bool CanBeBlocked = (control.CharacterData.IsGuarding && control.CharacterData.BlockCount >= info.Damage);
            bool PreciselyBlocked = (control.CharacterData.IsGuarding && info.PreciselyBlockedFrame > control.CharacterData.FirstFramesOfBlock && control.CharacterData.FirstFramesOfBlock > 0);
            if (info.AttackSkill != null) {
                if (!control.CharacterData.IsStunned && !CanBeBlocked)
                    control.TakeStun (info.Stun, info.HitReactDuration, info.AttackSkill);
                else if (info.PreciselyBlockedFrame <= control.CharacterData.FirstFramesOfBlock)
                    control.TakeStun (info.Stun * control.CharacterData.GuardStunReduction, info.HitReactDuration, info.AttackSkill);

                if (control.CharacterData.IsStunned && info.IsLethalToStunnedEnemy)
                    control.TakeDamage (control.CharacterData.HP, info.AttackSkill);
                else if (!CanBeBlocked && !PreciselyBlocked)
                    control.TakeDamage (info.Damage, info.AttackSkill);
                else if (info.PreciselyBlockedFrame <= control.CharacterData.FirstFramesOfBlock)
                    control.TakeDamage (info.Damage * control.CharacterData.GuardDamageReduction, info.AttackSkill);

            } else if (info.ProjectileSkill != null) {
                if (!control.CharacterData.IsStunned && !CanBeBlocked)
                    control.TakeStun (info.Stun, info.HitReactDuration, info.ProjectileSkill);
                else if (info.PreciselyBlockedFrame <= control.CharacterData.FirstFramesOfBlock)
                    control.TakeStun (info.Stun * control.CharacterData.GuardStunReduction, info.HitReactDuration, info.ProjectileSkill);

                if (!CanBeBlocked && !PreciselyBlocked)
                    control.TakeDamage (info.Damage, info.ProjectileSkill);
                else if (info.PreciselyBlockedFrame <= control.CharacterData.FirstFramesOfBlock)
                    control.TakeDamage (info.Damage * control.CharacterData.GuardDamageReduction, info.ProjectileSkill);
            }

            if (!control.CharacterData.IsSuperArmour) {

                if (!CanBeBlocked && !PreciselyBlocked)
                    control.TakeKnockback (info.KnockbackForce * hitVector, info.KnockbackTime);
                else if (info.PreciselyBlockedFrame <= control.CharacterData.FirstFramesOfBlock)
                    control.TakeKnockback (info.KnockbackForce * hitVector * control.CharacterData.GuardKnockbackReduction, info.KnockbackTime);
            }
            control.CharacterData.FormerAttackTarget = null;

            if (control.CharacterData.IsGuarding) {
                if (info.AttackSkill != null && info.AttackSkill.Type == AttackType.MustCollide) {
                    if (CanBeBlocked && info.PreciselyBlockedFrame <= control.CharacterData.FirstFramesOfBlock)
                        control.Animator.SetTrigger (TransitionParameter.GetHitOnGuard.ToString ());
                    else if (PreciselyBlocked) {
                        control.Animator.SetTrigger (TransitionParameter.GetHitOnGuardPrecisely.ToString ());
                        info.Attacker.Animator.Play ("GetCountered");
                        info.Attacker.TakeKnockback ((-30f) * hitVector, 0.1f);
                        //info.Attacker.TakeStun (5f, 1.0f, null);
                        info.Attacker.TakeStun (2.0f * info.Stun, 1.0f, null);
                        info.Attacker.CharacterData.GetHitTime = 0.7f;
                        /*
                        AIProgress AI = info.Attacker.GetComponent<AIProgress>();
                        if(AI != null)
                        {
                            AI.ForceEndCurrentTask();
                        }
                        */
                    }
                }
                if (info.ProjectileSkill != null) {
                    if (CanBeBlocked && info.PreciselyBlockedFrame <= control.CharacterData.FirstFramesOfBlock)
                        control.Animator.SetTrigger (TransitionParameter.GetHitOnGuard.ToString ());
                    else if (PreciselyBlocked) {
                        control.Animator.SetTrigger (TransitionParameter.GetHitOnGuardPrecisely.ToString ());
                        if (info.CanBeReflected) {
                            Vector3 dir = info.Attacker.gameObject.transform.position - control.gameObject.transform.position;
                            dir.y = 0f;
                            ReflectProjectile (info, control.GetReflectProjSpawnPoint ().position, dir.normalized);
                            //info.IsFinished = true;
                        }
                        /*
                        info.ProjectileObject.Dead ();
                        GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.AttackInfo);
                        AttackInfo reflectionInfo = obj.GetComponent<AttackInfo> ();
                        reflectionInfo.Init (null, info.ProjectileSkill, control);
                        obj.SetActive (true);
                        extraAttackInfo.Add (reflectionInfo);
                        //AttackManager.Instance.CurrentAttackInfo.Add(reflectionInfo);
                        reflectionInfo.Register ();
                        reflectionInfo.ProjectileSkill.Launch (reflectionInfo, control, control.GetReflectProjSpawnPoint ());
                        */
                    }
                }

            }
            //CameraManager.Instance.ShakeCamera (info.HitReactDuration);
            //control.Dead ();
        }
        private void ReflectProjectile (AttackInfo info, Vector3 pos, Vector3 dir) {
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.AttackInfo);
            AttackInfo reflectionInfo = obj.GetComponent<AttackInfo> ();
            reflectionInfo.Init (null, info.ProjectileSkill, control);
            obj.SetActive (true);
            extraAttackInfo.Add (reflectionInfo);
            //AttackManager.Instance.CurrentAttackInfo.Add(reflectionInfo);
            reflectionInfo.Register ();
            /*
            Vector3 dir = info.Attacker.gameObject.transform.position - control.gameObject.transform.position;
            dir.y = 0f;
            */
            reflectionInfo.ProjectileSkill.Launch (reflectionInfo, control, pos, dir);
            info.ProjectileObject.Dead ();
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
                //Debug.Log ("Check Collision !!!");
                if (IsCollidedWithAttackPoint (info)) {
                    //Debug.Log ("Collision Occurs !!!");
                    ProcessGrappling (info);

                }

            }
        }
        private void ProcessGrappling (Grappler info) {
            //if (info.Target == null) {
            info.Target = control;
            //Debug.Log ("Grappler HIT !!!");
            control.CharacterData.IsGrappled = true;

            if (info.Type == GrapplerType.FrontStab) {

                control.HitReactionAndFreeze (info.FreezeStartTiming, GrapplerType.FrontStab);

                info.Attacker.CharacterController.Move (info.Attacker.gameObject.transform.forward * 0.6f);
                Vector3 AttackerFront = info.Attacker.gameObject.transform.forward * 1.2f;
                AttackerFront.y = 0f;
                Vector3 diff = control.gameObject.transform.position - info.Attacker.gameObject.transform.position;
                diff.y = 0f;
                Vector3 dir = AttackerFront - diff;
                control.CharacterController.Move (dir);

                Vector3 dirVector = control.gameObject.transform.position - info.Attacker.gameObject.transform.position;
                Vector3 hitVector = (new Vector3 (dirVector.x, 0, dirVector.z)).normalized;
                control.FaceTarget = -hitVector;
                control.TurnToTarget (0f, 0f);
                info.Attacker.FaceTarget = hitVector;
                info.Attacker.TurnToTarget (0f, 0f);

                control.gameObject.transform.parent = info.Attacker.Animator.gameObject.transform;

            } else if (info.Type == GrapplerType.DownStab) {

                control.HitReactionAndFreeze (info.FreezeStartTiming, GrapplerType.DownStab);

                //info.Attacker.CharacterController.Move (info.Attacker.gameObject.transform.forward * 0.6f);
                Vector3 AttackerPos = control.gameObject.transform.position + control.gameObject.transform.forward * 0.9f;
                AttackerPos.y = 0f;
                Vector3 diff = AttackerPos - info.Attacker.gameObject.transform.position;
                diff.y = 0f;
                info.Attacker.CharacterController.Move (diff);

                Vector3 dirVector = control.gameObject.transform.position - info.Attacker.gameObject.transform.position;
                Vector3 hitVector = (new Vector3 (dirVector.x, 0, dirVector.z)).normalized;
                control.FaceTarget = -hitVector;
                control.TurnToTarget (0f, 0f);
                info.Attacker.FaceTarget = hitVector;
                info.Attacker.TurnToTarget (0f, 0f);
            }

            //control.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0f);
            info.GrapplingHit ();

            //}
            //control.Dead ();
        }

    }
}