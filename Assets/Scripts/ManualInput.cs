using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class ManualInput : MonoBehaviour {

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

            if (player.DodgeTrigger) {
                VirtualInputManager.Instance.ClearAllInputsInBuffer ();
            }
            if (player.AttackTrigger) {
                VirtualInputManager.Instance.ClearAllInputsInBuffer ();
            }
            if (player.ExecuteTrigger) {
                VirtualInputManager.Instance.ClearAllInputsInBuffer ();
            }

            player.CommandAttackHoldFrame = keysHoldFrames[0];
            player.CommandExecuteHoldFrame = keysHoldFrames[1];


            // for test
            if (Input.GetKeyDown (KeyCode.B)) {
                if (player.Animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) == 1.0f)
                    player.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 0.1f);
                else
                    player.Animator.SetFloat (TransitionParameter.SpeedMultiplier.ToString (), 1f);
            }


        }
    }

}