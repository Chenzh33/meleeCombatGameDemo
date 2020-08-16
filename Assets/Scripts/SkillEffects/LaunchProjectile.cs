using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/LaunchProjectile")]
    public class LaunchProjectile : SkillEffect {

        public float ProjectileLifeTime = 2f;
        public float ProjectileSpeed = 10f;

        [Range (0f, 1f)]
        public float ProjectileLaunchTiming = 0.3f;

        //public Transform ProjectileSpawnPoint;

        public AttackType attackType = AttackType.PROJECTILE;
        public int MaxTargetNum;
        public float Range;
        public float Damage = 1f;
        public float KnockbackForce = 10f;

        //[Range (0.01f, 1f)]
        //public float ComboInputStartTime = 0.3f;
        //[Range (0.01f, 1f)]
        //public List<float> ComboInputInterval = new List<float> {0f, 1f};
        //public float ComboInputEndTime = 0.7f;

        //public List<AttackType> AttackParts = new List<AttackPartType> ();
        //public List<AttackInfo> FinishedAttacks = new List<AttackInfo> ();

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.ATTACK_INFO);
            AttackInfo atkInfo = obj.GetComponent<AttackInfo> ();
            atkInfo.Init (null, this, stateEffect.CharacterControl);
            obj.SetActive (true);
            AttackManager.Instance.CurrentAttackInfo.Add (atkInfo);
            //ProjectileSpawnPoint = stateEffect.CharacterControl.GetProjectileSpawnPoint();
        }

        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime >= ProjectileLaunchTiming) {
                foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                    if (!info.IsRegistered && info.ProjectileSkill == this) {
                        info.Register ();
                        Launch (info, stateEffect.CharacterControl);
                    }
                }
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            /*
            foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                if (!info.IsFinished && info.AttackSkill == this) {
                    info.IsFinished = true;
                    info.IsRegistered = false;
                    FinishedAttacks.Add (info);
                }
            }
            foreach (AttackInfo info in FinishedAttacks) {
                if (AttackManager.Instance.CurrentAttackInfo.Contains (info)) {
                    info.Clear ();
                    AttackManager.Instance.CurrentAttackInfo.Remove (info);
                    PoolObject pobj = info.GetComponent<PoolObject> ();
                    PoolManager.Instance.ReturnToPool (pobj);
                }
            }
            FinishedAttacks.Clear ();
            */

        }
        public void Launch (AttackInfo info, CharacterControl control) {
            GameObject obj = PoolManager.Instance.GetObject(PoolObjectType.ATTACK_HOLD_PROJECTILE);
            obj.transform.parent = control.GetProjectileSpawnPoint();
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.parent = null;
            obj.SetActive(true);
            ProjectileObject projectileObject = obj.GetComponent<ProjectileObject>();
            projectileObject.Init(info, ProjectileLifeTime, ProjectileSpeed);
            info.ProjectileObject = projectileObject;
            //control.ProjectileObjs.Add(projectileObject);
            //control.ProjectileList.Add()

        }
       

    }
}