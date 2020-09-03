using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum TransitionParameter {
        Move,
        AttackMelee,
        AttackExecute,
        ForcedTransitionDodge,
        ForcedTransitionExecute,
        ForcedTransitionAttackHold,
        ForcedTransitionAttackHoldFS,
        Dodge,
        TransitionIndexer,
        AtkReleaseTiming,
        ExcReleaseTiming,
        DdgReleaseTiming,
        ButtonSmashing,
        AtkButtonHold,
        ExcButtonHold,
        DdgButtonHold,
        GrapplingHit,
        SpeedMultiplier,
        Stunned

    }

    public enum TeamTag{
       Player,
       Bandit, 
       

    }
    public enum VFXType{
        Trail,
        Hold

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