using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/GenerateVFX")]
    public class GenerateVFX : SkillEffect {
        [Range (0f, 3f)]
        public float GenerateVFXTiming = 0.9f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime > GenerateVFXTiming && stateEffect.CharacterControl.CharacterData.VFXs.Count == 0) {
                GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.SLAM_VFX);
                obj.SetActive (true);
                obj.transform.position = new Vector3 (stateEffect.CharacterControl.GetProjectileSpawnPoint().transform.position.x, 0f, stateEffect.CharacterControl.GetProjectileSpawnPoint().transform.position.z);
                ParticleSystem ps = obj.GetComponent<ParticleSystem> ();
                ps.Play (true);
                stateEffect.CharacterControl.CharacterData.VFXs.Add (obj.GetComponent<PoolObject> ());
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            foreach (PoolObject p in stateEffect.CharacterControl.CharacterData.VFXs) {
                ParticleSystem ps = p.GetComponent<ParticleSystem> ();
                ps.Clear ();
                ps.Stop (true);
                PoolManager.Instance.ReturnToPool (p);
            }
            stateEffect.CharacterControl.CharacterData.VFXs.Clear ();
        }

    }
}