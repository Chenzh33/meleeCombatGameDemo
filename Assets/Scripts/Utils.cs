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
        IdleState

    }

    public enum PoolObjectType {
        ATTACK_INFO,
        ATTACK_HOLD_PROJECTILE,
        BUTTON_SMASHER,
        GRAPPLER,
        PATH_FINDING_AGENT,
        CAMERA_SHAKER,
        SLAM_VFX,
        ATTACK_HOLD_AOE_VFX
    }
    public enum TeamTag{
       Player,
       Bandit, 
       

    }
    public enum BarImageType{
        Bound,
        Fill,
        Red
    }
    public enum VFXType{
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