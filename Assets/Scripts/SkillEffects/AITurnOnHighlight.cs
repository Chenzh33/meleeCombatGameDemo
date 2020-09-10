using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "AI/AITurnOnHighlight")]
    public class AITurnOnHighlight : SkillEffect {
        public float HighlightFactor = 2f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            AIProgress aiAgent = stateEffect.CharacterControl.gameObject.GetComponent<AIProgress> ();
            aiAgent.TurnOnHighlight (HighlightFactor);

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            AIProgress aiAgent = stateEffect.CharacterControl.gameObject.GetComponent<AIProgress> ();
            aiAgent.TurnOffHighlight (HighlightFactor);

        }

    }
}