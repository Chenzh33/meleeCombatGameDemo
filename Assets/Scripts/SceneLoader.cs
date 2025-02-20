﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class SceneLoader : MonoBehaviour {

        void Awake () {
            GameManager.Instance.Init ();

        }

        void Update () { }

        public void LoadStoryModeLevel (int sceneIndex) {
            GameManager.Instance.LoadScene ("Level" + sceneIndex.ToString (), true);
        }
        public void LoadMainMenu () {
            GameManager.Instance.LoadScene ("MainMenu", false);
        }
        public void RegisterAllUnit () {
            GameManager.Instance.RegisterAllUnit ();
        }

    }
}