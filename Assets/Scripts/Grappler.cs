using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class Grappler : MonoBehaviour {
        public GrapplingSkill Skill;
        public bool IsRegistered;
        public bool IsFinished;
        public int CurrentTargetNum;
        public GrapplerType Type;
        public float Range;
        public float Damage;
        public float Stun;
        public float FreezeStartTiming;
        public float DamageDelay;
        public CharacterControl Attacker;
        public CharacterControl Target;
        Coroutine CheckCompleteCoroutine;
        public float MaxGrapplingTime = 3f;

        public void Init (GrapplingSkill grapplingSkill, CharacterControl attacker) {
            Skill = grapplingSkill;
            IsRegistered = false;
            IsFinished = false;
            CurrentTargetNum = 0;
            Type = grapplingSkill.Type;
            Range = grapplingSkill.Range;
            Damage = grapplingSkill.Damage;
            Stun = grapplingSkill.Stun;
            FreezeStartTiming = grapplingSkill.FreezeStartTiming;
            Attacker = attacker;
            Target = null;
            CheckCompleteCoroutine = null;
        }

        public void Clear () {
            Skill = null;
            IsRegistered = false;
            IsFinished = false;
            CurrentTargetNum = 0;
            Attacker = null;
            Target = null;
            Range = 0f;
            CheckCompleteCoroutine = null;
        }
  
        IEnumerator _CheckComplete () {
            float t = 0f;
            while (true) {
                if (!Attacker.Animator.GetBool (TransitionParameter.GrapplingHit.ToString ()) && !IsFinished) {
                    //Target.CharacterData.IsGrappled = false;
                    //Target.CharacterData.GetHitTime = 0.5f;
                    Dead ();
                    Debug.Log("check complete...");
                    yield break;
                }
                t += Time.deltaTime;
                if(t > MaxGrapplingTime)
                {
                    Dead();
                    yield break;
                }
                yield return null;
            }

        }
        void Start () { }

        void Update () {

        }
        public void Register () {
            IsRegistered = true;

        }
        public void GrapplingHit () {
            Attacker.Animator.SetBool (TransitionParameter.GrapplingHit.ToString (), true);

            Attacker.CharacterData.GrapplingTarget = Target;
            if (CheckCompleteCoroutine != null)
                StopCoroutine (CheckCompleteCoroutine);
            CheckCompleteCoroutine = StartCoroutine (_CheckComplete ());
        }

        public void Dead () {
            //Attacker.Animator.SetBool (TransitionParameter.GrapplingHit.ToString (), false);

            Attacker.CharacterData.GrapplingTarget.gameObject.transform.parent = null;
            Target.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1.0f);
            Target.CharacterData.IsGrappled = false;
            Attacker.Animator.SetBool(TransitionParameter.GrapplingHit.ToString(), false);
            IsFinished = true;
            IsRegistered = false;
            if (AttackManager.Instance.CurrentGrappler.Contains (this))
                AttackManager.Instance.CurrentGrappler.Remove (this);
            PoolObject pobj = gameObject.GetComponent<PoolObject> ();
            PoolManager.Instance.ReturnToPool (pobj);
            this.Clear ();
            this.enabled = false;
        }
    }
}