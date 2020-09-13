using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace meleeDemo {

    public class MaterialChanger : MonoBehaviour {
        public Material material;
        public float alphaBase = 0.5f;
        void Start () {
            //material = GetComponent<Renderer>();

        }
        public void ChangeMaterial () {
            if (material != null) {
                Renderer[] arrMaterial = gameObject.GetComponentsInChildren<Renderer> ();
                foreach (Renderer ms in arrMaterial) {
                    Material[] mats = ms.materials;
                    for (int i = 0; i != mats.Length; ++i) {
                        mats[i] = material;
                    }
                    ms.materials = mats;
                }
            }

        }
        IEnumerator _ChangeTransparency (float duration) {
            float t = 0f;
            float alpha = 1f;
            Renderer[] arrMaterial = gameObject.GetComponentsInChildren<Renderer> ();
            while (t < duration) {
                alpha = (duration - t) / duration;
                foreach (Renderer ms in arrMaterial) {
                    Material[] mats = ms.materials;
                    for (int i = 0; i != mats.Length; ++i) {
                        mats[i].color = new Color(mats[i].color.r, mats[i].color.g, mats[i].color.b, alphaBase * alpha);;
                    }
                    ms.materials = mats;
                }
                t += Time.deltaTime;
                yield return null;
            }
        }

        public void ChangeTransparency (float duration) {
            if (material != null) {
                StartCoroutine (_ChangeTransparency (duration));
            }
        }

        public void TurnOffRenderers()
        {

        }

        public void TurnOnRenderers()
        {

        }
    }
}