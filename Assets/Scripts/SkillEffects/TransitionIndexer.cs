using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/TransitionIndexer")]
    public class TransitionIndexer : SkillEffect {
        //[Range (0.01f, 1f)]
        //public List<float> TransitionZones = new List<float> { 0f, 1f };

        public List<TimeInterval> TimeIntervals = new List<TimeInterval> ();
        //public float ComboInputEndTime = 0.7f;

        /*
                [Range (0.01f, 1f)]
                public float AllowTransitionStartTime = 0.2f;
                [Range (0.01f, 1f)]
                public float AllowTransitionEndTime = 0.2f;
                public int Index;
                */
        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            animator.SetInteger (TransitionParameter.TransitionIndexer.ToString (), 0);
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            CheckTransition (stateEffect, animator, stateInfo);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            animator.SetInteger (TransitionParameter.TransitionIndexer.ToString (), 0);
            animator.SetBool (TransitionParameter.ForcedTransition.ToString (), false);
        }

        public void CheckTransition (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            bool inInterval = false;
            bool ForceTransition = false;
            foreach (TimeInterval t in TimeIntervals) {
                if (t.index >= 0 && stateInfo.normalizedTime >= t.st && stateInfo.normalizedTime < t.ed) {
                    animator.SetInteger (TransitionParameter.TransitionIndexer.ToString (), t.index);
                    inInterval = true;
                }

                if (t.index < 0) {
                    if (stateInfo.normalizedTime >= t.st && stateInfo.normalizedTime < t.ed)
                        ForceTransition = true;
                }
            }
            if (!inInterval)
                animator.SetInteger (TransitionParameter.TransitionIndexer.ToString (), 0);

            if (ForceTransition)
                animator.SetBool (TransitionParameter.ForcedTransition.ToString (), true);
            else
                animator.SetBool (TransitionParameter.ForcedTransition.ToString (), false);
        }
    }
}