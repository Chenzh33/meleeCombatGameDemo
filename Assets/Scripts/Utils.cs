using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum TransitionParameter {
        Move,
        AttackMelee,
        AttackExecute,
        Dodge,
        Charge,
        TransitionIndexer,
        ForcedTransitionDodge,
        ForcedTransitionExecute,
        ForcedTransitionAttackHold,
        ForcedTransitionAttackHoldFS,
        AttackMeleeLink,
        AtkReleaseTiming,
        ExcReleaseTiming,
        DdgReleaseTiming,
        ButtonSmashing,
        AtkButtonHold,
        ExcButtonHold,
        DdgButtonHold,
        GrapplingHit,
        SpeedMultiplier,
        Stunned,
        MoveHold,
        ForbidDodge,
        IdleState,
        EnergyTaken,
        Fire,

    }

    public enum PoolObjectType {
        AttackInfo,
        ProjectileChargedAttack,
        ButtonSmasher,
        Grappler,
        PathFindingAgent,
        CameraShaker,
        VFXSlam,
        VFXAttackHold,
        ProjectileBullet,
        Shadow
    }
    public enum TeamTag {
        Player,
        Bandit,

    }

    public enum AttackType {
        Null,
        MustCollide,
        AOE,
        Projectile

    }
    public enum ProjectileType {
        ChargedAttack,
        Bullet

    }
    public enum BarImageType {
        Bound,
        Fill,
        Red
    }
    public enum VFXType {
        Null,
        Trail,
        Hold,
        Slam,
        AttackHoldAOE

    }

    [System.Serializable]
    public class TimeInterval {
        public int index;

        [Range (0f, 10f)]
        public float st;

        [Range (0f, 10f)]
        public float ed;
    }

}