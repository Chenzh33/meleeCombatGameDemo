using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "AI/AISimplePathFinding")]
    public class AISimplePathFinding : SkillEffect {

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
           

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (!stateEffect.CharacterControl.AIProgress.pathFindingAgent.IsStopped ())
                stateEffect.CharacterControl.AIProgress.SetInputVector ();
            else
                stateEffect.CharacterControl.AIProgress.ResetInputVector ();
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
           
        }

    }
}