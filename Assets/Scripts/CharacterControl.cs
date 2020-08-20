using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class CharacterControl : MonoBehaviour {
        public InputsDataPerFrame inputDataTop = new InputsDataPerFrame ();
        public int[] keysHoldFrames = new int[4];
        public bool isPlayerControl;
        public List<Collider> RagdollParts = new List<Collider> ();
        public List<Collider> AttackingParts = new List<Collider> ();
        //public List<ProjectileObject> ProjectileObjs = new List<ProjectileObject> ();
        //private List<TriggerDetector> TriggerDetectors = new List<TriggerDetector> ();
        private TriggerDetector detector;
        private Coroutine CheckStopCoroutine;
        private Coroutine TurnToTargetCoroutine;
        private Coroutine KnockbackCoroutine;
        public Vector3 FaceTarget;
        //public float SpeedMultiplyer = 1.0f;

        //[System.Serializable]
        public CharacterData data = new CharacterData ();

        CharacterController controller;
        Animator animator;

        //private float hInput;
        //private float vInput;
        //private Vector3 moveDirection = Vector3.zero;
        public bool AttackTrigger;
        public bool DodgeTrigger;
        //public int AttackHoldReleaseFrame;
        //public int ExecuteHoldReleaseFrame;

        void Awake () {
            animator = GetComponentInChildren<Animator> ();
            detector = GetComponentInChildren<TriggerDetector> ();
            controller = GetComponent<CharacterController> ();
            // load data process xxxx
            SetRagdollAndAttackingParts ();
        }

        void Start () {

        }

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
        public TriggerDetector GetTriggerDetector () {
            return detector;

        }

        public void TakeDamage (float damage) {
            this.CharacterData.HP -= damage;

            if (this.CharacterData.HP <= 0)
                Dead ();
        }

        public void TakeKnockback (Vector3 knockbackVector, float duration) {
            if (KnockbackCoroutine == null)
                KnockbackCoroutine = StartCoroutine (_TakeKnockback (knockbackVector, duration));
        }

        IEnumerator _TakeKnockback (Vector3 knockbackVector, float duration) {
            float t = 0f;
            while (t < duration) {
                this.CharacterController.Move (knockbackVector * Time.deltaTime);
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

        public void TurnOffCollider() {
            data.IsColliderOff = true;
            foreach (Collider c in RagdollParts) {
                c.isTrigger = true;
            }
        }
        public void TurnOnCollider() {
            data.IsColliderOff = false;
            foreach (Collider c in RagdollParts) {
                c.isTrigger = false;
            }
        }
        private void TurnOnRagdoll () {
            //Rigidbody rig = GetComponent<Rigidbody>();
            //rig.useGravity = false;
            animator.enabled = false;
            foreach (Collider c in RagdollParts) {
                c.isTrigger = false;
                c.attachedRigidbody.velocity = Vector3.zero;
            }

        }

        public void Dead () {
            TurnOnRagdoll ();
            data.IsDead = true;

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
                }
            }

        }

        public List<Collider> GetAttackingPart () {
            return AttackingParts;

        }
        public Transform GetProjectileSpawnPoint () {

            ProjectSpawnPoint p = this.gameObject.GetComponentInChildren<ProjectSpawnPoint> ();
            return p.gameObject.transform;
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
            while (Quaternion.Angle (animator.transform.localRotation, target) > 0.1f) {
                if (t >= stTime)
                    animator.transform.localRotation = Quaternion.Slerp (animator.transform.localRotation, target, smooth * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }
            TurnToTargetCoroutine = null;
        }

        public void TurnToTarget (float stTime, float smooth) {
            float angle = Mathf.Acos (Vector3.Dot (new Vector3 (0, 0, 1), FaceTarget)) * Mathf.Rad2Deg;
            if (FaceTarget.x < 0.0f) { angle = -angle; }
            Quaternion target = Quaternion.Euler (new Vector3 (0, angle, 0));
            if (smooth == 0f)
                animator.transform.localRotation = target;
            else if (TurnToTargetCoroutine == null)
                TurnToTargetCoroutine = StartCoroutine (_TurnToTarget (stTime, smooth, target));
        }

        IEnumerator _CheckStopMove (float time) {
            yield return new WaitForSeconds (time);
            Vector2 inputVector = inputDataTop.InputVector;
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
            if (!this.CharacterController.isGrounded) {
                Vector3 gravity = new Vector3 (0, -9.8f * Time.deltaTime, 0);
                this.CharacterController.Move (gravity);
            }

            /*
                        if (isPlayerControll) {

                            if (inputDataTop.InputVector.magnitude > 0.01f) {
                                animator.SetBool (TransitionParameter.Move.ToString (), true);
                            } else {
                                if (animator.GetCurrentAnimatorStateInfo(0).IsName ("Move")) {
                                    CheckStopMove (0.10f);
                                }
                            }
                            if (inputDataTop.KeysState[(int) InputKeyStateType.KEY_MELEE_ATTACK_DOWN]) {
                                animator.SetBool (TransitionParameter.AttackMelee.ToString (), true);
                                //AttackTrigger = true;
                            } else {
                                animator.SetBool (TransitionParameter.AttackMelee.ToString (), false);
                                //AttackTrigger = false;
                            }
                            if (inputDataTop.KeysState[(int) InputKeyStateType.KEY_DODGE_DOWN]) {
                                animator.SetBool (TransitionParameter.Dodge.ToString (), true);
                                //DodgeTrigger = true;
                            } else {
                                animator.SetBool (TransitionParameter.Dodge.ToString (), false);
                                //DodgeTrigger = false;
                            }

                        }
                        */

            if (isPlayerControl) {
                // for test
                if (Input.GetKeyDown (KeyCode.B)) {
                    if (animator.GetFloat ("SpeedMultiplier") == 1.0f)
                        animator.SetFloat ("SpeedMultiplier", 0.1f);
                    else
                        animator.SetFloat ("SpeedMultiplier", 1f);
                }
                inputDataTop = VirtualInputManager.Instance.GetTopInput ();

                if (inputDataTop.InputVector.magnitude > 0.01f) {
                    animator.SetBool (TransitionParameter.Move.ToString (), true);
                } else {
                    if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Move")) {
                        CheckStopMove (0.10f);
                    }
                }
                if (VirtualInputManager.Instance.CheckCommandInput ()) {
                    if (VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_MELEE_ATTACK_DOWN))
                        animator.SetBool (TransitionParameter.AttackMelee.ToString (), true);
                    if (VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_DODGE_DOWN))
                        animator.SetBool (TransitionParameter.Dodge.ToString (), true);
                }

                if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_MELEE_ATTACK_DOWN))
                    animator.SetBool (TransitionParameter.AttackMelee.ToString (), false);
                if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_DODGE_DOWN))
                    animator.SetBool (TransitionParameter.Dodge.ToString (), false);

                if (DodgeTrigger) {
                    //VirtualInputManager.Instance.ClearInputInBuffer (InputKeyStateType.KEY_DODGE_DOWN);
                    VirtualInputManager.Instance.ClearAllInputsInBuffer ();
                }
                if (AttackTrigger) {
                    //VirtualInputManager.Instance.ClearInputInBuffer (InputKeyStateType.KEY_MELEE_ATTACK_DOWN);
                    VirtualInputManager.Instance.ClearAllInputsInBuffer ();
                }

                keysHoldFrames = VirtualInputManager.Instance.GetKeysHoldFrames ();
                if (keysHoldFrames[0] < 0) {
                    animator.SetInteger (TransitionParameter.AtkReleaseTiming.ToString (), -keysHoldFrames[0]);
                    //Debug.Log("attack release frame: " + -keysHoldFrames[0]);

                } else
                    animator.SetInteger (TransitionParameter.AtkReleaseTiming.ToString (), 0);

            }

            if (DodgeTrigger) {
                DodgeTrigger = false;
            }
            if (AttackTrigger) {
                AttackTrigger = false;
            }

        }
    }

}