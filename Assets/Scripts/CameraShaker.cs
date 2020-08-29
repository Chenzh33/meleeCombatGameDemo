using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class CameraShaker : MonoBehaviour {

        public CharacterControl Caster;
        public bool IsRegistered;
        public bool IsFinished;
        public ShakeCamera skill;

        public void Init (ShakeCamera shakeSkill, CharacterControl caster) {
            skill = shakeSkill;
            Caster = caster;
            IsRegistered = false;
            IsFinished = false;

        }

        public void Clear () {
            skill = null;
            Caster = null;
            IsRegistered = false;
            IsFinished = false;

        }

        public void Dead () {
            //CameraManager.Instance.ResetCamera ();
            Clear ();
            PoolObject pobj = this.GetComponent<PoolObject> ();
            PoolManager.Instance.ReturnToPool (pobj);
        }

        void Update () {

        }
        public void Register () {
            IsRegistered = true;
            if (!CameraManager.Instance.IsShaking ())
                CameraManager.Instance.ShakeCamera ();
        }
    }
}