using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [System.Serializable]
    public class SpawnUnitInfo {
        public SpawnUnitInfo () {
            Name = "";
            //Count = 0;
            PosVector = Vector3.zero;
            RotVector = Vector3.zero;

        }
        public string Name;
        //public int Count;
        public Vector3 PosVector;
        public Vector3 RotVector;
    }

    public class UnitSpawner : MonoBehaviour {
        public List<SpawnUnitInfo> Configs = new List<SpawnUnitInfo> ();
        public Vector3 PlayerSpawnPoint = Vector3.zero;

        void Start () {

        }

        void Update () {

        }

        public void SpawnUnit (Vector3 position, Vector3 rotVec, string prefabName) {

            GameObject obj = Instantiate (Resources.Load (prefabName, typeof (GameObject)) as GameObject);
            //obj.SetActive (true);
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.LookRotation (rotVec, Vector3.up);
            CharacterControl control = obj.GetComponent<CharacterControl> ();
            control.Init ();
            control.Spawn ();
            if (control.isPlayerControl) {
                CameraManager.Instance.Init ();
                GameManager.Instance.SetPlayer (control);
            }

        }
        public void SpawnPlayer () {
            SpawnUnit (PlayerSpawnPoint, Vector3.zero, "Frank");
            //PlayerInput.enabled = true;

        }
        IEnumerator _SpawnEnemyGroup () {
            foreach (var item in Configs) {
                //for (int i = 0; i != item.Count; ++i) {
                SpawnAnEnemy (item.Name, item.PosVector, item.RotVector);
                yield return new WaitForSeconds (0.5f);
                //}
            }
            //AIAgentManager.Instance.RegisterAllEnemies ();
        }

        public void SpawnEnemyGroup () {
            StartCoroutine (_SpawnEnemyGroup ());

        }
        public void SpawnAnEnemy (string name, Vector3 pos, Vector3 rot) {
            /*
            float x = Random.Range (3f, 5f);
            float xx = Random.Range (0f, 1f);
            if (xx > 0.5f)
                x = -x;
            float y = Random.Range (3f, 5f);
            //float yy = Random.Range (0f, 1f);
            //if (yy > 0.5f)
            //   y = -y;
            */
            SpawnUnit (PlayerSpawnPoint + pos, rot, name);
        }
    }
}