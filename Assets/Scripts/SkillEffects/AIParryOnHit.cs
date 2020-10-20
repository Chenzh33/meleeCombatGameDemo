using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "AI/AIParryOnHit")]
    public class AIParryOnHit : SkillEffect {
        public int TriggerHitCount = 2;
        public int FrameOffset = 20;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            AIProgress AI = stateEffect.CharacterControl.AIProgress;
            AI.HitCountOnGuard = 0;

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            AIProgress AI = stateEffect.CharacterControl.AIProgress;
            if (AI.HitCountOnGuard >= TriggerHitCount) {
                stateEffect.CharacterControl.CharacterData.FirstFramesOfBlock = -FrameOffset;
                AI.HitCountOnGuard = 0;

            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            AIProgress AI = stateEffect.CharacterControl.AIProgress;
            AI.HitCountOnGuard = 0;
        }

    }
}