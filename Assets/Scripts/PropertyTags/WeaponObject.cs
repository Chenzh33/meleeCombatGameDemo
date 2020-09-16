using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class WeaponObject : MonoBehaviour {
        public Material NormalMaterial;
        public Material ActivateMaterial;

        void Start () {

        }

        void Update () {

        }

        public void ToggleWeaponMaterial () {
            Renderer renderer = gameObject.GetComponent<Renderer> ();
            Material[] mats = renderer.materials;
            Debug.Log ("normal: " + NormalMaterial.name.ToString());
            Debug.Log ("activate: " + ActivateMaterial.name.ToString());
            for (int i = 0; i != renderer.sharedMaterials.Length; ++i) {
                Debug.Log (renderer.sharedMaterials[i].name);
                if (renderer.sharedMaterials[i].name.Contains(NormalMaterial.name))
                    mats[i] = ActivateMaterial;
                else if (renderer.sharedMaterials[i].name.Contains(ActivateMaterial.name))
                    mats[i] = NormalMaterial;
            }
            renderer.materials = mats;
        }
    }
}