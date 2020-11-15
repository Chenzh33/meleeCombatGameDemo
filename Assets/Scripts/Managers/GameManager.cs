using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace meleeDemo {

    public class GameManager : Singleton<GameManager> {

        private Scene CurrScene;
        public Canvas CurrHUD;
        public CharacterControl Player;
        public LevelData CurrLevelData;
        public int CurrQuestIndex;
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

        void Update () { }

        public void LoadScene (string sceneName, bool InitCharacter) {
            StartCoroutine (LoadAsyncScene (sceneName, InitCharacter));
        }

        IEnumerator LoadAsyncScene (string sceneName, bool InitCharacter) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync (sceneName);

            while (!asyncLoad.isDone) {
                yield return null;
            }

            CurrScene = SceneManager.GetActiveScene ();
            InitSceneInfo (InitCharacter);
        }

        public void InitSceneInfo (bool InitCharacter) {
            //GameObject playerObj = (FindObjectOfType(typeof(ManualInput)) as ManualInput).gameObject;
            //Player = playerObj.GetComponent<CharacterControl>();
            LoadLevelData ();
            InitAllManagers ();

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
            UnitSpawner spawner = spawnerObj.GetComponent<UnitSpawner>();
            spawner.SetSpawnInfoFromCurrQuest(CurrQuest);
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
                Quest CurrQuest = CurrLevelData.Quests[CurrQuestIndex];
                CurrQuest.CurrKilledEnemyNum++;
                UpdateState ();
            }
        }

        public void UpdateState () {
            if (CheckCurrQuestComplete ()) {
                if (CurrQuestIndex != CurrLevelData.Quests.Count - 1) {
                    ConcludeCurrentQuest ();
                    CurrQuestIndex++;
                    BeginCurrentQuest ();
                } else
                    LevelClear ();

            }

        }

        public void GameOver () {
            StartCoroutine (_GameOver ());

        }

        IEnumerator _GameOver () {
            yield return new WaitForSeconds (3.0f);
            Debug.Log ("Game Over");

        }
        public void LevelClear () {
            StartCoroutine (_LevelClear ());

        }

        IEnumerator _LevelClear () {
            yield return new WaitForSeconds (3.0f);
            Debug.Log ("Level Clear");
        }
    }
}