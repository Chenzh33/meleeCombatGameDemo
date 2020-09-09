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

        [Range (0f, 1f)]
        public float ProjectileLaunchTiming = 0.3f;

        [Range (0f, 1f)]
        public float ReservedTime = 0.1f;
        //public Transform ProjectileSpawnPoint;

        public AttackType Type = AttackType.PROJECTILE;
        public int MaxTargetNum;
        public float Range;
        public float Damage = 1f;
        public float KnockbackForce = 10f;
        public float HitReactDuration = 0.1f;
        public float Stun = 1f;
        public bool IsAttackForward;
        public AnimationCurve SpeedGraph;

        //[Range (0.01f, 1f)]
        //public float ComboInputStartTime = 0.3f;
        //[Range (0.01f, 1f)]
        //public List<float> ComboInputInterval = new List<float> {0f, 1f};
        //public float ComboInputEndTime = 0.7f;

        //public List<AttackType> AttackParts = new List<AttackPartType> ();
        //public List<AttackInfo> FinishedAttacks = new List<AttackInfo> ();

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (animator.GetBool (TransitionParameter.Charge.ToString ())) {
                if (stateEffect.CharacterControl.CharacterData.Energy >= EnergyTaken) {
                    stateEffect.CharacterControl.TakeEnergy (EnergyTaken, this);
                    GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.ATTACK_INFO);
                    AttackInfo atkInfo = obj.GetComponent<AttackInfo> ();
                    atkInfo.Init (null, this, stateEffect.CharacterControl);
                    obj.SetActive (true);
                    AttackManager.Instance.CurrentAttackInfo.Add (atkInfo);
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
                        info.Register ();
                        Launch (info, stateEffect.CharacterControl);
                        Debug.Log ("register projectile : " + stateInfo.normalizedTime.ToString ());
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
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.ATTACK_HOLD_PROJECTILE);
            ProjectileVFX projectileVFX = obj.GetComponentInChildren<ProjectileVFX> ();
            float tileAngle = ProjectileTileAngle + Random.Range(-ProjectileTileAngleNoise, ProjectileTileAngleNoise);
            projectileVFX.transform.rotation = Quaternion.Euler (tileAngle, -90f, 0);

            obj.transform.parent = control.GetProjectileSpawnPoint ();
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.parent = null;
            obj.SetActive (true);
            //obj.transform.rotation.x = ProjectileTileAngle;

            ProjectileObject projectileObject = obj.GetComponent<ProjectileObject> ();
            projectileObject.Init (info, ProjectileLifeTime, ProjectileSpeed);
            info.ProjectileObject = projectileObject;
            //control.ProjectileObjs.Add(projectileObject);
            //control.ProjectileList.Add()

        }

    }
}