using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {
    public enum TransitionParameter {
        Move,
        AttackMelee,
        ForcedTransition

    }

    public class CharacterControl : MonoBehaviour {
        public InputsDataPerFrame inputDataTop = new InputsDataPerFrame ();
        public bool isPlayerControll;
        public List<Collider> RagdollParts = new List<Collider> ();
        public List<Collider> AttackingParts = new List<Collider> ();
        //private List<TriggerDetector> TriggerDetectors = new List<TriggerDetector> ();
        private TriggerDetector detector;

        CharacterController controller;
        Animator animator;

        //private float hInput;
        //private float vInput;
        //private Vector3 moveDirection = Vector3.zero;

        void Awake () {
            animator = GetComponentInChildren<Animator> ();
            controller = GetComponent<CharacterController> ();
            detector = GetComponentInChildren<TriggerDetector> ();
            SetRagdollAndAttackingParts ();
        }

        void Start () {

        }

        public CharacterController characterController {
            get {
                return controller;
            }
        }

        public TriggerDetector GetTriggerDetector () {
            return detector;

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

        private void TurnOnRagdoll() {
            //Rigidbody rig = GetComponent<Rigidbody>();
            //rig.useGravity = false;
            animator.enabled = false;
            foreach(Collider c in RagdollParts)
            {
                c.isTrigger = false;
                c.attachedRigidbody.velocity = Vector3.zero;
            }

        }

        public void Dead () {
            TurnOnRagdoll();

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
        /*
                public void MoveForward (Vector3 dir, float s, float sgraph) {
                    //m_Controller.Move(moveDirection * s * sgraph * Time.deltaTime);
                    controller.Move (dir * s * sgraph * Time.deltaTime);
                }
                */

        void Update () {
            /*
            hInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");
            //Debug.Log(hInput);
            //Debug.Log(vInput);
            */
            if (isPlayerControll)
                inputDataTop = VirtualInputManager.Instance.GetTopInput ();
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
            if (inputDataTop.KeysState[(int) InputKeyStateType.KEY_MELEE_ATTACK_DOWN]) {
                animator.SetBool (TransitionParameter.AttackMelee.ToString (), true);
            } else {
                animator.SetBool (TransitionParameter.AttackMelee.ToString (), false);
            }

        }
    }

}