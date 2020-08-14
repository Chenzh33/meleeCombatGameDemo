using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {
   



    [System.Serializable]
    public class TimeInterval{
        public int index;

        [Range (0.01f, 2f)]
        public float st;

        [Range (0.01f, 2f)]
        public float ed;
    }

}