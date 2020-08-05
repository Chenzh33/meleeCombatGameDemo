using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class AttackInfo : MonoBehaviour {
        public CharacterControl Attacker;
        public List<CharacterControl> Targets = new List<CharacterControl>();
        public Attack AttackSkill;
        public bool IsFinished;
        public bool IsRegistered;

        public void Init (Attack attackSkill, CharacterControl attacker) {
            IsRegistered = false;
            IsFinished = false;
            AttackSkill = attackSkill;
            Attacker = attacker;
            Targets.Clear();
        }
        void Start () {

        }

        void Update () {

        }
        public void Register()
        {
            IsRegistered = true;

        }
    }
}