using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace meleeDemo {

    public class GameManager : Singleton<GameManager> {

        private Scene CurrScene;
        public GameObject CurrMainMenuObj;
        public CharacterControl Player;
        public LevelData CurrLevelData;
        public int CurrQuestIndex;
        private Coroutine MainMenuRoutine;
        private Coroutine LoadSceneRoutine;
        public override void Init () {

        }

        void Awake () {
            InitAllManagers ();

        }

        public void InitAllManagers () {
            CurrScene = SceneManager.GetActiveScene ();
            VirtualInputManager.Instance.Init ();
            KeyboardManager.Instance.Init ();
            //Cursor.lockState = CursorLockMode.Locked;
            if (CurrScene.name != "MainMenu") {
                PoolManager.Instance.Init ();
                AttackManager.Instance.Init ();
                CameraManager.Instance.Init ();
                AIAgentManager.Instance.Init ();

            }
        }

        void Update () {
            if (CurrScene.name != "MainMenu" && !CurrLevelData.HasCompleted) {
                if (Input.GetKeyDown (KeyCode.Escape)) {
                    ToggleMainMenu ();

                }
            }
        }

        public void LoadScene (string sceneName, bool notMainMenu) {
            if (LoadSceneRoutine == null)
                LoadSceneRoutine = StartCoroutine (LoadAsyncScene (sceneName, notMainMenu));
        }

        IEnumerator LoadAsyncScene (string sceneName, bool notMainMenu) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync (sceneName);

            while (!asyncLoad.isDone) {
                yield return null;
            }

            CurrScene = SceneManager.GetActiveScene ();
            InitSceneInfo (notMainMenu);
            LoadSceneRoutine = null;
        }

        public void InitSceneInfo (bool notMainMenu) {
            //GameObject playerObj = (FindObjectOfType(typeof(ManualInput)) as ManualInput).gameObject;
            //Player = playerObj.GetComponent<CharacterControl>();
            if (notMainMenu) {
                LoadLevelData ();
                InitAllManagers ();
                CurrMainMenuObj = GameObject.Find ("MainMenu");
                CurrMainMenuObj.SetActive (false);
            }

        }

        public void LoadLevelData () {
            //if (CurrLevelData != null)
            //   Destroy (CurrLevelData);
            CurrQuestIndex = 0;
            string levelDataFileName = "Assets\\Scenes\\" + CurrScene.name + "\\config.csv";
            CurrLevelData = new LevelData (levelDataFileName);
            if (CurrLevelData.Quests != null) {
                BeginCurrentQuest ();
            }

        }
        public void BeginCurrentQuest () {
            Quest CurrQuest = CurrLevelData.Quests[CurrQuestIndex];
            if (CurrQuest.Type == QuestType.KillAllEnemies)
                CurrQuest.CurrKilledEnemyNum = 0;

            GameObject spawnerObj = GameObject.Find ("UnitSpawner");
            UnitSpawner spawner = spawnerObj.GetComponent<UnitSpawner> ();
            spawner.SetSpawnInfoFromCurrQuest (CurrQuest);
            /*
            if (CurrQuest.TimelineAssetName != "") {
                GameObject timelineObj = GameObject.Find ("SceneLoader");
                PlayableDirector pd = timelineObj.GetComponent<PlayableDirector> ();
                string playableName = CurrQuest.TimelineAssetName;
                Debug.Log(playableName);
                PlayableAsset pa = Resources.Load<PlayableAsset> (playableName);
                pd.playableAsset = pa;
                pd.Play ();
            }
            */

        }
        public void ConcludeCurrentQuest () {
            Quest CurrQuest = CurrLevelData.Quests[CurrQuestIndex];
            switch (CurrQuest.Result) {
                case QuestResult.OpenGate:
                    break;
                case QuestResult.Clear:
                    break;
            }
        }

        public void SetPlayer (CharacterControl player) {
            Player = player;
            player.CharacterData.OnDead -= OnUnitDead;
            player.CharacterData.OnDead += OnUnitDead;
        }

        public void RegisterAllUnit () {
            ManualInput playerInput = null;
            if (Player == null) {
                playerInput = FindObjectOfType (typeof (ManualInput)) as ManualInput;
                Player = playerInput.GetComponent<CharacterControl> ();
            }
            playerInput = Player.GetComponent<ManualInput> ();
            playerInput.enabled = true;

            AIAgentManager.Instance.RegisterAllEnemies ();

        }

        public bool CheckCurrQuestComplete () {
            Debug.Log ("check current quest complete");
            Quest CurrQuest = CurrLevelData.Quests[CurrQuestIndex];
            switch (CurrQuest.Type) {
                case QuestType.KillAllEnemies:
                    if (CurrQuest.Enemies.Count == CurrQuest.CurrKilledEnemyNum)
                        return true;
                    else
                        return false;
                case QuestType.DestroyTarget:
                    CharacterControl control = CurrQuest.GoalObject.GetComponent<CharacterControl> ();
                    if (control.CharacterData.IsDead)
                        return true;
                    else
                        return false;
                case QuestType.ReachPosition:
                    Vector3 distVec = Player.gameObject.transform.position - CurrQuest.GoalObject.gameObject.transform.position;
                    distVec.y = 0f;
                    if (distVec.magnitude < 2.0f)
                        return true;
                    else
                        return false;
                default:
                    return false;

            }

        }
        public void OnUnitDead (CharacterControl unit) {
            if (unit == Player) {
                GameOver ();
            } else {
                if (!CurrLevelData.HasCompleted) {
                    Quest CurrQuest = CurrLevelData.Quests[CurrQuestIndex];
                    CurrQuest.CurrKilledEnemyNum++;
                    UpdateState ();
                }
            }
        }

        public void UpdateState () {
            if (CheckCurrQuestComplete ()) {
                if (CurrQuestIndex != CurrLevelData.Quests.Count - 1) {
                    ConcludeCurrentQuest ();
                    CurrQuestIndex++;
                    BeginCurrentQuest ();
                } else {
                    LevelClear ();
                }

            }

        }

        public void GameOver () {
            StartCoroutine (_GameOver ());

        }

        IEnumerator _GameOver () {
            yield return new WaitForSecondsRealtime (3.0f);
            Debug.Log ("Game Over");

        }
        public void LevelClear () {
            CurrLevelData.HasCompleted = true;
            StartCoroutine (_LevelClear ());

        }

        IEnumerator _LevelClear () {
            yield return new WaitForSecondsRealtime (3.0f);
            Debug.Log ("Level Clear");
        }

        public void ToggleMainMenu () {
            if (MainMenuRoutine == null)
                MainMenuRoutine = StartCoroutine (_ToggleMainMenu ());
        }
        IEnumerator _ToggleMainMenu () {
            if (CurrMainMenuObj.activeInHierarchy) {
                ResumeGame ();
            } else {
                PauseGame ();
            }

            yield return new WaitForSecondsRealtime (0.5f);
            MainMenuRoutine = null;
        }

        public void PauseGame () {
            if (Player != null) {
                ManualInput input = Player.GetComponent<ManualInput> ();
                input.enabled = false;
            }
            Time.timeScale = 0.0f;
            if (CurrMainMenuObj != null) {
                CurrMainMenuObj.SetActive (true);
                MenuController controller = CurrMainMenuObj.GetComponent<MenuController> ();
                controller.InitButtonList ();
            }

        }
        public void ResumeGame () {
            VirtualInputManager.Instance.ClearAllInputsInBuffer ();
            if (Player != null) {
                ManualInput input = Player.GetComponent<ManualInput> ();
                input.enabled = true;
            }
            Time.timeScale = 1.0f;
            if (CurrMainMenuObj != null) {
                CurrMainMenuObj.SetActive (false);
            }

        }
    }
}