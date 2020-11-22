using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [DllImport ("user32.dll")]
        static extern void mouse_event (MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

        [Flags]
        enum MouseEventFlag : uint {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

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
                //Debug.Log (buttonObjContainer.Buttons.Length);
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
            if (Time.timeScale == 0f)
                ResumeGame ();
            if (MenuIndexRecord.Count == 0 && index < 0)
                return;
            StartCoroutine (_MenuChange (index));

        }
        IEnumerator _MenuChange (int index) {
            animator.SetTrigger ("Fade");
            SetInputState (false);
            yield return new WaitForSecondsRealtime (0.5f);
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
            yield return new WaitForSecondsRealtime (0.5f);
            SetInputState (true);
        }
        public void InitButtonList () {

            currentSelectedButtonIdx = 0;
            UIContainer buttonObjContainer = this.GetComponent<UIContainer> ();
            if (buttonObjContainer != null) {
                //Debug.Log (buttonObjContainer.Buttons.Length);
                buttonList = buttonObjContainer.Buttons;
                if (buttonList != null) {
                    buttonNum = buttonList.Length;
                    Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
                    if (currentSelectedButton != null) {
                        EventSystem.current.SetSelectedGameObject (null);
                        EventSystem.current.SetSelectedGameObject (currentSelectedButton.gameObject);
                        /*
                        currentSelectedButton.Unselect ();
                        currentSelectedButton.Select ();
                        */
                    }
                }
            }
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
        public void KeepItemInViewport () {
            ScrollRect scrollRect = GetComponentInChildren<ScrollRect> ();
            if (scrollRect != null) {
                RectTransform viewport = scrollRect.gameObject.transform.GetChild (0).GetChild (0).GetComponent<RectTransform> ();
                float offsetY = viewport.offsetMax.y;
                float upperBoundY = 0f;
                float lowerBoundY = -scrollRect.GetComponent<RectTransform> ().rect.height;
                Button currentButton = buttonList[currentSelectedButtonIdx];
                float MaxY = currentButton.GetComponent<RectTransform> ().offsetMax.y + offsetY;
                float MinY = currentButton.GetComponent<RectTransform> ().offsetMin.y + offsetY;
                //Rect rectTarget = buttonList[(buttonNum + currentSelectedButtonIdx - 1) % buttonNum].GetComponent<RectTransform>().rect;
                float deltaY = Mathf.Max (lowerBoundY - MinY, MaxY - upperBoundY);
                if (lowerBoundY > MinY) {
                    StartCoroutine (MoveScrollRectPosition (-20f / 4, 0.2f));
                } else if (upperBoundY < MaxY) {
                    StartCoroutine (MoveScrollRectPosition (20f / 4, 0.2f));
                }
                //Debug.Log(rectTarget);
            }

        }
        IEnumerator MoveScrollRectPosition (float offset, float duration) {
            ScrollRect scrollRect = GetComponentInChildren<ScrollRect> ();
            if (scrollRect != null) {
                //PointerEventData data;// = new PointerEventData();
                //data.scrollDelta = new Vector2(0f, -0.1f);

                //Vector2 offsetVector = new Vector2 (scrollRect.normalizedPosition.x, scrollRect.normalizedPosition.y + offset);
                //DOTween.To (() => scrollRect.normalizedPosition, x => scrollRect.normalizedPosition = x, offsetVector, 1.0);
                //scrollRect.normalizedPosition = offsetVector;
                //scrollRect.OnScroll.Invoke();
                //scrollRect.OnScroll(data);

                float t = 0f;
                while (t < duration) {
                    Vector2 offsetVector = new Vector2 (scrollRect.normalizedPosition.x, scrollRect.normalizedPosition.y + offset * Time.deltaTime);
                    scrollRect.normalizedPosition = offsetVector;
                    //mouse_event(MouseEventFlag.Wheel, 0, 0, (uint)(offset * Time.deltaTime), UIntPtr.Zero);
                    t += Time.deltaTime;
                    yield return null;

                }
            }
        }

        public void PreviousItem () {
            currentSelectedButtonIdx = (buttonNum + currentSelectedButtonIdx - 1) % buttonNum;
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.Select ();
            KeepItemInViewport ();
            //StartCoroutine(MoveScrollRectPosition(5f / 4 , 0.2f));
            //MoveScrollRectPosition (1.0f); 
            //animator.SetInteger ("CurrentItemIndex", currentSelectedButtonIdx);
        }
        public void NextItem () {
            currentSelectedButtonIdx = (currentSelectedButtonIdx + 1) % buttonNum;
            Button currentSelectedButton = buttonList[currentSelectedButtonIdx];
            if (currentSelectedButton != null)
                currentSelectedButton.Select ();
            KeepItemInViewport ();
            //MoveScrollRectPosition (-1.0f); 
            //StartCoroutine(MoveScrollRectPosition(-5f / 4 , 0.2f));
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
        public void PauseGame () {
            GameManager.Instance.PauseGame ();
        }
        public void ResumeGame () {
            GameManager.Instance.ResumeGame ();
        }
    }
}