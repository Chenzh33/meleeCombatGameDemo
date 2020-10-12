using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "AI/AISwarmMethod")]
    public class AISwarmMethod : SkillEffect {
        public float AvoidanceRadius = 4f;
        public float AvoidanceForce = 2.0f;
        public float KeepoutRadius = 3f;
        public float KeepoutForce = 3.0f;
        public float Smooth = 5.0f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            /*
            AIProgress ai = stateEffect.CharacterControl.AIProgress;
            if (!AIAgentManager.Instance.CurrentSwarmAgent.Contains (ai))
                AIAgentManager.Instance.CurrentSwarmAgent.Add (ai);
                */

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            /*
            AIProgress ai = stateEffect.CharacterControl.AIProgress;
            if (!ai.enabled)
                return;
            if (ai.enemyTarget == null)
                return;
            if (animator.GetBool(TransitionParameter.Dodge.ToString()))
                return;
            Vector3 direction = ai.enemyTarget.gameObject.transform.position - ai.gameObject.transform.position;
            direction.y = 0f;
            //Vector3 direction = -ai.gameObject.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            animator.transform.root.rotation = Quaternion.Slerp(animator.transform.root.rotation, rotation, Time.deltaTime * Smooth);
           
            ai.inputVectorIncremental = new Vector2();
            //ai.inputVectorIncremental += ai.enemyTarget.inputVector;

            Vector2 avoidanceVector = new Vector2();
            Vector2 keepoutVector = new Vector2();
            foreach(AIProgress agent in AIAgentManager.Instance.CurrentSwarmAgent)
            {
                if(ai == agent)
                    continue;
                Vector3 dir = ai.gameObject.transform.position - agent.gameObject.transform.position;
                dir.y = 0f;
                if(dir.magnitude < AvoidanceRadius)
                {
                    float f = 1.0f - dir.magnitude / AvoidanceRadius;
                    Vector2 dir2d = new Vector2(dir.x, dir.z);
                    avoidanceVector += dir2d.normalized * f * AvoidanceForce;
                }

            }
            if (AIAgentManager.Instance.CurrentSwarmAgent.Count > 2)
                avoidanceVector = avoidanceVector / (AIAgentManager.Instance.CurrentSwarmAgent.Count - 1);
            Vector3 keepoutDir = ai.gameObject.transform.position - ai.enemyTarget.gameObject.transform.position;
            keepoutVector = new Vector2();
            if (keepoutDir.magnitude < KeepoutRadius)
            {
                float factor = 1.0f - keepoutDir.magnitude / KeepoutRadius;
                keepoutVector = (new Vector2(keepoutDir.x, keepoutDir.z)).normalized * factor * KeepoutForce;
            }
            ai.inputVectorIncremental += avoidanceVector;
            ai.inputVectorIncremental += keepoutVector;

                */
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            /*
            AIProgress ai = stateEffect.CharacterControl.AIProgress;
            if (AIAgentManager.Instance.CurrentSwarmAgent.Contains (ai))
                AIAgentManager.Instance.CurrentSwarmAgent.Remove(ai);
            ai.inputVectorIncremental = new Vector2();
                */
        }

    }
}