using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T instance;

        static public T Instance {
            get {
                if (instance == null) {
                    instance = Object.FindObjectOfType (typeof (T)) as T;

                    if (instance == null) {
                        GameObject go = new GameObject (typeof (T).ToString ());
                        DontDestroyOnLoad (go);
                        instance = go.AddComponent<T> ();
                    }

                }
                return instance;
            }
        }

        public virtual void Init()
        {

        }
    }
}