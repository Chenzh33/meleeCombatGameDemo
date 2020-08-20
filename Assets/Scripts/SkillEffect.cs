using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {
    public abstract class SkillEffect : ScriptableObject {
        public abstract void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo);
        public abstract void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo);
        public abstract void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo);

        public bool CheckInTransitionBetweenSameState (CharacterControl control, Animator animator) {
            if (animator.IsInTransition (0)) {
                if (control.CharacterData.GetPrevState () == control.CharacterData.GetCurrState ()) {
                    Debug.Log ("Check in transition between same state");
                    return true;
                }
            }
            return false;

        }

    }
}