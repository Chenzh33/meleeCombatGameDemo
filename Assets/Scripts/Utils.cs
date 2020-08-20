using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum TransitionParameter {
        Move,
        AttackMelee,
        ForcedTransition,
        Dodge,
        TransitionIndexer,
        AtkReleaseTiming,
        ButtonSmashing,
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