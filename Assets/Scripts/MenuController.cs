using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace meleeDemo {

    public class MenuController : MonoBehaviour {
        private Animator animator;
        public Vector2 inputVector = new Vector2 ();
        public bool[] inputKeyStates = new bool[12];
        /*
        public float interval = 0.4f;
        private float currentTime;
        */

        void Awake () {
            animator = GetComponent<Animator> ();
            //currentTime = 0f;

        }

        void Start () {

        }

        void Update () {
            if (inputVector.x < 0f || inputVector.y > 0f) {
                animator.SetBool ("PreviousItem", true);
                animator.SetBool ("NextItem", false);

            } else if (inputVector.x > 0f || inputVector.y < 0f) {
                animator.SetBool ("NextItem", true);
                animator.SetBool ("PreviousItem", false);

            } else {
                animator.SetBool ("NextItem", false);
                animator.SetBool ("PreviousItem", false);
            }

            if (inputKeyStates[(int) InputKeyStateType.KEY_EXECUTE_ATTACK_DOWN])
                animator.SetTrigger ("Confirm");
            else
                animator.ResetTrigger ("Confirm");

            if (inputKeyStates[(int) InputKeyStateType.KEY_DODGE_DOWN])
                animator.SetTrigger ("Back");
            else
                animator.ResetTrigger ("Back");


        }

    }
}