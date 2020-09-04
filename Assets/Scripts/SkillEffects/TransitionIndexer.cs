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
            animator.SetBool (TransitionParameter.ForcedTransitionDodge.ToString (), false);
            animator.SetBool (TransitionParameter.ForcedTransitionExecute.ToString (), false);
            animator.SetBool (TransitionParameter.ForcedTransitionAttackHold.ToString (), false);
            animator.SetBool (TransitionParameter.ForcedTransitionAttackHoldFS.ToString (), false);
        }

        public void CheckTransition (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            bool inInterval = false;
            bool ForcedTransitionDodge = false;
            bool ForcedTransitionExecute = false;
            bool ForcedTransitionAttackHold = false;
            bool ForcedTransitionAttackHoldFS = false;
            foreach (TimeInterval t in TimeIntervals) {
                if (stateInfo.normalizedTime >= t.st && stateInfo.normalizedTime < t.ed) {
                    if (t.index >= 0) {
                        animator.SetInteger (TransitionParameter.TransitionIndexer.ToString (), t.index);
                        inInterval = true;
                    } else if (t.index == -1)
                        ForcedTransitionDodge = true;
                    else if (t.index == -2)
                        ForcedTransitionExecute = true;
                    else if (t.index == -3)
                        ForcedTransitionAttackHold = true;
                    else if (t.index == -4)
                        ForcedTransitionAttackHoldFS = true;
                }

            }
            if (!inInterval)
                animator.SetInteger (TransitionParameter.TransitionIndexer.ToString (), 0);

            if (ForcedTransitionDodge)
                animator.SetBool (TransitionParameter.ForcedTransitionDodge.ToString (), true);
            else
                animator.SetBool (TransitionParameter.ForcedTransitionDodge.ToString (), false);

            if (ForcedTransitionExecute)
                animator.SetBool (TransitionParameter.ForcedTransitionExecute.ToString (), true);
            else
                animator.SetBool (TransitionParameter.ForcedTransitionExecute.ToString (), false);

            if (ForcedTransitionAttackHold)
                animator.SetBool (TransitionParameter.ForcedTransitionAttackHold.ToString (), true);
            else
                animator.SetBool (TransitionParameter.ForcedTransitionAttackHold.ToString (), false);

            if (ForcedTransitionAttackHoldFS)
                animator.SetBool (TransitionParameter.ForcedTransitionAttackHoldFS.ToString (), true);
            else
                animator.SetBool (TransitionParameter.ForcedTransitionAttackHoldFS.ToString (), false);


        }
    }
}