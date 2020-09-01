using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/TurnOnTrail")]
    public class TurnOnTrail : SkillEffect {
        [Range(0f, 3f)]
        public float TurnOnTrailTiming = 0.9f;
        [Range(0f, 3f)]
        public float TurnOffTrailTiming = 3f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime > TurnOnTrailTiming && stateInfo.normalizedTime <= TurnOffTrailTiming && stateEffect.CharacterControl.ParticleSystem.isPaused) {
                stateEffect.CharacterControl.ParticleSystem.Play(true);
            }
            if (stateInfo.normalizedTime > TurnOffTrailTiming && !stateEffect.CharacterControl.ParticleSystem.isPaused) {
                stateEffect.CharacterControl.ParticleSystem.Pause(true);
                stateEffect.CharacterControl.ParticleSystem.Clear();
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (!stateEffect.CharacterControl.ParticleSystem.isPaused)
            {
                stateEffect.CharacterControl.ParticleSystem.Pause(true);
                stateEffect.CharacterControl.ParticleSystem.Clear();
            }
        }

    }
}