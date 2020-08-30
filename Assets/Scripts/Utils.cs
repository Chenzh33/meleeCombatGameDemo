using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum TransitionParameter {
        Move,
        AttackMelee,
        AttackExecute,
        ForcedTransition,
        Dodge,
        TransitionIndexer,
        AtkReleaseTiming,
        ExcReleaseTiming,
        ButtonSmashing,
        AtkButtonHold,
        ExcButtonHold,
        GrapplingHit,
        SpeedMultiplier

    }

    public enum TeamTag{
       Player,
       Bandit, 
       

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