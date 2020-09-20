using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class KeyboardManager : Singleton<KeyboardManager> {
        private InputsDataPerFrame inputs;

        public override void Init () {
            inputs = new InputsDataPerFrame ();
        }

        void Update () {

            inputs.InputVector = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
            inputs.InputVector = Vector2.ClampMagnitude (inputs.InputVector, 1);

            inputs.KeysState[(int) InputKeyStateType.KEY_MELEE_ATTACK_DOWN] = Input.GetKeyDown (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_MELEE_ATTACK]);
            inputs.KeysState[(int) InputKeyStateType.KEY_MELEE_ATTACK_UP] = Input.GetKeyUp (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_MELEE_ATTACK]);
            inputs.KeysState[(int) InputKeyStateType.KEY_MELEE_ATTACK] = Input.GetKey (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_MELEE_ATTACK]);

            inputs.KeysState[(int) InputKeyStateType.KEY_EXECUTE_ATTACK_DOWN] = Input.GetKeyDown (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_EXECUTE_ATTACK]);
            inputs.KeysState[(int) InputKeyStateType.KEY_EXECUTE_ATTACK_UP] = Input.GetKeyUp (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_EXECUTE_ATTACK]);
            inputs.KeysState[(int) InputKeyStateType.KEY_EXECUTE_ATTACK] = Input.GetKey (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_EXECUTE_ATTACK]);

            inputs.KeysState[(int) InputKeyStateType.KEY_DODGE_DOWN] = Input.GetKeyDown (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_DODGE]);
            inputs.KeysState[(int) InputKeyStateType.KEY_DODGE_UP] = Input.GetKeyUp (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_DODGE]);
            inputs.KeysState[(int) InputKeyStateType.KEY_DODGE] = Input.GetKey (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_DODGE]);

            inputs.KeysState[(int) InputKeyStateType.KEY_CHARGE_DOWN] = Input.GetKeyDown (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_CHARGE]);
            inputs.KeysState[(int) InputKeyStateType.KEY_CHARGE_UP] = Input.GetKeyUp (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_CHARGE]);
            inputs.KeysState[(int) InputKeyStateType.KEY_CHARGE] = Input.GetKey (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_CHARGE]);

            inputs.KeysState[(int) InputKeyStateType.KEY_GUARD_DOWN] = Input.GetKeyDown (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_GUARD]);
            inputs.KeysState[(int) InputKeyStateType.KEY_GUARD_UP] = Input.GetKeyUp (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_GUARD]);
            inputs.KeysState[(int) InputKeyStateType.KEY_GUARD] = Input.GetKey (VirtualInputManager.Instance.DicKeys[InputKeyType.KEY_GUARD]);
            VirtualInputManager.Instance.LoadInput (inputs);
        }

    }
}