using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class AttackInfo : MonoBehaviour {
        public Attack AttackSkill;
        public LaunchProjectile ProjectileSkill;
        public bool IsRegistered;
        public bool IsFinished;
        public bool IsAttackForward;
        public bool IsAOEAttackTowardsCenter;
        public bool IsAOEAttackAttachToPlayer;
        public int CurrentTargetNum;
        public int MaxTargetNum;
        public CharacterControl Attacker;
        public List<CharacterControl> Targets = new List<CharacterControl> ();
        public AttackType Type;
        public float Range;
        public ProjectileObject ProjectileObject;
        public float Damage;
        public float KnockbackForce;
        public float HitReactDuration;
        public float Stun;
        //public float AOEAttackCenterOffset = 3.0f;
        //public Transform AttackCenter;
        public VFXType vfxType = VFXType.Slam;
        public PoolObject VFXObj;

        public void Init (Attack attackSkill, LaunchProjectile projectileSkill, CharacterControl attacker) {
            AttackSkill = attackSkill;
            ProjectileSkill = projectileSkill;
            IsRegistered = false;
            IsFinished = false;
            CurrentTargetNum = 0;
            Attacker = attacker;
            Targets.Clear ();
            ProjectileObject = null;

            if (attackSkill != null) {
                IsAttackForward = attackSkill.IsAttackForward;
                IsAOEAttackTowardsCenter = attackSkill.IsAOEAttackTowardsCenter;
                IsAOEAttackAttachToPlayer = attackSkill.IsAOEAttackAttachToPlayer;
                Type = attackSkill.Type;
                MaxTargetNum = attackSkill.MaxTargetNum;
                Range = attackSkill.Range;
                Damage = attackSkill.Damage;
                KnockbackForce = attackSkill.KnockbackForce;
                HitReactDuration = attackSkill.HitReactDuration;
                Stun = attackSkill.Stun;
                vfxType = attackSkill.vfxType;
                VFXObj = null;
                //AOEAttackCenterOffset = attackSkill.AOEAttackCenterOffset;
                //AttackCenter = Attacker.CharacterData.AOEAttackCenter;
            } else {
                IsAttackForward = projectileSkill.IsAttackForward;
                Type = projectileSkill.Type;
                MaxTargetNum = projectileSkill.MaxTargetNum;
                Range = projectileSkill.Range;
                Damage = projectileSkill.Damage;
                KnockbackForce = projectileSkill.KnockbackForce;
                HitReactDuration = projectileSkill.HitReactDuration;
                Stun = projectileSkill.Stun;
            }
        }

        public void Clear () {
            AttackSkill = null;
            ProjectileSkill = null;
            IsRegistered = false;
            IsFinished = false;
            CurrentTargetNum = 0;
            MaxTargetNum = 0;
            Attacker = null;
            Targets.Clear ();
            Type = AttackType.NULL;
            Range = 0f;
            ProjectileObject = null;
            //AttackCenter = null;
            /*
            if (VFXObj != null) {
                ParticleSystem ps = VFXObj.GetComponent<ParticleSystem> ();
                ps.Clear ();
                ps.Stop (true);
                PoolManager.Instance.ReturnToPool (VFXObj);
                VFXObj = null;
            }
            */
            VFXObj = null;
        }

        void Start () {

        }

        void Update () {

        }
        public void Register () {
            IsRegistered = true;
            if (Type == AttackType.AOE)
            {
                Vector3 pos = Attacker.gameObject.transform.position + Attacker.gameObject.transform.forward * AttackSkill.AOEAttackCenterOffset;
                this.gameObject.transform.position = new Vector3(pos.x, 0f, pos.z);
                if(IsAOEAttackAttachToPlayer)
                    this.gameObject.transform.parent = Attacker.gameObject.transform;
                if (vfxType != VFXType.Null)
                {
                    GameObject obj = GenerateVFXObject();
                    obj.SetActive(true);
                    obj.transform.position = this.gameObject.transform.position;
                    //obj.transform.parent = this.gameObject.transform;
                    ParticleSystem ps = obj.GetComponent<ParticleSystem>();
                    ps.Play(true);
                    VFXObj = obj.GetComponent<PoolObject>();
                    VFXObj.WaitAndDestroy(1f);
                    //Attacker.VFXs.Add (obj.GetComponent<PoolObject> ());
                }
            }

        }

        public GameObject GenerateVFXObject () {
            GameObject obj = null;
            switch (vfxType) {
                case VFXType.Slam:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.SLAM_VFX);
                    break;
                case VFXType.AttackHoldAOE:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.ATTACK_HOLD_AOE_VFX);
                    break;

            }
            return obj;
        }
    }
}