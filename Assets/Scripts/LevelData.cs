using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace meleeDemo {

    public enum QuestType {
        KillAllEnemies,
        ReachPosition,
        DestroyTarget

    }

    public enum QuestResult {
        OpenGate,
        Clear
    }

    [System.Serializable]
    public class Quest {
        public QuestType Type;
        public QuestResult Result;
        public GameObject GoalObject;
        public GameObject ResultObject;
        public string TimelineAssetName;
        public List<SpawnUnitInfo> Enemies = new List<SpawnUnitInfo>();
        public int CurrKilledEnemyNum;
        public Quest (string[] ss) {
            switch (ss[1]) {
                case "KAE":
                    Type = QuestType.KillAllEnemies;
                    break;
                case "RP":
                    Type = QuestType.ReachPosition;
                    GoalObject = GameObject.Find (ss[2]);
                    break;
                case "DT":
                    Type = QuestType.DestroyTarget;
                    GoalObject = GameObject.Find (ss[2]);
                    break;
            }
            switch (ss[3]) {
                case "OP":
                    Result = QuestResult.OpenGate;
                    ResultObject = GameObject.Find (ss[4]);
                    break;
                case "C":
                    Result = QuestResult.Clear;
                    break;
            }
            TimelineAssetName = ss[5];

        }
        public void AddEnemyConfig (string[] ss) {
            Vector3 pos = Vector3.zero;
            pos.x = float.Parse (ss[2]);
            pos.y = float.Parse (ss[3]);
            pos.z = float.Parse (ss[4]);
            Vector3 rot = Vector3.zero;
            rot.x = float.Parse (ss[5]);
            rot.y = float.Parse (ss[6]);
            rot.z = float.Parse (ss[7]);
            Enemies.Add (new SpawnUnitInfo (ss[1], pos, rot));

        }

    }

    [System.Serializable]
    public class LevelData {
        public List<Quest> Quests = new List<Quest> ();
        public Vector3 PlayerSpawnPoint;

        public LevelData (string filename) {
            StreamReader sr = new StreamReader (filename);
            string line = "";
            while ((line = sr.ReadLine ()) != null) {
                string[] ss = line.Split (',');
                switch (ss[0]) {
                    case "P":
                        PlayerSpawnPoint.x = float.Parse (ss[1]);
                        PlayerSpawnPoint.y = float.Parse (ss[2]);
                        PlayerSpawnPoint.z = float.Parse (ss[3]);
                        break;
                    case "Q":
                        Quests.Add (new Quest (ss));
                        break;
                    case "E":
                        Quests[Quests.Count - 1].AddEnemyConfig (ss);
                        break;
                }
            }

        }

    }
}