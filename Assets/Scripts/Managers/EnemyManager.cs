using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class EnemyManager : Singleton<EnemyManager> {

        private EnemySpawner spawner;

        public override void Init () {
            spawner = GameObject.FindObjectOfType<EnemySpawner> ();
        }

        public void SpawnEnemy (Vector3 position, string prefabName) {
            spawner.SpawnEnemy (position, prefabName);
        }
    }
}