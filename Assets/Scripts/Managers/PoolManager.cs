using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum PoolObjectType {
        ATTACK_INFO,
        ATTACK_HOLD_PROJECTILE
    }

    public class PoolManager : Singleton<PoolManager> {
        public Dictionary<PoolObjectType, List<GameObject>> Pool = new Dictionary<PoolObjectType, List<GameObject>> ();

        private void BuildDictionary () {
            PoolObjectType[] types = System.Enum.GetValues (typeof (PoolObjectType)) as PoolObjectType[];
            foreach (PoolObjectType t in types) {
                Pool.Add (t, new List<GameObject> ());
            }
        }

        public GameObject GetObject (PoolObjectType type) {
            Debug.Log ("get object");
            GameObject obj = null;
            List<GameObject> list = Pool[type];
            if (list.Count > 0) {
                obj = list[0];
                list.RemoveAt (0);
                Debug.Log ("count > 0");
            } else {
                Debug.Log ("count = 0");
                obj = InstantiatePrefab (type);
                //list.Add (obj);
            }
            return obj;

        }

        public void ReturnToPool (PoolObject obj) {
            Debug.Log ("return to pool");
            GameObject go = obj.gameObject;
            go.transform.parent = null;
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            List<GameObject> list = Pool[obj.type];
            list.Add (obj.gameObject);
            obj.gameObject.SetActive (false);

        }
        public override void Init () {
            BuildDictionary ();

        }

        private GameObject InstantiatePrefab (PoolObjectType type) {
            GameObject obj = null;
            switch (type) {
                case PoolObjectType.ATTACK_INFO:
                    {
                        obj = Instantiate (Resources.Load ("AttackInfoPrefab", typeof (GameObject)) as GameObject);
                        break;
                    }
                case PoolObjectType.ATTACK_HOLD_PROJECTILE:
                    {
                        obj = Instantiate (Resources.Load ("AttackHoldProjectilePrefab", typeof (GameObject)) as GameObject);
                        break;
                    }

            }
            return obj;

        }
    }
}