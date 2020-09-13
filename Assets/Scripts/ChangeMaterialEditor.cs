using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace meleeDemo {

    [CustomEditor(typeof(MaterialChanger))]
    public class ChangeMaterialEditor : Editor{

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MaterialChanger materialChanger = (MaterialChanger)target;

            if(GUILayout.Button("Change Material To Transparent"))
            {
                materialChanger.ChangeMaterial();
            }

        }

    }
}