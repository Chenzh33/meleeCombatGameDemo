using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/Movement")]
    public class Movement : SkillEffect {
        //private CharacterController characterController;
        public AnimationCurve speedGraph;
        public float speed = 6.0f;
        public float smooth = 5.0f;
        public bool LockDirection;
        //private Vector3 faceDirection;
        //private CharacterControl characterControl;
        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            //faceDirection = animator.transform.forward;
            //characterControl = stateEffect.GetCharacterControl();

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (!LockDirection)
                ControlledMove (stateEffect.CharacterControl, animator, animatorStateInfo);
            else
                MoveForward (stateEffect.CharacterControl, animator, animatorStateInfo);

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }

        public void ControlledMove (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {
            InputsDataPerFrame inputData = control.inputDataTop;
            Vector2 inputVector = inputData.InputVector;
            //Debug.Log(inputVector);
            bool[] inputKeysState = inputData.KeysState;

          
            if (inputVector.magnitude > 0f) {
                animator.SetBool (TransitionParameter.Move.ToString (), true);
                if (animatorStateInfo.IsName ("Move")) {
                    Vector3 moveDirection = new Vector3 (inputVector.x, 0, inputVector.y);
                    //MoveForward (moveDirection, speed, 1.0f);
                    control.characterController.Move (moveDirection * speed * Time.deltaTime);
                    //animator.transform.root.Translate(moveDirection * speed * Time.deltaTime);
                    //animator.transform.Translate(moveDirection * speed * Time.deltaTime);
                    float angle = Mathf.Acos (Vector3.Dot (new Vector3 (0, 0, 1), moveDirection)) * Mathf.Rad2Deg;
                    if (inputVector.x < 0.0f) { angle = -angle; }
                    Quaternion target = Quaternion.Euler (new Vector3 (0, angle, 0));
                    animator.transform.localRotation = Quaternion.Slerp (animator.transform.localRotation, target, Time.deltaTime * smooth);
                }
            } else {
                animator.SetBool (TransitionParameter.Move.ToString (), false);
            }
        }
        public void MoveForward (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {
            Vector3 faceDirection = animator.transform.forward;
            control.characterController.Move (faceDirection * speed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime);

        }
    }
}