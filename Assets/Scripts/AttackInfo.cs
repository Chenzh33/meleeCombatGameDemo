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
                Type = attackSkill.attackType;
                MaxTargetNum = attackSkill.MaxTargetNum;
                Range = attackSkill.Range;
                Damage = attackSkill.Damage;
                KnockbackForce = attackSkill.KnockbackForce;
                HitReactDuration = attackSkill.HitReactDuration;
            } else {
                IsAttackForward = projectileSkill.IsAttackForward;
                Type = projectileSkill.attackType;
                MaxTargetNum = projectileSkill.MaxTargetNum;
                Range = projectileSkill.Range;
                Damage = projectileSkill.Damage;
                KnockbackForce = projectileSkill.KnockbackForce;
                HitReactDuration = projectileSkill.HitReactDuration;
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
        }
        void Start () {

        }

        void Update () {

        }
        public void Register () {
            IsRegistered = true;

        }
    }
}