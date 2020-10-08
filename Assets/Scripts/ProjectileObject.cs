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
        private float ProjectileScale;
        private bool IsMoving;
        private bool RangeChangeWithScaling;
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

        }
        void Start () {

        }

        void Update () {
            if (IsMoving && CurrentTime < Duration) {
                //Debug.DrawRay(transform.position, transform.forward, Color.red);
                transform.Translate (transform.forward * Speed * SpeedGraph.Evaluate (CurrentTime / Duration) * Time.deltaTime, Space.World);
                ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in pss)
                {
                    ps.gameObject.transform.localScale = Vector3.one * ScaleGraph.Evaluate(CurrentTime / Duration) * ProjectileScale;
                }
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

        public float GetCurrentScale()
        {
            ParticleSystem[] pss = GetComponentsInChildren<ParticleSystem>();
            if (pss != null)
                return pss[0].gameObject.transform.localScale.x;
            else
                return 0f;
        }
    }
}