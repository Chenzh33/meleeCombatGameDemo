using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class ButtonSmasher : MonoBehaviour {

        public CharacterControl control;
        public InputKeyType SmashKey;
        public float MaxDuration;
        public float MinDuration;
        public float MaxSmashCount;
        public float SmashCountGain;
        public float SmashCountDecay;

        [SerializeField]
        private float CurrentSmashCount;
        Coroutine routine;
        public void Init (CharacterControl c, InputKeyType key, float maxD, float minD, float maxSC, float SCG, float SCD) {
            control = c;
            SmashKey = key;
            MaxDuration = maxD;
            MinDuration = minD;
            MaxSmashCount = maxSC;
            SmashCountGain = SCG;
            SmashCountDecay = SCD;
            CurrentSmashCount = 0f;
            Smashing ();
        }

        public void Dead () {
            control.Animator.SetBool (TransitionParameter.ButtonSmashing.ToString (), false);
            PoolObject pobj = gameObject.GetComponent<PoolObject> ();
            PoolManager.Instance.ReturnToPool (pobj);

        }
        void Smashing () {
            if (routine != null)
                StopCoroutine (_Smashing ());
            routine = StartCoroutine (_Smashing ());

        }

        IEnumerator _Smashing () {
            float time = 0f;
            while (true) {
                InputsDataPerFrame inputDataTop = control.inputDataTop;
                if (inputDataTop.KeysState[(int) SmashKey * 3])
                    CurrentSmashCount += SmashCountGain;
                if (time > MinDuration) {
                    CurrentSmashCount -= SmashCountDecay * Time.deltaTime;
                    if (CurrentSmashCount <= 0 || time >= MaxDuration) {
                        Dead ();
                        yield break;
                    }
                }
                if (CurrentSmashCount > MaxSmashCount)
                    CurrentSmashCount = MaxSmashCount;
                time += Time.deltaTime;
                yield return null;
            }
        }

    }
}