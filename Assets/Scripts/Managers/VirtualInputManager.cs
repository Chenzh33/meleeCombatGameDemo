using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class InputsDataPerFrame {
        public InputsDataPerFrame () {
            InputVector = new Vector2 ();
            KeysState = new bool[12];
        }
        public override string ToString () {
            string str = InputVector.ToString ();
            str += " ";
            for (int i = 0; i != 12; ++i) {
                if (KeysState[i])
                    str += "1";
                else
                    str += "0";
                if (i % 3 == 2)
                    str += " ";
            }
            return str;
        }
        public Vector2 InputVector;
        public bool[] KeysState;
        //public unsafe fixed bool KeysState[12];
        // = new bool[12];
    }

    public enum InputKeyType {
        KEY_MELEE_ATTACK,
        KEY_EXECUTE_ATTACK,
        KEY_DODGE,
        KEY_CHARGE
    }
    public enum InputKeyStateType {
        KEY_MELEE_ATTACK_DOWN,
        KEY_MELEE_ATTACK_UP,
        KEY_MELEE_ATTACK,
        KEY_EXECUTE_ATTACK_DOWN,
        KEY_EXECUTE_ATTACK_UP,
        KEY_EXECUTE_ATTACK,
        KEY_DODGE_DOWN,
        KEY_DODGE_UP,
        KEY_DODGE,
        KEY_CHARGE_DOWN,
        KEY_CHARGE_UP,
        KEY_CHARGE
    }

    public class VirtualInputManager : Singleton<VirtualInputManager> {

        public Dictionary<InputKeyType, KeyCode> DicKeys = new Dictionary<InputKeyType, KeyCode> ();
        public const int INPUT_BUFFER_SIZE = 10;
        public const int MAX_HOLD_FRAME = 600;
        private int curIndex = 0;
        private InputsDataPerFrame[] inputBuffer = new InputsDataPerFrame[INPUT_BUFFER_SIZE];
        private int[] KeysHoldFrames = new int[5];

        public override void Init () {
            SetDefaultKeyConfig ();
            InitInputDataPool ();

        }

        private void InitInputDataPool () {
            for (int i = 0; i != INPUT_BUFFER_SIZE; ++i) {
                inputBuffer[i] = new InputsDataPerFrame ();
                //inputBuffer[i].InputVector = new Vector2();
                //inputBuffer[i].KeysState = new bool[12];
            }

            for (int i = 0; i != System.Enum.GetValues (typeof (InputKeyType)).Length + 1; ++i) {
                KeysHoldFrames[i] = 0;

            }
        }

        public void SetDefaultKeyConfig () {
            DicKeys.Clear ();
            DicKeys.Add (InputKeyType.KEY_MELEE_ATTACK, KeyCode.I);
            DicKeys.Add (InputKeyType.KEY_EXECUTE_ATTACK, KeyCode.J);
            DicKeys.Add (InputKeyType.KEY_DODGE, KeyCode.K);
            DicKeys.Add (InputKeyType.KEY_CHARGE, KeyCode.Space);

        }

        public int GetIndex () {
            return curIndex;
        }

        void Update () {

        }

        public bool CheckCommandInput () {
            for (int i = 0; i != System.Enum.GetValues (typeof (InputKeyType)).Length; ++i) {
                if (inputBuffer[curIndex].KeysState[i * 3 + 2])
                    return true;
            }
            return false;
        }

        public bool CheckInputInBuffer (InputKeyStateType keystate) {
            for (int i = 0; i != VirtualInputManager.INPUT_BUFFER_SIZE; ++i) { // from old to new
                //for (int i = VirtualInputManager.INPUT_BUFFER_SIZE; i != 0; --i) { // from new to old
                if (inputBuffer[(curIndex + i) % VirtualInputManager.INPUT_BUFFER_SIZE].KeysState[(int) keystate]) {
                    return true;
                }
            }
            return false;
        }
        public void ClearAllInputsInBuffer () {
            for (int i = 0; i != VirtualInputManager.INPUT_BUFFER_SIZE; ++i) { // from old to new
                for (int k = 0; k != 4; ++k) { // from old to new
                    inputBuffer[i].KeysState[k * 3] = false;
                }
            }

        }
        public void ClearInputInBuffer (InputKeyStateType keystate) {
            for (int i = 0; i != VirtualInputManager.INPUT_BUFFER_SIZE; ++i) { // from old to new
                if (inputBuffer[(curIndex + i + 1) % VirtualInputManager.INPUT_BUFFER_SIZE].KeysState[(int) keystate]) {
                    inputBuffer[(curIndex + i + 1) % VirtualInputManager.INPUT_BUFFER_SIZE].KeysState[(int) keystate] = false;
                    return;
                }
            }

        }

        public InputsDataPerFrame GetTopInput () {
            return inputBuffer[curIndex];

        }
        public InputsDataPerFrame[] GetAllInputs () {
            return inputBuffer;
        }

        public int[] GetKeysHoldFrames () {
            return KeysHoldFrames;
        }
        public void LoadInput (InputsDataPerFrame data) {
            curIndex = (curIndex + 1) % INPUT_BUFFER_SIZE;
            inputBuffer[curIndex].InputVector = data.InputVector;
            inputBuffer[curIndex].KeysState = (bool[]) data.KeysState.Clone ();
            StoreHoldTime (data);
            //Debug.Log (inputBuffer[curIndex + 1].ToString());
            //Debug.Log (inputBuffer[curIndex + 1].KeysState[1]);
        }

        private void StoreHoldTime (InputsDataPerFrame data) {

            for (int i = 0; i != System.Enum.GetValues (typeof (InputKeyType)).Length; ++i) {
                if (KeysHoldFrames[i] < 0)
                    KeysHoldFrames[i] = 0;
                if (data.KeysState[i * 3 + 2] && KeysHoldFrames[i] != VirtualInputManager.MAX_HOLD_FRAME)
                    ++KeysHoldFrames[i];
                else if (data.KeysState[i * 3 + 1]) {
                    KeysHoldFrames[i] *= -1;
                    //Debug.Log ("release frame: " + -KeysHoldFrames[i]);

                }
            }
            if (data.InputVector.magnitude > 0.01f)
            {
                if (KeysHoldFrames[4] != VirtualInputManager.MAX_HOLD_FRAME)
                    ++KeysHoldFrames[4];
            }
            else
                KeysHoldFrames[4] = 0;

        }

    }
}