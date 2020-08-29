using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class SceneLoader : MonoBehaviour {

        void Awake () {
            VirtualInputManager.Instance.Init ();
            KeyboardManager.Instance.Init ();
            PoolManager.Instance.Init();
            AttackManager.Instance.Init ();
            CameraManager.Instance.Init ();
            AIAgentManager.Instance.Init ();
        }

        void Update () { }
    }
}