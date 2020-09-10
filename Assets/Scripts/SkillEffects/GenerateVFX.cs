using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/GenerateVFX")]
    public class GenerateVFX : SkillEffect {
        [Range (0f, 3f)]
        public float GenerateVFXTiming = 0.9f;
        public float AOEAttackCenterOffset = 1.3f;

        public VFXType Type = VFXType.Slam;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {

            if (stateInfo.normalizedTime > GenerateVFXTiming && stateEffect.CharacterControl.CharacterData.VFXs.Count == 0) {
               
                GameObject obj = GenerateVFXObject ();
                obj.SetActive (true);
                Vector3 pos = stateEffect.CharacterControl.gameObject.transform.position + stateEffect.CharacterControl.gameObject.transform.forward * AOEAttackCenterOffset;
                obj.transform.position = new Vector3 (pos.x, 0f, pos.z);
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

        public GameObject GenerateVFXObject () {
            GameObject obj = null;
            switch (Type) {
                case VFXType.Slam:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.VFXSlam);
                    break;
                case VFXType.AttackHoldAOE:
                    obj = PoolManager.Instance.GetObject (PoolObjectType.VFXAttackHold);
                    break;

            }
            return obj;
        }

    }
}