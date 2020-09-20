using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class ProjectileObject : MonoBehaviour {
        public CharacterControl Owner;
        public AttackInfo ProjectileInfo;
        private float Duration;
        private float Speed;
        private float CurrentTime;
        private float CurrentTimeToDamage;
        private bool IsMoving;
        private AnimationCurve SpeedGraph;

        public void Init (AttackInfo projectileInfo, float duration, float speed) {
            ProjectileInfo = projectileInfo;
            Owner = projectileInfo.Attacker;
            CurrentTime = 0;
            Duration = duration;
            Speed = speed;
            IsMoving = true;
            SpeedGraph = projectileInfo.ProjectileSkill.SpeedGraph;

        }
        void Start () {

        }

        void Update () {
            if (IsMoving && CurrentTime < Duration) {
                //Debug.DrawRay(transform.position, transform.forward, Color.red);
                transform.Translate (transform.forward * Speed * SpeedGraph.Evaluate (CurrentTime / Duration) * Time.deltaTime, Space.World);
              
                CurrentTime += Time.deltaTime;
                CurrentTimeToDamage += Time.deltaTime;
                if (CurrentTimeToDamage > ProjectileInfo.DamageInterval) {
                    ProjectileInfo.Targets.Clear ();
                    ProjectileInfo.CurrentTargetNum = 0;
                    CurrentTimeToDamage = 0f;
                }
            } else {
                ProjectileInfo.IsFinished = true;
                ProjectileInfo.IsRegistered = false;
                if (AttackManager.Instance.CurrentAttackInfo.Contains (ProjectileInfo)) {
                    AttackManager.Instance.CurrentAttackInfo.Remove (ProjectileInfo);
                    PoolObject pobj = ProjectileInfo.GetComponent<PoolObject> ();
                    PoolManager.Instance.ReturnToPool (pobj);
                    ProjectileInfo.Clear ();
                }
                Dead ();
            }
        }

        public void Dead () {
            IsMoving = false;
            /*
            if (Owner.ProjectileObjs.Contains (this))
                Owner.ProjectileObjs.Remove (this);
                */
            PoolObject pobj = gameObject.GetComponent<PoolObject> ();
            PoolManager.Instance.ReturnToPool (pobj);
        }
    }
}