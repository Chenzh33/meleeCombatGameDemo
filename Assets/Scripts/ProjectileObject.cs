using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class ProjectileObject : MonoBehaviour {
        public CharacterControl Owner;
        public AttackInfo ProjectileInfo;
        public float Duration;
        public float Speed;
        public float CurrentTime;
        public float CurrentTimeToDamage;
        public float ProjectileScale;
        public bool IsMoving;
        public bool IsAOERangeChangeWithScaling;
        public bool IsVFXScaleChangeWithScaling;
        private AnimationCurve SpeedGraph;
        private AnimationCurve ScaleGraph;

        public void Init (AttackInfo projectileInfo, float duration, float speed) {
            ProjectileInfo = projectileInfo;
            Owner = projectileInfo.Attacker;
            CurrentTime = 0;
            Duration = duration;
            Speed = speed;
            IsMoving = true;
            SpeedGraph = projectileInfo.ProjectileSkill.SpeedGraph;
            ScaleGraph = projectileInfo.ProjectileSkill.ScaleGraph;
            ProjectileScale = projectileInfo.ProjectileSkill.ProjectileScale;
            IsAOERangeChangeWithScaling = projectileInfo.IsAOERangeChangeWithScaling;
            IsVFXScaleChangeWithScaling = projectileInfo.IsVFXScaleChangeWithScaling;

        }
        void Start () {

        }

        void FixedUpdate () {
            if (IsMoving && !ProjectileInfo.IsFinished && CurrentTime < Duration) {
                //Debug.DrawRay(transform.position, transform.forward, Color.red);
                if (Speed > 0f)
                    transform.Translate (transform.forward * Speed * SpeedGraph.Evaluate (CurrentTime / Duration) * Time.fixedDeltaTime, Space.World);
                ProjectileInfo.transform.localScale = Vector3.one * ScaleGraph.Evaluate (CurrentTime / Duration) * ProjectileScale;
                if (IsVFXScaleChangeWithScaling) {
                    ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem> ();
                    foreach (ParticleSystem ps in pss) {
                        ps.gameObject.transform.localScale = Vector3.one * ScaleGraph.Evaluate (CurrentTime / Duration) * ProjectileScale;
                    }
                }
                CurrentTime += Time.fixedDeltaTime;
                CurrentTimeToDamage += Time.fixedDeltaTime;
                if (CurrentTimeToDamage > ProjectileInfo.DamageInterval) {
                    ProjectileInfo.Targets.Clear ();
                    ProjectileInfo.CurrentTargetNum = 0;
                    CurrentTimeToDamage = 0f;
                }
            } else {
                Dead ();
                if (AttackManager.Instance.CurrentAttackInfo.Contains (ProjectileInfo)) {
                    AttackManager.Instance.CurrentAttackInfo.Remove (ProjectileInfo);
                    PoolObject infoObj = ProjectileInfo.GetComponent<PoolObject> ();
                    PoolManager.Instance.ReturnToPool (infoObj);
                    ProjectileInfo.Clear ();
                }
            }
        }

        public void Dead () {
            ProjectileInfo.IsFinished = true;
            ProjectileInfo.IsRegistered = false;

            IsMoving = false;
            /*
            if (Owner.ProjectileObjs.Contains (this))
                Owner.ProjectileObjs.Remove (this);
                */

            PoolObject pobj = gameObject.GetComponent<PoolObject> ();
            PoolManager.Instance.ReturnToPool (pobj);

        }

        public float GetCurrentScale () {
            return ProjectileInfo.transform.localScale.x;
            /*
            ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem> ();
            if (pss != null)
                return pss[0].gameObject.transform.localScale.x;
            else
                return 0f;
                */
        }
    }
}