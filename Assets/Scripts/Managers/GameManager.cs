﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace meleeDemo {

    public class GameManager : Singleton<GameManager> {

        private Scene CurrScene;
        public Canvas CurrHUD;
        public CharacterControl Player;
        public override void Init()
        {
           
        }

        void Awake()
        {
            InitAllManagers();

        }

        public void InitAllManagers()
        {
            CurrScene = SceneManager.GetActiveScene();
            VirtualInputManager.Instance.Init();
            KeyboardManager.Instance.Init();
            if (CurrScene.name != "MainMenu")
            {
                PoolManager.Instance.Init();
                AttackManager.Instance.Init();
                CameraManager.Instance.Init();
                AIAgentManager.Instance.Init();

            }
        }

        void Update () {
        }

        public void LoadScene(string sceneName, bool InitCharacter)
        {
            StartCoroutine(LoadAsyncScene(sceneName, InitCharacter));
        }

        IEnumerator LoadAsyncScene(string sceneName, bool InitCharacter)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            CurrScene = SceneManager.GetActiveScene();
            InitSceneInfo(InitCharacter);
        }

        public void InitSceneInfo(bool InitCharacter)
        {
            //GameObject playerObj = (FindObjectOfType(typeof(ManualInput)) as ManualInput).gameObject;
            //Player = playerObj.GetComponent<CharacterControl>();
            InitAllManagers();

        }
        public void SetPlayer(CharacterControl player)
        {
            Player = player;
        }

        public void RegisterAllUnit()
        {
            AIAgentManager.Instance.RegisterAllEnemies ();
            ManualInput playerInput = Player.GetComponent<ManualInput>();
            playerInput.enabled = true;

        }

    }
}