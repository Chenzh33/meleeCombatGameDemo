using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "AI/AIMovement")]
    public class AIMovement : SkillEffect {
        public bool UseCrowdMethod;
        public bool AlwaysFaceTarget;
        public float AvoidanceRadius = 4f;
        public float AvoidanceForce = 2.0f;
        public float KeepoutRadius = 3f;
        public float KeepoutForce = 3.0f;
        public float Smooth = 5.0f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            AIProgress AI = stateEffect.CharacterControl.AIProgress;
            if (UseCrowdMethod)
                RegisterInCrowdState (AI);
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            AIProgress AI = stateEffect.CharacterControl.AIProgress;

            if (!AI.enabled || AI.pathFindingAgent == null || AI.enemyTarget == null)
                return;
            if (AI.pathFindingAgent.IsStopped ()) {
                //AI.ResetInputVector ();
                return;
            }
            AI.pathFindingAgent.GoToTarget();

            if (AlwaysFaceTarget) {
                Vector3 direction = AI.enemyTarget.gameObject.transform.position - AI.gameObject.transform.position;
                direction.y = 0f;
                Quaternion rotation = Quaternion.LookRotation (direction, Vector3.up);
                animator.transform.root.rotation = Quaternion.Slerp (animator.transform.root.rotation, rotation, Time.deltaTime * Smooth);

            }

            if (UseCrowdMethod)
                RegisterInCrowdState (AI);

            if (AI.IsInCrowd) {

                AI.inputVectorIncremental = new Vector2 ();

                Vector2 avoidanceVector = new Vector2 ();
                foreach (AIProgress agent in AIAgentManager.Instance.CurrentAgentCrowd) {
                    if (AI == agent)
                        continue;
                    Vector3 avoidanceDir = AI.gameObject.transform.position - agent.gameObject.transform.position;
                    avoidanceDir.y = 0f;
                    if (avoidanceDir.magnitude < AvoidanceRadius) {
                        float f = 1.0f - avoidanceDir.magnitude / AvoidanceRadius;
                        avoidanceVector += (new Vector2 (avoidanceDir.x, avoidanceDir.z)).normalized * f * AvoidanceForce;
                    }

                }
                if (AIAgentManager.Instance.CurrentAgentCrowd.Count > 2)
                    avoidanceVector = avoidanceVector / (AIAgentManager.Instance.CurrentAgentCrowd.Count - 1);

                Vector2 keepoutVector = new Vector2 ();
                Vector3 keepoutDir = AI.gameObject.transform.position - AI.enemyTarget.gameObject.transform.position;
                if (keepoutDir.magnitude < KeepoutRadius) {
                    float factor = 1.0f - keepoutDir.magnitude / KeepoutRadius;
                    keepoutVector = (new Vector2 (keepoutDir.x, keepoutDir.z)).normalized * factor * KeepoutForce;
                }
                AI.inputVectorIncremental += avoidanceVector;
                AI.inputVectorIncremental += keepoutVector;
                // test
            }
            AI.SetInputVector ();

        }

        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            AIProgress AI = stateEffect.CharacterControl.AIProgress;
            AI.inputVectorIncremental = new Vector2 ();
        }

        public void RegisterInCrowdState (AIProgress AI) {

            if (AI.IsInCrowd) {
                if (!AIAgentManager.Instance.CurrentAgentCrowd.Contains (AI))
                    AIAgentManager.Instance.CurrentAgentCrowd.Add (AI);
            } else {
                if (AIAgentManager.Instance.CurrentAgentCrowd.Contains (AI))
                    AIAgentManager.Instance.CurrentAgentCrowd.Remove (AI);
            }

        }

    }
}