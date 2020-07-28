using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DebugHUB : MonoBehaviour {
    private TextMeshProUGUI debugText;
    private void Awake () {
        GameObject debugTextObj = transform.Find ("DebugText").gameObject;
        debugText = debugTextObj.GetComponent<TextMeshProUGUI> ();
        debugText.text = "";

    }

    void Update () {
        InputsDataPerFrame[] inputData = VirtualInputManager.Instance.GetAllInputs ();
        debugText.text = "";
        foreach (InputsDataPerFrame d in inputData) {
            debugText.text += d.ToString ();
            debugText.text += "\n";

        }
    }

}