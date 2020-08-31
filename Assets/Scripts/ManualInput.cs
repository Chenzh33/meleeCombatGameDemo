﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class ManualInput : MonoBehaviour {

        [SerializeField]
        CharacterControl player;

        public InputsDataPerFrame inputDataTop = new InputsDataPerFrame ();
        public int[] keysHoldFrames = new int[4];

        void Awake () {
            player = GetComponent<CharacterControl> ();
        }

        void Start () {

        }

        void Update () {

            inputDataTop = VirtualInputManager.Instance.GetTopInput ();
            keysHoldFrames = VirtualInputManager.Instance.GetKeysHoldFrames ();

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
            }


            if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_MELEE_ATTACK_DOWN))
                player.CommandAttack = false;
            if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_EXECUTE_ATTACK_DOWN))
                player.CommandExecute = false;
            if (!VirtualInputManager.Instance.CheckInputInBuffer (InputKeyStateType.KEY_DODGE_DOWN))
                player.CommandDodge = false;

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
            Vector3 forward = gameObject.transform.forward;
            forward.y = 0f;
            float angle = (Quaternion.LookRotation(forward, Vector3.up)).eulerAngles.y * Mathf.Deg2Rad;
            //Debug.Log(forward.ToString() + " " + Mathf.Cos(angle).ToString() + " " + Mathf.Sin(angle).ToString());
            float blendTreeInputX = player.inputVector.x * Mathf.Cos(angle) - player.inputVector.y * Mathf.Sin(angle);
            float blendTreeInputY = player.inputVector.x * Mathf.Sin(angle) + player.inputVector.y * Mathf.Cos(angle);
            player.Animator.SetFloat("InputX", blendTreeInputX);
            player.Animator.SetFloat("InputY", blendTreeInputY);
            */

        }
    }

}