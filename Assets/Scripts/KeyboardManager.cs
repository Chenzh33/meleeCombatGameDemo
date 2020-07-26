using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class KeyboardManager : MonoBehaviour {
    private InputsDataPerFrame inputs;
   
    void Update()
    {
        inputs.InputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Horizontal"));
        inputs.InputVector = Vector3.ClampMagnitude(inputs.InputVector, 1);
        inputs.MAttackET = Input.GetKeyDown(KeyCode.I);
        inputs.MAttackEX = Input.GetKeyUp(KeyCode.I);
        inputs.RAttackET = Input.GetKeyDown(KeyCode.J);
        inputs.RAttackEX = Input.GetKeyUp(KeyCode.J);
        inputs.DodgeET = Input.GetKeyDown(KeyCode.K);
        inputs.DodgeEX = Input.GetKeyUp(KeyCode.K);
        inputs.ChargeET = Input.GetKeyDown(KeyCode.L);
        inputs.ChargeEX = Input.GetKeyUp(KeyCode.L);
        
        VirtualInputManager.Instance.LoadInput(inputs);
    }
  
}