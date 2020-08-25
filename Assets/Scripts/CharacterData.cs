using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [System.Serializable]
    public class CharacterData {
        public float HP;
        public float MaxHP;
        public bool IsColliderOff;
        public bool IsAnimationPause;
        public bool IsGrappled;
        public bool IsDead;
        public bool IsInvincible;
        public CharacterControl GrapplingTarget;
        public TeamTag Team;

        public const int STATE_BUFFER_SIZE = 3;
        public int[] StateBuffer = new int[STATE_BUFFER_SIZE];
        private int curIndex;

        void Start () {
            curIndex = 0;

        }

        void Update () {

        }

        public int GetPrevState () {
            return StateBuffer[(curIndex - 1 + STATE_BUFFER_SIZE) % STATE_BUFFER_SIZE];
        }

        public int GetCurrState () {
            return StateBuffer[curIndex];
        }
        public void LoadState (int name) {
            curIndex = (curIndex + 1) % STATE_BUFFER_SIZE;
            StateBuffer[curIndex] = name;

        }
    }
}