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
        public bool MoveUnderControl;
        public bool ConsiderMomentum;
        public AnimationCurve extraSpeedGraph;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            if (MoveUnderControl)
                ControlledMove (stateEffect.CharacterControl, animator, animatorStateInfo);
            else
                MoveForward (stateEffect.CharacterControl, animator, animatorStateInfo);
        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
            /*
            if (MoveUnderControl)
                animator.SetBool (TransitionParameter.Move.ToString (), false);
                */
        }

        public void ControlledMove (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {
            Vector2 inputVector = control.inputVector;
            //Debug.Log(inputVector);

            if (inputVector.magnitude > 0.01f) {
                //animator.SetBool (TransitionParameter.Move.ToString (), true);
                if (animatorStateInfo.IsName ("Move") ||
                    animatorStateInfo.IsName ("Walk") ||
                    animatorStateInfo.IsName ("Run")) {
                    Vector3 moveDirection = new Vector3 (inputVector.x, 0, inputVector.y);
                    //MoveForward (moveDirection, speed, 1.0f);
                    Vector3 deltaMoveAmount = moveDirection * animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * speed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime;
                    /*
                    if (ConsiderMomentum) {
                        if (control.CharacterData.GetPrevState () == Animator.StringToHash ("Move") ||
                            control.CharacterData.GetPrevState () == Animator.StringToHash ("Dodge_End") ||
                            control.CharacterData.GetPrevState () == Animator.StringToHash ("Dodge_Tumbling") ||
                            control.CharacterData.GetPrevState () == Animator.StringToHash ("Run")) {
                            Debug.Log (control.CharacterData.GetPrevState () == Animator.StringToHash ("Dodge_End"));
                            Debug.Log (control.CharacterData.GetPrevState () == Animator.StringToHash ("Dodge_Tumbing"));
                            Vector3 extraDeltaMove = moveDirection * control.CharacterController.velocity.magnitude;
                            extraDeltaMove = extraDeltaMove * animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * extraSpeedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime;
                            deltaMoveAmount = deltaMoveAmount + extraDeltaMove;
                        }
                    }
                    */
                    control.CharacterController.Move (deltaMoveAmount);
                    //animator.transform.root.Translate(moveDirection * speed * Time.deltaTime);
                    //animator.transform.Translate(moveDirection * speed * Time.deltaTime);

                    if (!animatorStateInfo.IsName ("Walk")) {
                        Quaternion rotation = Quaternion.LookRotation (moveDirection, Vector3.up);
                        animator.gameObject.transform.root.rotation = Quaternion.Slerp (animator.gameObject.transform.root.rotation, rotation, Time.deltaTime * 5f);
                        //float angle = Mathf.Acos(Vector3.Dot(new Vector3(0, 0, 1), moveDirection)) * Mathf.Rad2Deg;
                        //if (inputVector.x < 0.0f) { angle = -angle; }
                        //Quaternion target = Quaternion.Euler(new Vector3(0, angle, 0));
                        //animator.transform.localRotation = Quaternion.Slerp(animator.transform.localRotation, target, Time.deltaTime * smooth);
                    }

                }
            }
        }

        public void MoveForward (CharacterControl control, Animator animator, AnimatorStateInfo animatorStateInfo) {
            Vector3 moveDirection = control.FaceTarget;
            Vector3 deltaMoveAmount = moveDirection * animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * speed * speedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime;
            if (ConsiderMomentum) {
                if (control.CharacterData.GetPrevState () == Animator.StringToHash ("Move") ||
                    control.CharacterData.GetPrevState () == Animator.StringToHash ("Dodge_End") ||
                    control.CharacterData.GetPrevState () == Animator.StringToHash ("Dodge_Tumbling") ||
                    control.CharacterData.GetPrevState () == Animator.StringToHash ("Run")) {

                    Vector3 extraDeltaMove = moveDirection * control.CharacterController.velocity.magnitude;
                    extraDeltaMove = extraDeltaMove * animator.GetFloat (TransitionParameter.SpeedMultiplier.ToString ()) * extraSpeedGraph.Evaluate (animatorStateInfo.normalizedTime) * Time.deltaTime;
                    deltaMoveAmount = deltaMoveAmount + extraDeltaMove;
                }
            }
            control.CharacterController.Move (deltaMoveAmount);

        }
    }
}