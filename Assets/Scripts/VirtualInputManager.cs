using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct InputsDataPerFrame{
    public Vector2 InputVector;
    public bool MAttackET;
    public bool MAttackEX;
    public bool RAttackET;
    public bool RAttackEX;
    public bool DodgeET;
    public bool DodgeEX;
    public bool ChargeET;
    public bool ChargeEX;
}

public class VirtualInputManager : MonoBehaviour {
    public static VirtualInputManager Instance = null;
    private const int INPUT_BUFFER_SIZE = 16;
    private int curIndex = 0;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else{
            Destroy(this.gameObject);
        }
    }
    private void PushInput()
    {
        inputBuffer[curIndex + 1] = inputBuffer[0];
        Debug.Log(inputBuffer[curIndex + 1]);
        curIndex = (curIndex + 1) % INPUT_BUFFER_SIZE;

    }
    void FixedUpdate()
    {
        PushInput();

    }
    
    public InputsDataPerFrame GetInput()
    {
        return inputBuffer[curIndex + 1];

    }

    public void LoadInput(InputsDataPerFrame data)
    {
        inputBuffer[0] = data;
    }
   
    private InputsDataPerFrame[] inputBuffer = new InputsDataPerFrame[INPUT_BUFFER_SIZE + 1];

}