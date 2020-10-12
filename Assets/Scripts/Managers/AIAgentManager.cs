using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class AIAgentManager : Singleton<AIAgentManager> {

        public List<AIProgress> TotalAIAgent = new List<AIProgress> ();
        public List<AIProgress> CurrentAgentCrowd = new List<AIProgress> ();

        public override void Init () {
            AIProgress[] allAIAgent = FindObjectsOfType (typeof (AIProgress)) as AIProgress[];
            foreach (AIProgress ai in allAIAgent) {
                TotalAIAgent.Add (ai);
            }
        }

        public void RegisterAllEnemies () {
            foreach (AIProgress ai in TotalAIAgent) {
                ai.enabled = true;
                ai.RegisterDamageEvent ();
            }
        }

        /*
                public List<AIProgress> GetCurrentSwarmAgent (CharacterControl target) {
                    foreach (AIProgress ai in TotalAIAgent) {
                        if (!ai.aiUnit.CharacterData.IsRunning && ai.enemyTarget == target) {
                            if (!CurrentSwarmAgent.Contains (ai))
                                CurrentSwarmAgent.Add (ai);
                        } else
                        if (CurrentSwarmAgent.Contains (ai))
                            CurrentSwarmAgent.Remove (ai);
                    }
                    return CurrentSwarmAgent;
                }
                */

    }
}