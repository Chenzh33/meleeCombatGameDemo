using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class StatewithEffect : StateMachineBehaviour {
        public List<SkillEffect> ListSkillEffect = new List<SkillEffect> ();
        private CharacterControl characterControl;

        public CharacterControl CharacterControl {
            get {
                return characterControl;
            }
        }

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (characterControl == null)
                //characterControl = animator.transform.root.GetComponentInChildren<CharacterControl> ();
                characterControl = animator.transform.root.GetComponent<CharacterControl> ();
            if (characterControl != null) {
                characterControl.CharacterData.LoadState (stateInfo.shortNameHash);
                //Debug.Log(stateInfo.shortNameHash);
                foreach (SkillEffect se in ListSkillEffect) {
                    se.OnEnter (this, animator, stateInfo);

                }
            }

        }
        public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (characterControl != null) {
                foreach (SkillEffect se in ListSkillEffect) {
                    se.UpdateEffect (this, animator, stateInfo);
                }
            }

        }
        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (characterControl != null) {
                foreach (SkillEffect se in ListSkillEffect) {
                    se.OnExit (this, animator, stateInfo);
                }
            }

        }
    }
}