using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace meleeDemo {

    public class MenuController : MonoBehaviour {
        private Animator animator;
        //public Canvas currentMenu;
        //public GameObject currentMenuObj;
        private ManualInput input;

        [SerializeField]
        private Button[] buttonList;
        [SerializeField]
        private int currentSelectedButtonIdx;
        private int currentMenuIndex;
        [SerializeField]
        private List<int> MenuIndexRecord = new List<int> ();

        private int buttonNum;
        public Vector2 inputVector = new Vector2 ();
        public bool[] inputKeyStates = new bool[12];
        //public float interval = 0.4f;
        //private Coroutine changeItemCooldownCoroutine;

        void Awake () {
            input = GetComponent<ManualInput> ();
            Animator[] animators = GetComponentsInChildren<Animator> ();
            animator = animators[animators.Length - 1];
            //OnMenuChange ();
            currentMenuIndex = 0;
            //currentMenuObj = this.transform.GetChild (0).gameObject;
            //MenuIndexRecord.Add(0);
            SetButtonList ();
            //currentMenu = GetComponent<Canvas>();
            //currentTime = 0f;

        }

        void Start () {

        }

        void Update () {

            if (input != null && input.enabled) {

                CheckChangeItem ();

                if (inputKeyStates[(int) InputKeyStateType.KEY_EXECUTE_ATTACK_DOWN])
                    ConfirmItem ();

                if (inputKeyStates[(int) InputKeyStateType.KEY_DODGE_DOWN])
                    Back ();
            }

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
            if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.W)) {
                PreviousItem ();
                //checkInput = true;
                //} else if (inputVector.x > 0f || inputVector.y < 0f) {
            } else if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.S)) {
                NextItem ();
                //checkInput = true;
            }
            /*
            if (checkInput)
                changeItemCooldownCoroutine = StartCoroutine (_CheckChangeItemCooldown ());
                */
            //}
        }
        /*
        public Canvas GetCurrentCanvas () {
            return currentMenu;
        }
        */
        public void SetButtonList () {
            GameObject currentMenuObj = this.transform.GetChild (currentMenuIndex).gameObject;
            UIContainer buttonObjContainer = currentMenuObj.GetComponent<UIContainer> ();
            if (buttonObjContainer != null) {
                Debug.Log (buttonObjContainer.Buttons.Length);
                buttonList = buttonObjContainer.Buttons;
                if (buttonList != null) {
                    buttonNum = buttonList.Length;
                    Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
                    if (currentSelectedButton != null)
                        currentSelectedButton.Select ();
                }
            }
        }
        public void MenuChange (int index) {
            if (MenuIndexRecord.Count == 0 && index < 0)
                return;
            StartCoroutine (_MenuChange (index));

        }
        IEnumerator _MenuChange (int index) {
            animator.SetTrigger ("Fade");
            SetInputState (false);
            yield return new WaitForSeconds (0.5f);
            currentSelectedButtonIdx = 0;
            GameObject currentMenuObj = this.transform.GetChild (currentMenuIndex).gameObject;
            currentMenuObj.SetActive (false);
            if (index >= 0) {
                MenuIndexRecord.Add (currentMenuIndex);
                currentMenuIndex = index;
            } else {
                int previousMenuIndex = MenuIndexRecord[MenuIndexRecord.Count - 1];
                MenuIndexRecord.RemoveAt (MenuIndexRecord.Count - 1);
                currentMenuIndex = previousMenuIndex;
            }
            currentMenuObj = this.transform.GetChild (currentMenuIndex).gameObject;
            currentMenuObj.SetActive (true);
            SetButtonList ();
            yield return new WaitForSeconds (0.5f);
            SetInputState (true);

            /*
                        Canvas[] CanvasList = GetComponentsInChildren<Canvas> ();
                        foreach (Canvas c in CanvasList) {
                            if (c.gameObject.transform.parent == this.transform) {
                                Debug.Log (c);
                                currentMenu = c;
                            }

                        }
                        */
            //buttonList = currentMenu.GetComponentsInChildren<Button> ();
            //UIContainer buttonObjContainer = currentMenu.GetComponent<UIContainer> ();

            //animator.SetInteger ("CurrentItemIndex", currentSelectedButtonIdx);
        }

        public void SetInputState (bool state) {
            if (input != null) {
                if (!state) {
                    inputVector = new Vector2 ();
                    inputKeyStates = new bool[12];
                }
                input.enabled = state;
            }

        }

        public void FocusOnTarget (RectTransform target) {

        }

        public void MoveScrollRectPosition (float offset) {
            ScrollRect scrollRect = GetComponentInChildren<ScrollRect> ();
            if (scrollRect != null) {

                Vector2 offsetVector = new Vector2 (scrollRect.normalizedPosition.x, scrollRect.normalizedPosition.y + offset);
                //DOTween.To (() => scrollRect.normalizedPosition, x => scrollRect.normalizedPosition = x, offsetVector, 1.0);
                scrollRect.normalizedPosition = offsetVector;
            }
        }

        public void PreviousItem () {
            currentSelectedButtonIdx = (buttonNum + currentSelectedButtonIdx - 1) % buttonNum;
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.Select ();
            //animator.SetInteger ("CurrentItemIndex", currentSelectedButtonIdx);
        }
        public void NextItem () {
            currentSelectedButtonIdx = (currentSelectedButtonIdx + 1) % buttonNum;
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.Select ();
            MoveScrollRectPosition (0.1f); 
            //animator.SetInteger ("CurrentItemIndex", currentSelectedButtonIdx);
        }
        public void ConfirmItem () {
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.onClick.Invoke ();

        }
        public void Back () {
            MenuChange (-1);
        }
    }
}