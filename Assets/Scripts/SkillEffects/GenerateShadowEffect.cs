using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/GenerateShadowEffect")]
    public class GenerateShadowEffect : SkillEffect {
        [Range (0f, 3f)]
        public float TurnOnShadowTiming = 0f;
        [Range (0f, 3f)]
        public float TurnOffShadowTiming = 1f;

        public float ShadowDuration = 1f;

        public int GenerationFrameInterval = 5;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            stateEffect.CharacterControl.CharacterData.EffectShadow = 0;
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime > TurnOnShadowTiming && stateInfo.normalizedTime <= TurnOffShadowTiming) {
                if (GenerationFrameInterval == 0)
                    GenerateShadow (stateEffect, animator, stateInfo);
                else {
                    if (stateEffect.CharacterControl.CharacterData.EffectShadow == 0)
                        GenerateShadow (stateEffect, animator, stateInfo);
                    stateEffect.CharacterControl.CharacterData.EffectShadow = (stateEffect.CharacterControl.CharacterData.EffectShadow + 1) % GenerationFrameInterval;

                }
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

        }
        public void GenerateShadow (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            GameObject shadowObj = PoolManager.Instance.GetObject (PoolObjectType.Shadow);
            PoolObject shadow = shadowObj.GetComponent<PoolObject> ();
            MaterialChanger matChanger = shadowObj.GetComponent<MaterialChanger> ();

            CharacterControl player = stateEffect.CharacterControl;
            Animator shadowAnimator = shadowObj.GetComponentInChildren<Animator> ();
            shadowObj.SetActive (true);
            matChanger.TurnOffRenderers();
          
            shadowAnimator.Play (player.CharacterData.GetCurrState (), -1, stateInfo.normalizedTime);
            shadowAnimator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0f);
            shadow.WaitAndDestroy (ShadowDuration);
            matChanger.ChangeTransparency (ShadowDuration);
            shadowObj.transform.position = player.gameObject.transform.position;
            shadowObj.transform.rotation = player.gameObject.transform.rotation;
            matChanger.TurnOnRenderersDelay(0.05f);
        }

    }
}