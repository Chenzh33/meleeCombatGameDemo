using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace meleeDemo {

    public class CharacterControl : MonoBehaviour {
        CharacterController controller;
        Animator animator;
        AIProgress aiProgress;
        ManualInput manualInput;
        public ParticleSystem particleSystemTrail;
        public ParticleSystem particleSystemHold;

        public bool isPlayerControl;
        public List<Collider> RagdollParts = new List<Collider> ();
        public List<Collider> AttackingParts = new List<Collider> ();
        public Collider AttackPoint;
        public Transform SpawnPoint;
        public Transform ReflectSpawnPoint;
        public Transform Spine;
        //public List<ProjectileObject> ProjectileObjs = new List<ProjectileObject> ();
        //private List<TriggerDetector> TriggerDetectors = new List<TriggerDetector> ();
        private TriggerDetector detector;
        private Coroutine CheckStopCoroutine;
        private Coroutine TurnToTargetCoroutine;
        private Coroutine KnockbackCoroutine;
        private Coroutine HitReactCoroutine;
        private Coroutine SetFormerTargetCoroutine;
        private Coroutine TurnOffEnergyRegenCoroutine;
        private Coroutine TurnOffArmourRegenCoroutine;
        private Coroutine DodgeCoolDownCoroutine;
        private Coroutine FreezeForFramesCoroutine;

        public AnimationCurve KnockbackSpeedGraph;
        public Vector3 FaceTarget;

        // Command derived from ManualInput or AIProgress
        public Vector2 inputVector = new Vector2 ();
        public bool[] inputKeyStates = new bool[12];
        public bool CommandAttack;
        public bool CommandExecute;
        public bool CommandDodge;
        public bool CommandCharge;
        public bool CommandGuard;
        public bool CommandFire;
        public int CommandAttackHoldFrame;
        public int CommandExecuteHoldFrame;
        public int CommandDodgeHoldFrame;
        public bool CommandGuardHoldOn;
        public int InputAxisHoldFrame;
        //public float SpeedMultiplyer = 1.0f;

        //[System.Serializable]
        public CharacterData data = new CharacterData ();

        /*
                public bool AttackTrigger;
                public bool ExecuteTrigger;
                public bool DodgeTrigger;
                */

        void Awake () {
            Init ();
        }
        public void Init () {
            animator = GetComponentInChildren<Animator> ();
            detector = GetComponentInChildren<TriggerDetector> ();
            controller = GetComponent<CharacterController> ();
            aiProgress = GetComponent<AIProgress> ();
            manualInput = GetComponent<ManualInput> ();

            ParticleSystemTag[] particleSystemTags = GetComponentsInChildren<ParticleSystemTag> ();
            foreach (ParticleSystemTag p in particleSystemTags) {
                if (p.tag == VFXType.Trail) {
                    particleSystemTrail = p.GetComponent<ParticleSystem> ();
                    particleSystemTrail.Pause (true);
                    particleSystemTrail.Clear ();
                } else if (p.tag == VFXType.Hold) {
                    particleSystemHold = p.GetComponent<ParticleSystem> ();
                    particleSystemHold.Pause (true);
                    particleSystemHold.Clear ();
                }

            }

            if (manualInput != null)
                isPlayerControl = true;
            else
                isPlayerControl = false;
            // load data process xxxx
            SetRagdollAndAttackingParts ();
            //this.CharacterData.OnDead += Dead;
            //this.CharacterData.OnDamage += Dead;
            if (aiProgress != null)
                aiProgress.enabled = false;
            if (manualInput != null)
                manualInput.enabled = false;

        }

        /*
                IEnumerator _Spawn () {
                    this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, 10f, this.gameObject.transform.position.z);
                    this.Animator.Play ("Spawn");
                    this.gameObject.transform.DOMoveY (0f, 0.1f);
                }
                */
        public void Spawn () {
            //StartCoroutine (_Spawn ());
            this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, 10f, this.gameObject.transform.position.z);
            this.Animator.Play ("Spawn");
            this.gameObject.transform.DOMoveY (0f, 0.2f);
        }

        /*
                void Start () {

                }
                */

        public Animator Animator {
            get {
                return animator;
            }
        }
        public CharacterController CharacterController {
            get {
                return controller;
            }
        }
        public CharacterData CharacterData {
            get {
                return data;
            }
        }
        public AIProgress AIProgress {
            get {
                return aiProgress;
            }
        }
        public TriggerDetector GetTriggerDetector () {
            return detector;

        }
        public ParticleSystem VFXTrail {
            get {
                return particleSystemTrail;
            }
        }
        public ParticleSystem VFXHold {
            get {
                return particleSystemHold;
            }
        }
        IEnumerator _TurnOffEnergyRegen (float delay) {
            this.CharacterData.OffEnergyRegen = true;
            yield return new WaitForSeconds (delay);
            this.CharacterData.OffEnergyRegen = false;

            TurnOffEnergyRegenCoroutine = null;

        }

        public void TurnOffEnergyRegen (float delay) {
            if (TurnOffEnergyRegenCoroutine != null)
                StopCoroutine (TurnOffEnergyRegenCoroutine);
            TurnOffEnergyRegenCoroutine = StartCoroutine (_TurnOffEnergyRegen (delay));

        }
        IEnumerator _TurnOffArmourRegen (float delay) {
            this.CharacterData.OffArmourRegen = true;
            yield return new WaitForSeconds (delay);
            this.CharacterData.OffArmourRegen = false;

            TurnOffArmourRegenCoroutine = null;

        }

        public void TurnOffArmourRegen (float delay) {
            if (TurnOffArmourRegenCoroutine != null)
                StopCoroutine (TurnOffArmourRegenCoroutine);
            TurnOffArmourRegenCoroutine = StartCoroutine (_TurnOffArmourRegen (delay));

        }
        IEnumerator _DodgeCoolDown (float cd) {
            this.Animator.SetBool (TransitionParameter.ForbidDodge.ToString (), true);
            yield return new WaitForSeconds (cd);
            this.Animator.SetBool (TransitionParameter.ForbidDodge.ToString (), false);

            DodgeCoolDownCoroutine = null;

        }

        public void DodgeCoolDown () {
            if (DodgeCoolDownCoroutine != null)
                StopCoroutine (DodgeCoolDownCoroutine);
            DodgeCoolDownCoroutine = StartCoroutine (_DodgeCoolDown (this.CharacterData.DodgeCoolDown));

        }

        public void TakeStun (float stun, SkillEffect skill) {

            this.CharacterData.Armour -= stun;
            //if (hitReactionTime > 0f && this.CharacterData.GetHitTime < hitReactionTime && !this.CharacterData.IsSuperArmour && !this.CharacterData.IsDead)
            //   this.CharacterData.GetHitTime = hitReactionTime;

            TurnOffArmourRegen (this.CharacterData.ArmourRegenerationDelay);

            if (this.CharacterData.Armour <= 0 && !this.CharacterData.IsDead)
                GetStunned ();

        }

        public void TakeDamage (float damage, float hitReactionTime, SkillEffect skill) {
            bool CanBeBlocked = (this.CharacterData.IsGuarding && this.CharacterData.BlockCount >= damage);
            if (!this.CharacterData.IsStunned && !this.CharacterData.IsSuperArmour && !this.CharacterData.IsDead && !CanBeBlocked) {
                if (hitReactionTime > 0f && this.CharacterData.GetHitTime < hitReactionTime)
                    this.CharacterData.GetHitTime = hitReactionTime;
                if (isPlayerControl) {
                    this.Animator.Play ("HitReact4", 0, 0f);
                } else {
                    if (this.CharacterData.IsGrappled && ((DirectDamage) skill).Type == GrapplerType.DownStab)
                        this.Animator.Play ("HitReact_Executed", 0, 0f);
                    else {
                        int randomIndex = Random.Range (0, 3) + 1;
                        this.Animator.Play ("HitReact" + randomIndex.ToString (), 0, 0f);
                    }
                }
            }

            this.CharacterData.TakeDamage (damage);
            this.CharacterData.SendGetDamageEvent (skill, this);
            if (this.CharacterData.HP <= 0)
                Dead (skill);

            if (this.aiProgress != null) {
                this.aiProgress.UpdateFearState ();

            }

            //TurnOffArmourRegen (this.CharacterData.ArmourRegenerationDelay);

            //this.CharacterData.OnDead (skill);
        }

        public void TakeEnergy (float energy, SkillEffect skill) {

            // assume already checked the energy amount
            /*
            if (energy > this.CharacterData.Energy) {
                return;
            }
            */
            if (energy > 0f) {
                TurnOffEnergyRegen (this.CharacterData.EnergyRegenerationDelay);
                this.CharacterData.TakeEnergy (energy);
            }
        }

        public void OnEnemyGetDamaged (SkillEffect skill, CharacterControl enemy) {
            //Debug.Log (skill.GetType ().ToString ());
            if (enemy.CharacterData.HP <= 0f) {

                this.CharacterData.CurrentEnergyUnitChargeToFull ();
                if (skill.GetType ().ToString () == "meleeDemo.DirectDamage") {
                    this.CharacterData.GetEnergy (this.CharacterData.EnergyGetOnEnemyDeathByExecute);
                } else if (skill.GetType ().ToString () == "meleeDemo.Attack") {
                    if (((Attack) skill).IsLethalToStunnedEnemy) {
                        this.CharacterData.GetEnergy (this.CharacterData.EnergyGetOnEnemyDeathByExecute);
                    }
                }
            } else {
                if (skill.GetType ().ToString () == "meleeDemo.DirectDamage") {

                    float EnergyGet = ((DirectDamage) skill).EnergyGetWhenHit;
                    if (EnergyGet > 0)
                        this.CharacterData.GetEnergyToMaxOneUnit (EnergyGet);
                } else if (skill.GetType ().ToString () == "meleeDemo.Attack") {
                    float EnergyGet = ((Attack) skill).EnergyGetWhenHit;
                    if (EnergyGet > 0)
                        this.CharacterData.GetEnergyToMaxOneUnit (EnergyGet);
                    this.SetFormerTarget (enemy, 1f);
                }
            }

        }
        public void FreezeForFrames (int freezeFrames) {
            if (FreezeForFramesCoroutine != null)
                StopCoroutine (FreezeForFramesCoroutine);
            FreezeForFramesCoroutine = StartCoroutine (_FreezeForFrames (freezeFrames));
        }
        IEnumerator _FreezeForFrames (int freezeFrames) {
            int f = 0;
            this.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0f);
            while (true) {
                ++f;
                if (f > freezeFrames) {
                    this.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1f);
                    yield break;
                }
                yield return new WaitForFixedUpdate ();
            }

        }
        public void HitReactionAndFreeze (float freezeStTime, GrapplerType type) {
            if (HitReactCoroutine != null)
                StopCoroutine (HitReactCoroutine);
            HitReactCoroutine = StartCoroutine (_HitReactionAndFreeze (freezeStTime, type));
        }

        IEnumerator _HitReactionAndFreeze (float freezeStTime, GrapplerType type) {
            //this.CharacterData.GetHitTime = 0.5f;
            //int randomIndex = Random.Range (0, 3) + 1;
            //int randomIndex = 1;
            if (type == GrapplerType.FrontStab)
                this.Animator.Play ("GrapplingHitFrontStab", 0, 0f);
            else if (type == GrapplerType.DownStab) {
                if (this.CharacterData.IsStunned)
                    this.Animator.Play ("GrapplingHitDownStabStunned", 0, 0f);
                else
                    this.Animator.Play ("GrapplingHitDownStab", 0, 0f);

            }

            float t = 0f;
            while (true) {
                if (t > freezeStTime) {
                    /*
                    if (t > freezeStTime && t <= freezeStTime + freezeTime) {
                            this.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0f);
                        if (t > freezeStTime + freezeTime) {
                            */
                    this.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0f);
                    yield break;
                }
                t = t + Time.deltaTime;
                //this.CharacterData.GetHitTime = 0.5f;
                yield return null;
            }

        }
        /*
        public void MoveCharacter()
        {

        }
        */
        public void SetFormerTarget (CharacterControl target, float duration) {
            if (SetFormerTargetCoroutine != null)
                StopCoroutine (SetFormerTargetCoroutine);

            if (target != null)
                SetFormerTargetCoroutine = StartCoroutine (_SetFormerTarget (target, duration));
            else
                this.CharacterData.FormerAttackTarget = null;
        }

        IEnumerator _SetFormerTarget (CharacterControl target, float duration) {
            this.Animator.SetBool (TransitionParameter.LockOnEnemy.ToString (), true);
            this.CharacterData.FormerAttackTarget = target;
            float t = 0f;
            while (t < duration) {
                t += Time.deltaTime;
                //Debug.Log(t);
                yield return null;
            }
            this.CharacterData.FormerAttackTarget = null;
            SetFormerTargetCoroutine = null;
            this.Animator.SetBool (TransitionParameter.LockOnEnemy.ToString (), false);
        }

        public void TakeKnockback (Vector3 knockbackVector, float duration) {
            if (KnockbackCoroutine != null)
                StopCoroutine (KnockbackCoroutine);
            KnockbackCoroutine = StartCoroutine (_TakeKnockback (knockbackVector, duration));
        }

        IEnumerator _TakeKnockback (Vector3 knockbackVector, float duration) {
            float t = 0f;
            while (t < duration) {
                this.CharacterController.Move (knockbackVector * this.Animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * KnockbackSpeedGraph.Evaluate (t / duration) * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }
            KnockbackCoroutine = null;
        }
        /*
        public List<TriggerDetector> GetAllTriggers () {
            if (TriggerDetectors.Count == 0) {
                TriggerDetector[] triggers = this.gameObject.GetComponentsInChildren<TriggerDetector> ();
                foreach (TriggerDetector t in triggers) {
                    TriggerDetectors.Add (t);
                }

            }
            return TriggerDetectors;
        }
        */

        public void TurnOffCollider () {
            data.IsColliderOff = true;
            /*
            foreach (Collider c in RagdollParts) {
                c.isTrigger = true;
            }
            */
            controller.enabled = false;
        }
        public void TurnOnCollider () {
            data.IsColliderOff = false;
            /*
            foreach (Collider c in RagdollParts) {
                c.isTrigger = false;
            }
            */

            controller.enabled = true;
        }
        public void TurnOnRagdoll () {
            //Rigidbody rig = GetComponent<Rigidbody>();
            //rig.useGravity = false;
            //animator.enabled = false;

            controller.enabled = false;
            this.CharacterData.IsRagdollOn = true;
            foreach (Collider c in RagdollParts) {
                //c.isTrigger = false;
                c.attachedRigidbody.velocity = Vector3.zero;
                c.attachedRigidbody.useGravity = false;
            }

            //animator.enabled = false;

        }

        public void Dead (SkillEffect skill) {
            //TurnOnRagdoll ();
            int randomIndex = Random.Range (0, 2) + 1;
            if (isPlayerControl)
                this.Animator.Play ("Frank_Dead" + randomIndex.ToString (), 0, 0f);
            else {
                if (this.CharacterData.IsGrappled && ((DirectDamage) skill).Type == GrapplerType.DownStab) {
                    this.Animator.Play ("Ybot_Dead_Executed", 0, 0f);
                } else
                    this.Animator.Play ("Ybot_Dead" + randomIndex.ToString (), 0, 0f);
            }
            data.IsDead = true;

            gameObject.layer = LayerMask.NameToLayer ("Default");

            AIProgress agent = GetComponent<AIProgress> ();
            if (agent != null) {
                agent.Dead ();
                AIAgentManager.Instance.TotalAIAgent.Remove (agent);
            }

        }
        public void DestroyObject () {
            if (isPlayerControl)
                GameManager.Instance.GameOver ();
            else
                Destroy (this.gameObject);

        }

        public void GetStunned () {

            AIProgress agent = GetComponent<AIProgress> ();
            if (agent != null) {
                agent.StopMove ();
                //AIAgentManager.Instance.TotalAIAgent.Remove (agent);
            }

            if (isPlayerControl) {
                this.Animator.Play ("Stun", 0, 0f);
            } else {
                this.Animator.Play ("Stun", 0, 0f);
                this.CharacterData.IsStunned = true;
                this.Animator.SetBool (TransitionParameter.Stunned.ToString (), true);
            }
            this.CharacterData.GetHitTime = 0f;

        }

        private void SetRagdollAndAttackingParts () {
            RagdollParts.Clear ();
            AttackingParts.Clear ();
            Collider[] cols = this.gameObject.GetComponentsInChildren<Collider> ();
            foreach (Collider c in cols) {
                if (c.gameObject != this.gameObject) {
                    if (c.GetComponent<Rigidbody> () != null) {
                        /*
                        if (c.GetComponent<TriggerDetector> () == null)
                            c.gameObject.AddComponent<TriggerDetector> ();
                        */
                        if (c.isTrigger) {
                            AttackingParts.Add (c);
                        } else {
                            RagdollParts.Add (c);
                            c.isTrigger = true;
                        }
                    }
                    if (c.GetComponent<WeaponAttackPoint> () != null) {
                        AttackPoint = c;
                    }
                }
            }

        }

        public List<Collider> GetAttackingPart () {
            return AttackingParts;

        }
        public Collider GetAttackPoint () {
            return AttackPoint;

        }

        public Transform GetSpine () {
            if (Spine == null) {
                SpineTag s = this.gameObject.GetComponentInChildren<SpineTag> ();
                Spine = s.gameObject.transform;
            }
            return Spine;
        }

        public Transform GetProjectileSpawnPoint () {
            if (SpawnPoint == null) {
                ProjectSpawnPoint p = this.gameObject.GetComponentInChildren<ProjectSpawnPoint> ();
                SpawnPoint = p.gameObject.transform;
            }
            return SpawnPoint;
        }
        public Transform GetReflectProjSpawnPoint () {
            if (ReflectSpawnPoint == null) {
                ReflectProjSpawnPoint p = this.gameObject.GetComponentInChildren<ReflectProjSpawnPoint> ();
                ReflectSpawnPoint = p.gameObject.transform;
            }
            return ReflectSpawnPoint;
        }

        /*
        public List<ProjectileObject> GetProjectileObjs() {
            return ProjectileObjs;

        }
        */
        /*
                public void MoveForward (Vector3 dir, float s, float sgraph) {
                    //m_Controller.Move(moveDirection * s * sgraph * Time.deltaTime);
                    controller.Move (dir * s * sgraph * Time.deltaTime);
                }
                */
        IEnumerator _TurnToTarget (float stTime, float smooth, Quaternion target) {

            float t = 0f;
            while (Quaternion.Angle (animator.transform.root.rotation, target) > 0.1f) {
                if (t >= stTime)
                    animator.transform.root.rotation = Quaternion.Slerp (animator.transform.root.rotation, target, smooth * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }
            TurnToTargetCoroutine = null;
        }

        public void StopTurnToTarget () {
            if (TurnToTargetCoroutine != null)
                StopCoroutine (TurnToTargetCoroutine);
            FaceTarget = gameObject.transform.forward;

        }

        public void TurnToTarget (float stTime, float smooth) {
            if (TurnToTargetCoroutine != null)
                StopCoroutine (TurnToTargetCoroutine);
            /*
            float angle = Mathf.Acos (Vector3.Dot (new Vector3 (0, 0, 1), FaceTarget)) * Mathf.Rad2Deg;
            if (FaceTarget.x < 0.0f) { angle = -angle; }
            Quaternion target = Quaternion.Euler (new Vector3 (0, angle, 0));
            */
            Quaternion target = Quaternion.LookRotation (FaceTarget, Vector3.up);
            if (smooth == 0f)
                animator.transform.root.rotation = target;
            else //if (TurnToTargetCoroutine == null)
                TurnToTargetCoroutine = StartCoroutine (_TurnToTarget (stTime, smooth, target));
        }

        IEnumerator _CheckStopMove (float time) {
            yield return new WaitForSeconds (time);
            if (inputVector.magnitude <= 0.01f) {
                animator.SetBool (TransitionParameter.Move.ToString (), false);
            }
            CheckStopCoroutine = null;
        }
        public void CheckStopMove (float time) {
            //if (CheckStopCoroutine != null)
            //StopCoroutine (CheckStopCoroutine);
            if (CheckStopCoroutine == null)
                CheckStopCoroutine = StartCoroutine (_CheckStopMove (time));

        }
        /*
        public bool CheckInTransitionBetweenSameClips(Animator animator)
        {

        }
        */

        void Update () {

            /*
            hInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");
            //Debug.Log(hInput);
            //Debug.Log(vInput);
            */

            /*
            Vector2 inputVector = inputData.InputVector;
            bool[] inputKeysState = inputData.KeysState;

            //moveDirection = Vector3.ClampMagnitude(moveDirection, 1);
            stateinfo = animator.GetCurrentAnimatorStateInfo (0);
            //if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            if (inputVector.magnitude > 0f) {
                animator.SetBool (TransitionParameter.Move.ToString (), true);
                if (stateinfo.IsName ("Run")) {
                    moveDirection = new Vector3 (inputVector.x, 0, inputVector.y);
                    MoveForward (moveDirection, speed, 1.0f);
                    float angle = Mathf.Acos (Vector3.Dot (new Vector3 (0, 0, 1), moveDirection)) * Mathf.Rad2Deg;
                    if (inputVector.x < 0.0f) { angle = -angle; }
                    Quaternion target = Quaternion.Euler (new Vector3 (0, angle, 0));
                    transform.localRotation = Quaternion.Slerp (transform.localRotation, target, Time.deltaTime * smooth);
                }
            } else {
                animator.SetBool (TransitionParameter.Move.ToString (), false);
            }

            */
            //Debug.Log(stateinfo.IsName("Run"));
            /*
             if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2")) { times = Mathf.Ceil(stateinfo.normalizedTime); }
             if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
             {
                 if (stateinfo.IsName("Idle") || stateinfo.IsName("Run") || stateinfo.normalizedTime > times)
                 {
                     if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                     {
                         float h = Input.GetAxis("Horizontal");
                         float v = Input.GetAxis("Vertical");
                         //椭圆映射
                         float x = h * Mathf.Sqrt(1 - (v * v) / 2.0f);
                         float z = v * Mathf.Sqrt(1 - (h * h) / 2.0f);
                         moveDirection = new Vector3(x, 0, z);
                         m_Controller.Move(moveDirection * speed * Time.deltaTime);
                         //获得角度
                         float angle = Mathf.Acos(Vector3.Dot(new Vector3(0, 0, 1), moveDirection)) * Mathf.Rad2Deg;
                         //识别z轴两侧方向
                         if (h < 0.0f) { angle = -angle; }
                         //平滑转向
                         Quaternion target = Quaternion.Euler(new Vector3(0, angle, 0));
                         transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * smooth);
                     }
                 }
             }
             */

            /* 
             if (!this.CharacterController.isGrounded) {
                 Vector3 gravity = new Vector3 (0, -9.8f * Time.deltaTime, 0);
                 this.CharacterController.Move (gravity);
             }
             */

            if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Move") ||
                animator.GetCurrentAnimatorStateInfo (0).IsName ("Walk") ||
                animator.GetCurrentAnimatorStateInfo (0).IsName ("Run") ||
                animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle"))
                animator.SetBool (TransitionParameter.IdleState.ToString (), true);
            else
                animator.SetBool (TransitionParameter.IdleState.ToString (), false);

            this.CharacterData.UpdateData ();

            if (inputVector.magnitude > 0.01f) {
                animator.SetBool (TransitionParameter.Move.ToString (), true);
            } else {
                //animator.SetBool (TransitionParameter.Move.ToString (), false);

                if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Move") ||
                    animator.GetCurrentAnimatorStateInfo (0).IsName ("Walk") ||
                    animator.GetCurrentAnimatorStateInfo (0).IsName ("Run")) {
                    //Debug.Log("check stop");
                    CheckStopMove (0.05f);
                }

            }
            if (CommandAttack)
                animator.SetBool (TransitionParameter.AttackMelee.ToString (), true);
            else
                animator.SetBool (TransitionParameter.AttackMelee.ToString (), false);

            if (CommandExecute)
                animator.SetBool (TransitionParameter.AttackExecute.ToString (), true);
            else
                animator.SetBool (TransitionParameter.AttackExecute.ToString (), false);

            if (CommandDodge)
                animator.SetBool (TransitionParameter.Dodge.ToString (), true);
            else
                animator.SetBool (TransitionParameter.Dodge.ToString (), false);

            if (isPlayerControl) {

                if (CommandCharge)
                    animator.SetBool (TransitionParameter.Charge.ToString (), true);
                else
                    animator.SetBool (TransitionParameter.Charge.ToString (), false);

                if (CommandGuard)
                    animator.SetBool (TransitionParameter.Guard.ToString (), true);
                else
                    animator.SetBool (TransitionParameter.Guard.ToString (), false);

                if (CommandGuardHoldOn)
                    animator.SetBool (TransitionParameter.GuardHoldOn.ToString (), true);
                else
                    animator.SetBool (TransitionParameter.GuardHoldOn.ToString (), false);

                if (CommandAttackHoldFrame > 8)
                    animator.SetBool (TransitionParameter.AtkButtonHold.ToString (), true);
                else
                    animator.SetBool (TransitionParameter.AtkButtonHold.ToString (), false);

                if (CommandAttackHoldFrame < 0)
                    animator.SetInteger (TransitionParameter.AtkReleaseTiming.ToString (), -CommandAttackHoldFrame);
                else
                    animator.SetInteger (TransitionParameter.AtkReleaseTiming.ToString (), 0);

                if (CommandExecuteHoldFrame > 5)
                    animator.SetBool (TransitionParameter.ExcButtonHold.ToString (), true);
                else
                    animator.SetBool (TransitionParameter.ExcButtonHold.ToString (), false);

                if (CommandExecuteHoldFrame < 0)
                    animator.SetInteger (TransitionParameter.ExcReleaseTiming.ToString (), -CommandExecuteHoldFrame);
                else
                    animator.SetInteger (TransitionParameter.ExcReleaseTiming.ToString (), 0);

                if (CommandDodgeHoldFrame > 5)
                    animator.SetBool (TransitionParameter.DdgButtonHold.ToString (), true);
                else
                    animator.SetBool (TransitionParameter.DdgButtonHold.ToString (), false);

                if (CommandDodgeHoldFrame < 0)
                    animator.SetInteger (TransitionParameter.DdgReleaseTiming.ToString (), -CommandDodgeHoldFrame);
                else
                    animator.SetInteger (TransitionParameter.DdgReleaseTiming.ToString (), 0);

                if (InputAxisHoldFrame > 5)
                    animator.SetBool (TransitionParameter.MoveHold.ToString (), true);
                else
                    animator.SetBool (TransitionParameter.MoveHold.ToString (), false);

            } else {
                if (CommandFire)
                    animator.SetBool (TransitionParameter.Fire.ToString (), true);
                else
                    animator.SetBool (TransitionParameter.Fire.ToString (), false);

            }
            /*
                        if (DodgeTrigger) {
                            DodgeTrigger = false;
                        }
                        if (AttackTrigger) {
                            AttackTrigger = false;
                        }
                        if (ExecuteTrigger) {
                            ExecuteTrigger = false;
                        }
                        */

        }
    }

}