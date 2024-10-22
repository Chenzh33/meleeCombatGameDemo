using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/LaunchProjectile")]
    public class LaunchProjectile : SkillEffect {
        public float EnergyTaken = 1f;

        public float ProjectileLifeTime = 2f;
        public float ProjectileSpeed = 10f;
        public float ProjectileTileAngle = 30f;
        public float ProjectileTileAngleNoise = 15f;
        public float DirectionOffset = 0f;
        public bool NotNeedCharge;
        public bool IsAOERangeChangeWithScaling;
        public bool IsVFXScaleChangeWithScaling;
        public bool IsGeneratedOnSpawnPoint;

        [Range (0f, 1f)]
        public float ProjectileLaunchTiming = 0.3f;

        [Range (0f, 1f)]
        public float ReservedTime = 0.1f;
        //public Transform ProjectileSpawnPoint;

        public AttackType Type = AttackType.Projectile;
        public ProjectileType ProjType = ProjectileType.ChargedAttack;
        public int MaxTargetNum;
        public float Range;
        public float Damage = 1f;
        public float DamageInterval = 0.5f;
        public float KnockbackForce = 10f;
        public float KnockbackTime = 0.1f;
        public float HitReactDuration = 0.1f;
        public float Stun = 1f;
        public float ProjectileScale = 1f;
        public int PreciselyBlockedFrame = 10;
        public bool IsAttackForward;
        public bool IsAOEAttackTowardsCenter;
        public bool CanBeReflected;
        public bool CanReflectProjectile;
        public bool IsAttachedToPlayer;
        public AnimationCurve SpeedGraph;
        public AnimationCurve ScaleGraph;

        //[Range (0.01f, 1f)]
        //public float ComboInputStartTime = 0.3f;
        //[Range (0.01f, 1f)]
        //public List<float> ComboInputInterval = new List<float> {0f, 1f};
        //public float ComboInputEndTime = 0.7f;

        //public List<AttackType> AttackParts = new List<AttackPartType> ();
        //public List<AttackInfo> FinishedAttacks = new List<AttackInfo> ();

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (animator.GetBool (TransitionParameter.Charge.ToString ()) || NotNeedCharge) {
                if (stateEffect.CharacterControl.CharacterData.Energy >= EnergyTaken) {
                    stateEffect.CharacterControl.TakeEnergy (EnergyTaken, this);
                    GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.AttackInfo);
                    AttackInfo atkInfo = obj.GetComponent<AttackInfo> ();
                    atkInfo.Init (null, this, stateEffect.CharacterControl);
                    obj.SetActive (true);
                    AttackManager.Instance.CurrentAttackInfo.Add (atkInfo);
                    animator.SetBool (TransitionParameter.EnergyTaken.ToString (), true);
                } else {
                    stateEffect.CharacterControl.CharacterData.SendMessage (MessageType.EnergyNotEnough);
                }
            }

            //ProjectileSpawnPoint = stateEffect.CharacterControl.GetProjectileSpawnPoint();
            /*
            if(stateEffect.CharacterControl.CharacterData.GetPrevState() == Animator.StringToHash("AttackHold"))
            {
                Debug.Log("prev state is attack hold");

            }
            */
            //Debug.Log ("enter launch projectile: " + stateInfo.normalizedTime.ToString ());
        }

        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (!CheckInTransitionBetweenSameState (stateEffect.CharacterControl, animator) && stateInfo.normalizedTime >= ProjectileLaunchTiming && stateInfo.normalizedTime < ProjectileLaunchTiming + ReservedTime) {
                //if (!animator.IsInTransition(0) && stateInfo.normalizedTime >= ProjectileLaunchTiming) {
                foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                    if (!info.IsRegistered && info.ProjectileSkill == this) {
                        Vector3 spawnPoint = Vector3.zero;
                        if (IsGeneratedOnSpawnPoint)
                            spawnPoint = stateEffect.CharacterControl.GetProjectileSpawnPoint ().position;
                        else {
                            spawnPoint = stateEffect.CharacterControl.gameObject.transform.position;
                            spawnPoint.y = 0f;
                        }
                        //Launch (info, stateEffect.CharacterControl, spawnPoint, stateEffect.CharacterControl.FaceTarget);
                        Vector3 dir = animator.transform.root.forward;
                        if (DirectionOffset != 0f)
                            dir = Quaternion.Euler (0f, DirectionOffset, 0f) * dir;
                        Launch (info, stateEffect.CharacterControl, spawnPoint, dir);

                        info.Register ();
                        //Debug.Log ("register projectile : " + stateInfo.normalizedTime.ToString ());
                    }
                }
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            animator.SetBool (TransitionParameter.EnergyTaken.ToString (), false);
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
        public void Launch (AttackInfo info, CharacterControl control, Vector3 spawnPoint, Vector3 direction) {
            GameObject obj = null;
            switch (info.ProjType) {
                case ProjectileType.ChargedAttack:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.ProjectileChargedAttack);
                    break;
                case ProjectileType.RifleBullet:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.ProjectileBulletRifle);
                    break;
                case ProjectileType.ChargedAttackHold:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.ProjectileChargedAttackHold);
                    break;
                case ProjectileType.ChargedGuard:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.ProjectileChargedGuard);
                    break;
                case ProjectileType.PistolBullet:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.ProjectileBulletPistol);
                    break;
            }
            ProjectileVFX projectileVFX = obj.GetComponentInChildren<ProjectileVFX> ();
            if (projectileVFX != null) {
                float tileAngle = ProjectileTileAngle + Random.Range (-ProjectileTileAngleNoise, ProjectileTileAngleNoise);
                projectileVFX.transform.rotation = Quaternion.Euler (tileAngle, -90f, 0);
            }

            obj.transform.position = spawnPoint;
            //obj.transform.localRotation = Quaternion.identity;
            //obj.transform.localPosition = Vector3.zero;
            //obj.transform.parent = null;
            obj.SetActive (true);
            //obj.transform.rotation = Quaternion.LookRotation(control.FaceTarget, Vector3.up);
            obj.transform.rotation = Quaternion.LookRotation (direction, Vector3.up);

            info.gameObject.transform.parent = obj.transform;
            info.transform.localPosition = Vector3.zero;
            info.transform.localRotation = Quaternion.identity;
            info.transform.localScale = Vector3.one * ProjectileScale * ScaleGraph.Evaluate (0f);
            if (IsVFXScaleChangeWithScaling) {
                ParticleSystem[] pss = obj.GetComponentsInChildren<ParticleSystem> ();
                foreach (ParticleSystem ps in pss) {
                    ps.gameObject.transform.localScale = Vector3.one * ProjectileScale * ScaleGraph.Evaluate (0f);
                }
            }

            ProjectileObject projectileObject = obj.GetComponent<ProjectileObject> ();
            projectileObject.Init (info, ProjectileLifeTime, ProjectileSpeed);
            info.ProjectileObject = projectileObject;
            //control.ProjectileObjs.Add(projectileObject);
            //control.ProjectileList.Add()
            if (IsAttachedToPlayer)
                obj.transform.parent = control.gameObject.transform.root;

        }

    }
}