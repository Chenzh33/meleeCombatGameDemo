using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "AI/AISwarmMethod")]
    public class AISwarmMethod : SkillEffect {

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            AIProgress ai = stateEffect.CharacterControl.AIProgress;
            if (!AIAgentManager.Instance.CurrentSwarmAgent.Contains (ai))
                AIAgentManager.Instance.CurrentSwarmAgent.Add (ai);

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            AIProgress ai = stateEffect.CharacterControl.AIProgress;
            if (AIAgentManager.Instance.CurrentSwarmAgent.Contains (ai))
                AIAgentManager.Instance.CurrentSwarmAgent.Remove(ai);
        }

    }
}