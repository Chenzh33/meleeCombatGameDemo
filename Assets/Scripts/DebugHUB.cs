using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace meleeDemo {

    public class DebugHUB : MonoBehaviour {
        private TextMeshProUGUI debugText;
        private void Awake () {
            GameObject debugTextObj = transform.Find ("DebugText").gameObject;
            debugText = debugTextObj.GetComponent<TextMeshProUGUI> ();
            debugText.text = "";

        }

        public void Fresh () {
            InputsDataPerFrame[] inputData = VirtualInputManager.Instance.GetAllInputs ();
            int curIdx = VirtualInputManager.Instance.GetIndex ();
            debugText.text = "";
            for (int i = VirtualInputManager.INPUT_BUFFER_SIZE; i != 0; --i) {
                debugText.text += inputData[(curIdx + i) % VirtualInputManager.INPUT_BUFFER_SIZE].ToString ();
                debugText.text += "\n";

            }
        }
        void Update () {
            Fresh ();

        }
    }
}