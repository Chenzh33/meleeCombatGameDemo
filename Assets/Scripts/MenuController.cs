using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace meleeDemo {

    public class MenuController : MonoBehaviour {
        private Animator animator;
        public Canvas currentMenu;
        private Button[] buttonList;
        [SerializeField]
        private int currentSelectedButtonIdx;
        private int buttonNum;
        public Vector2 inputVector = new Vector2 ();
        public bool[] inputKeyStates = new bool[12];
        //public float interval = 0.4f;
        //private Coroutine changeItemCooldownCoroutine;

        void Awake () {
            animator = GetComponent<Animator> ();
            OnMenuChange ();
            //currentMenu = GetComponent<Canvas>();
            //currentTime = 0f;

        }

        void Start () {

        }

        void Update () {
            CheckChangeItem ();

            if (inputKeyStates[(int) InputKeyStateType.KEY_EXECUTE_ATTACK_DOWN])
                ConfirmItem ();

            if (inputKeyStates[(int) InputKeyStateType.KEY_DODGE_DOWN])
                Back ();

        }
        /*
        IEnumerator _CheckChangeItemCooldown () {
            yield return new WaitForSeconds (interval);
            changeItemCooldownCoroutine = null;

        }
        */

        public void CheckChangeItem () {
            //if (changeItemCooldownCoroutine == null) {
                //bool checkInput = false;
                //if (inputVector.x < 0f || inputVector.y > 0f) {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W)) {
                    PreviousItem ();
                    //checkInput = true;
                //} else if (inputVector.x > 0f || inputVector.y < 0f) {
                }else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S)) {
                    NextItem ();
                    //checkInput = true;
                }
                /*
                if (checkInput)
                    changeItemCooldownCoroutine = StartCoroutine (_CheckChangeItemCooldown ());
                    */
            //}
        }
        public void OnMenuChange () {
            currentSelectedButtonIdx = 0;
            buttonList = currentMenu.GetComponentsInChildren<Button> ();
            buttonNum = buttonList.Length;
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.Select ();
        }

        public void PreviousItem () {
            currentSelectedButtonIdx = (buttonNum + currentSelectedButtonIdx - 1) % buttonNum;
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.Select ();
        }
        public void NextItem () {
            currentSelectedButtonIdx = (currentSelectedButtonIdx + 1) % buttonNum;
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.Select ();
        }
        public void ConfirmItem () {
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.onClick.Invoke ();

        }
        public void Back () {
            animator.SetTrigger ("Back");
        }
    }
}