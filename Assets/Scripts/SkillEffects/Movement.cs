using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Movement")]
    public class Movement : SkillEffect {
        //private CharacterController characterController;
        //private Coroutine CheckStopCoroutine;
        public AnimationCurve speedGraph;
        public float speed = 6.0f;
        public float smooth = 5.0f;
        public bool LockDirection;
        public bool AllowEarlyTurn;
        public float smoothEarlyTurn = 20f;

        [Range (0f, 1f)]
        public float AllowTurnStartTime = 0f;
        //private Vector3 faceDirection;
        //private CharacterControl characterControl;
        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            //faceDirection = animator.transform.forward;
            //characterControl = stateEffect.GetCharacterControl();

/*
            if (AllowEarlyTurn) {
                Vector2 inputDirection2d = stateEffect.CharacterControl.inputDataTop.InputVector;
                if (inputDirection2d.magnitude > 0.01f) {
                    Vector3 inputDirection = new Vector3 (inputDirection2d.x, 0, inputDirection2d.y);
                    float angle = Mathf.Acos (Vector3.Dot (new Vector3 (0, 0, 1), inputDirection)) * Mathf.Rad2Deg;
                    if (inputDirection.x < 0.0f) { angle = -angle; }
                    Quaternion target = Quaternion.Euler (new Vector3 (0, angle, 0));
                    stateEffect.CharacterControl.TurnToTarget (animatorStateInfo.length * AllowTurnStartTime, smoothEarlyTurn, target);
                    stateEffect.CharacterControl.FaceTarget = inputDirection;
                } else {
                    stateEffect.CharacterControl.FaceTarget = animator.transform.forward;
                }
            } else {
                stateEffect.CharacterControl.FaceTarget = animator.transform.forward;

            }
            */

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (!LockDirection)
                ControlledMove (stateEffect.CharacterControl, animator, animatorStateInfo);
            else
                MoveForward (stateEffect.CharacterControl, animator, animatorStateInfo);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (!LockDirection)
                animator.SetBool (TransitionParameter.Move.ToString (), false);
        }

        /*
                IEnumerator _CheckStopMove (CharacterControl control, Animator animator, float time) {
                    yield return new WaitForSeconds (time);
                    InputsDataPerFrame inputData = control.inputDataTop;
                    Vector2 inputVector = inputData.InputVector;
                    if (inputVector.magnitude <= 0.01f) {
                        animator.SetBool (TransitionParameter.Move.ToString (), false);
                    }
                }
                public void CheckStopMove (CharacterControl control, Animator animator, float time) {
                    if (CheckStopCoroutine != null)
                        StopCoroutine (CheckStopCoroutine);
                    CheckStopCoroutine = StartCoroutine (_CheckStopMove (control, animator, time));

                }
                */
        public void ControlledMove (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {
            InputsDataPerFrame inputData = control.inputDataTop;
            Vector2 inputVector = inputData.InputVector;
            //Debug.Log(inputVector);
            bool[] inputKeysState = inputData.KeysState;

            if (inputVector.magnitude > 0.01f) {
                //animator.SetBool (TransitionParameter.Move.ToString (), true);
                if (animatorStateInfo.IsName ("Move")) {
                    Vector3 moveDirection = new Vector3 (inputVector.x, 0, inputVector.y);
                    //MoveForward (moveDirection, speed, 1.0f);
                    control.CharacterController.Move (moveDirection * animator.GetFloat("SpeedMultiplier") * speed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime);
                    //animator.transform.root.Translate(moveDirection * speed * Time.deltaTime);
                    //animator.transform.Translate(moveDirection * speed * Time.deltaTime);
                    float angle = Mathf.Acos (Vector3.Dot (new Vector3 (0, 0, 1), moveDirection)) * Mathf.Rad2Deg;
                    if (inputVector.x < 0.0f) { angle = -angle; }
                    Quaternion target = Quaternion.Euler (new Vector3 (0, angle, 0));
                    animator.transform.localRotation = Quaternion.Slerp (animator.transform.localRotation, target, Time.deltaTime * smooth);
                }
            } 
            /*
            else {
                if (animatorStateInfo.IsName ("Move")) {
                    //animator.SetBool (TransitionParameter.Move.ToString (), false);
                    control.CheckStopMove (0.10f);
                }
                
                //animator.SetBool (TransitionParameter.Move.ToString (), false);
                
                if (animatorStateInfo.IsName ("Move")) {
                    if (control.IsStoppingMovement) {
                        animator.SetBool (TransitionParameter.Move.ToString (), false);
                        control.IsStoppingMovement = false;
                    } else
                        control.IsStoppingMovement = true;
                }
                
            }*/
            //Vector3 horizontalVelocity = new Vector3 (control.characterController.velocity.x, 0, control.characterController.velocity.z);
        }
        public void MoveForward (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {
            Vector3 moveDirection = control.FaceTarget;
            control.CharacterController.Move (moveDirection * animator.GetFloat("SpeedMultiplier") * speed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime);

        }
    }
}