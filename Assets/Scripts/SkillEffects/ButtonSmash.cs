using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ButtonSmash")]
    public class ButtonSmash : SkillEffect {

        public InputKeyType SmashKey;
        public float MaxDuration;
        public float MinDuration;
        public float MaxSmashCount = 3f;
        public float SmashCountGain = 0.5f;
        public float SmashCountDecay = 1.0f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (!animator.GetBool (TransitionParameter.ButtonSmashing.ToString())) {
                GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.BUTTON_SMASHER);
                obj.SetActive (true);
                ButtonSmasher smasher = obj.GetComponent<ButtonSmasher> ();
                smasher.Init (stateEffect.CharacterControl, SmashKey, MaxDuration, MinDuration, MaxSmashCount, SmashCountGain, SmashCountDecay);
                animator.SetBool (TransitionParameter.ButtonSmashing.ToString (), true);

            }
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) { }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) { }

    }
}