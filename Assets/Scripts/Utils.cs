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
        ButtonHold,
        GrapplingHit,
        SpeedMultiplier

    }

    [System.Serializable]
    public class TimeInterval {
        public int index;

        [Range (0f, 2f)]
        public float st;

        [Range (0f, 2f)]
        public float ed;
    }

}