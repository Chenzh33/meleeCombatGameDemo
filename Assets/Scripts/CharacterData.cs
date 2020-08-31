using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [System.Serializable]
    public class CharacterData {
        public float HP;
        public float MaxHP;
        public float Armour;
        public float MaxArmour;
        public float ArmourRegenerationInStun;
        public float ArmourRegeneration;
        public bool IsColliderOff;
        public bool IsAnimationPause;
        public bool IsGrappled;
        public bool IsDead;
        public bool IsInvincible;
        public bool IsRunning;
        public bool IsStunned;
        public CharacterControl GrapplingTarget;
        public TeamTag Team;

        public const int STATE_BUFFER_SIZE = 3;
        public int[] StateBuffer = new int[STATE_BUFFER_SIZE];
        private int curIndex = 0;

        public void UpdateData () {
            if (!IsStunned && Armour < MaxArmour)
                Armour += ArmourRegeneration * Time.deltaTime;
            if (Armour > MaxArmour)
                Armour = MaxArmour;

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