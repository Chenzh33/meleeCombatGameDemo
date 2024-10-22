﻿using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace meleeDemo {

    public class ManualInput : MonoBehaviour {

        [SerializeField]
        CharacterControl player;
        MenuController menu;

        public InputsDataPerFrame inputDataTop = new InputsDataPerFrame ();
        public int[] keysHoldFrames = new int[5];

        void Awake () {
            player = GetComponent<CharacterControl> ();
            menu = GetComponent<MenuController> ();
        }

        void Start () {

        }

        void Update () {

            inputDataTop = VirtualInputManager.Instance.GetTopInput ();
            keysHoldFrames = VirtualInputManager.Instance.GetKeysHoldFrames ();

            if (player != null) {

                //Debug.Log(inputDataTop.InputVector);
                player.inputVector = inputDataTop.InputVector;
                player.inputKeyStates = inputDataTop.KeysState;

                if (VirtualInputManager.Instance.CheckCommandInput ()) {
                    if (VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_MELEE_ATTACK_DOWN))
                        player.CommandAttack = true;
                    if (VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_EXECUTE_ATTACK_DOWN))
                        player.CommandExecute = true;
                    if (VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_DODGE_DOWN))
                        player.CommandDodge = true;
                    if (VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_GUARD_DOWN))
                        player.CommandGuard = true;
                }

                if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_MELEE_ATTACK_DOWN))
                    player.CommandAttack = false;
                if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_EXECUTE_ATTACK_DOWN))
                    player.CommandExecute = false;
                if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_DODGE_DOWN))
                    player.CommandDodge = false;
                if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_GUARD_DOWN))
                    player.CommandGuard = false;

                if (inputDataTop.KeysState[(int) InputKeyStateType.KEY_CHARGE])
                    player.CommandCharge = true;
                else
                    player.CommandCharge = false;

                //if (inputDataTop.KeysState[(int) InputKeyStateType.KEY_GUARD])
                /*
                if (inputDataTop.KeysState[(int) InputKeyStateType.KEY_GUARD_DOWN])
                    player.CommandGuard = true;
                else
                    player.CommandGuard = false;
                    */

                if (inputDataTop.KeysState[(int) InputKeyStateType.KEY_GUARD])
                    player.CommandGuardHoldOn = true;
                else
                    player.CommandGuardHoldOn = false;

                /*
                            if (player.DodgeTrigger) {
                                VirtualInputManager.Instance.ClearAllInputsInBuffer ();
                            }
                            if (player.AttackTrigger) {
                                VirtualInputManager.Instance.ClearAllInputsInBuffer ();
                            }
                            if (player.ExecuteTrigger) {
                                VirtualInputManager.Instance.ClearAllInputsInBuffer ();
                            }
                            */

                player.CommandAttackHoldFrame = keysHoldFrames[0];
                player.CommandExecuteHoldFrame = keysHoldFrames[1];
                player.CommandDodgeHoldFrame = keysHoldFrames[2];
                player.InputAxisHoldFrame = keysHoldFrames[5];

                // for test
                if (Input.GetKeyDown (KeyCode.B)) {
                    if (player.Animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) == 1.0f)
                        player.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0.1f);
                    else
                        player.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1f);
                }
                // for test
                if (Input.GetKeyDown (KeyCode.Y)) {
                    if (!player.Animator.GetBool ("IsRunning")) {
                        player.Animator.SetBool ("IsRunning", true);
                        player.CharacterData.IsRunning = true;
                    } else {
                        player.Animator.SetBool ("IsRunning", false);
                        player.CharacterData.IsRunning = false;
                    }
                }
             
                // for test
                /*
                if (Input.GetKeyDown (KeyCode.P)) {
                    Dictionary<string, int> dicts = new Dictionary<string, int> ();
                    dicts.Add ("Swordman", 1);
                    //dicts.Add ("Gunman", 2);
                    SpawnEnemyGroup (dicts);
                }
                */

                // for test
                if (Input.GetKeyDown (KeyCode.T)) {
                    CinemachineStateDrivenCamera camera = GameObject.FindObjectOfType (typeof (CinemachineStateDrivenCamera)) as CinemachineStateDrivenCamera;
                    Animator anim = camera.m_AnimatedTarget;
                    CinemachineCameraOffset offset = camera.gameObject.GetComponent<CinemachineCameraOffset> ();
                    float Offset = anim.GetFloat ("ExtraOffset");
                    offset.m_Offset.y += Offset;
                    offset.m_Offset.z += -Offset;
                }
                // for test
                if (Input.GetKeyDown (KeyCode.N)) {
                    if (player.CharacterData.Energy > 1.0f)
                        player.TakeEnergy (1.0f, null);
                }
                if (Input.GetKeyDown (KeyCode.M)) {
                    player.CharacterData.GetEnergy (4.0f);
                }
                /*
                            Vector3 forward = gameObject.transform.forward;
                            forward.y = 0f;
                            float angle = (Quaternion.LookRotation (forward, Vector3.up)).eulerAngles.y * Mathf.Deg2Rad;
                            //Debug.Log(forward.ToString() + " " + Mathf.Cos(angle).ToString() + " " + Mathf.Sin(angle).ToString());
                            float blendTreeInputX = player.inputVector.x * Mathf.Cos (angle) - player.inputVector.y * Mathf.Sin (angle);
                            float blendTreeInputY = player.inputVector.x * Mathf.Sin (angle) + player.inputVector.y * Mathf.Cos (angle);
                            player.Animator.SetFloat ("InputX", blendTreeInputX);
                            player.Animator.SetFloat ("InputY", blendTreeInputY);

                            if (Input.GetKeyDown (KeyCode.F)) {
                                player.Animator.SetBool ("Fire", true);
                            } else {
                                player.Animator.SetBool ("Fire", false);
                            }
                            */
            }
            if (menu != null) {
                menu.inputVector = inputDataTop.InputVector;
                menu.inputKeyStates = inputDataTop.KeysState;
            }

        }

        /*
                public void SpawnAnEnemy (string name) {
                    float x = Random.Range (3f, 5f);
                    float xx = Random.Range (0f, 1f);
                    if (xx > 0.5f)
                        x = -x;
                    float y = Random.Range (3f, 5f);
                    //float yy = Random.Range (0f, 1f);
                    //if (yy > 0.5f)
                    //   y = -y;
                    EnemyManager.Instance.SpawnEnemy (this.gameObject.transform.position + new Vector3 (x, 0f, y), name);
                }

                IEnumerator _SpawnEnemyGroup (Dictionary<string, int> dicts) {
                    foreach (var item in dicts) {
                        for (int i = 0; i != item.Value; ++i) {
                            SpawnAnEnemy (item.Key);
                            yield return new WaitForSeconds (0.5f);
                        }
                    }

                }

                public void SpawnEnemyGroup (Dictionary<string, int> dicts) {
                    StartCoroutine (_SpawnEnemyGroup (dicts));

                }
                */

    }
}