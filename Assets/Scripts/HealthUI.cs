using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace meleeDemo {
    public class HealthUI : MonoBehaviour {
        CharacterData data;
        Coroutine AnimationCoroutine;

        public float TransitionSpeed = 0.2f;
        public float TransitionDelay = 1f;

        public Image BarFill;
        public Image BarRed;
        public Image BarBound;

        void Start () {
            GameObject player = (FindObjectOfType (typeof (ManualInput)) as ManualInput).gameObject;
            CharacterControl playerControl = player.GetComponent<CharacterControl> ();

            data = playerControl.CharacterData;
            BarImage[] barImages = this.GetComponentsInChildren<BarImage> ();
            foreach (BarImage im in barImages) {
                if (im.type == BarImageType.Bound)
                    BarBound = im.GetComponent<Image> ();
                else if (im.type == BarImageType.Fill)
                    BarFill = im.GetComponent<Image> ();
                else if (im.type == BarImageType.Red)
                    BarRed = im.GetComponent<Image> ();
            }
            playerControl.CharacterData.OnHealthChange += HealthChange;

        }

        IEnumerator _BarAnimation (float delay, float speed) {
            yield return new WaitForSeconds (delay);
            while (true) {
                if (BarFill.fillAmount - BarRed.fillAmount > 0.01f) {
                    BarRed.fillAmount = BarFill.fillAmount;
                    break;
                }
                float deltaAmount = speed * Time.deltaTime;
                BarRed.fillAmount -= deltaAmount;
                yield return null;
            }
            AnimationCoroutine = null;

        }

        public void BarAnimation (float delay, float speed) {
            if (BarFill.fillAmount > BarRed.fillAmount)
            {
                BarRed.fillAmount = BarFill.fillAmount;
                return;
            }
            if (AnimationCoroutine != null)
                StopCoroutine (AnimationCoroutine);
            AnimationCoroutine = StartCoroutine (_BarAnimation (delay, speed));

        }

        // Update is called once per frame
        void Update () {

        }

        void HealthChange () {
            BarFill.fillAmount = data.HP / data.MaxHP;
            if (BarRed != null)
                BarAnimation (TransitionDelay, TransitionSpeed);
        }
    }
}