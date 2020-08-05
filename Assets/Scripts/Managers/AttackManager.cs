using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class AttackManager : Singleton<AttackManager> {

        public List<AttackInfo> CurrentAttackInfo = new List<AttackInfo>();
        public void Init () {

        }

    }
}