using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class EnemySpawner : MonoBehaviour {
        public string UnitPrefabName;

        void Start () {

        }

        void Update () {

        }

        public void SpawnEnemy (Vector3 position, string prefabName) {

            GameObject obj = Instantiate (Resources.Load (prefabName, typeof (GameObject)) as GameObject);
            obj.SetActive(true);
            obj.transform.position = position;
            CharacterControl control = obj.GetComponent<CharacterControl>();
            control.Init();
            control.Spawn();

        }
    }
}