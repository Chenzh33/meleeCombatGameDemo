using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace meleeDemo {
    public class EnergyUI : MonoBehaviour {

        CharacterData data;

        Coroutine AnimationCoroutine;

        public Image BarFill;
        public Image BarBound;
        public TextMeshProUGUI Count;
        Animator BarBoundAnim;

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
            }
            Count = this.GetComponentInChildren<TextMeshProUGUI> ();
            BarBoundAnim = BarBound.GetComponent<Animator>();
            playerControl.CharacterData.OnEnergyChange += EnergyChange;
            playerControl.CharacterData.OnEnergyTake += PlayTakeEnergy;
            playerControl.CharacterData.OnEnergyNotEnough += PlayNotEnoughEnergy;
            playerControl.CharacterData.OnEnergyGet += PlayGetEnergy;

        }

        void EnergyChange () {

            int count;
            if (data.Energy / data.MaxEnergy > 1.0f) {
                count = (int) (data.Energy / data.MaxEnergy);
                Count.text = count.ToString ();
            } else {
                count = 0;
                Count.text = "";
            }
            BarFill.fillAmount = (data.Energy - (float) count * data.MaxEnergy) / data.MaxEnergy;
        }

        void PlayNotEnoughEnergy()
        {
            BarBoundAnim.Play("NotEnoughEnergy");

        }

        void PlayTakeEnergy()
        {
            BarBoundAnim.Play("TakeEnergy");

        }

        void PlayGetEnergy()
        {
            BarBoundAnim.Play("GetEnergy");

        }
    }
}