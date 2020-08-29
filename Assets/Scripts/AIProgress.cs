using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;
using UnityEngine.AI;

namespace meleeDemo {

    public class AIProgress : MonoBehaviour {
        //public NavMeshAgent navMeshAgent;
        public Animator animator;
        public PathFindingAgent pathFindingAgent;
        public CharacterControl aiUnit;
        public CharacterControl enemyTarget;
        public TeamTag team;
        public float aquisitionInterval = 1f;
        public float visionRange = 15f;
        public float attackRange = 2f;
        public float WalkRange = 5f;

        void Awake () {

            aiUnit = GetComponent<CharacterControl> ();
            animator = GetComponentInChildren<Animator> ();
            team = aiUnit.CharacterData.Team;
            enemyTarget = null;

        }

        void Start () {
            GameObject pathFindingAgentObj = PoolManager.Instance.GetObject (PoolObjectType.PATH_FINDING_AGENT);
            pathFindingAgent = pathFindingAgentObj.GetComponent<PathFindingAgent> ();
            pathFindingAgent.transform.position = gameObject.transform.position;
            pathFindingAgent.Init ();
        }

        // simple version
        float lastFindTargetTime = float.NegativeInfinity;

        [Task]
        public void FindTarget () {
            if (Time.time - lastFindTargetTime > aquisitionInterval) {

                GameObject player = (FindObjectOfType (typeof (ManualInput)) as ManualInput).gameObject;
                enemyTarget = player.GetComponent<CharacterControl> ();
                if (IsTargetVisible () && enemyTarget.CharacterData.Team != team) {
                    pathFindingAgent.SetTarget (enemyTarget.gameObject.transform);
                    Debug.Log ("Find player!");
                } else
                    enemyTarget = null;
                lastFindTargetTime = Time.time;
            }
            Task.current.Complete (enemyTarget != null);
        }

        [Task]
        public bool IsTargetVisible () {
            if (enemyTarget == null)
                return false;
            if (Vector3.Distance (enemyTarget.gameObject.transform.position, gameObject.transform.position) < visionRange)
                return true;
            else
                return false;

        }

        [Task]
        public void ChaseTarget () {
            pathFindingAgent.StartNav ();
            pathFindingAgent.GoToTarget ();
            WaitForArrival ();

        }

        [Task]
        public bool HasTarget () {
            return (enemyTarget != null);
        }

        [Task]
        public bool ClearTarget () {
            enemyTarget = null;
            pathFindingAgent.Stop ();
            pathFindingAgent.Target = null;
            return true;
        }

        [Task]
        public bool RunToWalk () {
            animator.SetBool ("IsRunning", false);
            aiUnit.CharacterData.IsRunning = false;
            return true;
        }

        [Task]
        public bool WalkToRun () {
            animator.SetBool ("IsRunning", true);
            aiUnit.CharacterData.IsRunning = true;
            return true;
        }

        [Task]
        public void WaitForArrival () {
            var task = Task.current;
            if (!task.isStarting && Vector3.Distance (pathFindingAgent.gameObject.transform.position, gameObject.transform.position) < 2f) {
                //pathFindingAgent.Stop ();
                task.Succeed ();
            }
        }

        [Task]
        public bool IsTargetInAttackRange () {
            if (enemyTarget == null)
                return false;
            if (Vector3.Distance (enemyTarget.gameObject.transform.position, gameObject.transform.position) < attackRange)
                return true;
            else
                return false;

        }

        [Task]
        public bool IsTargetInWalkRange () {
            if (Vector3.Distance (enemyTarget.gameObject.transform.position, gameObject.transform.position) < WalkRange)
                return true;
            else
                return false;

        }


        void Update () {

/*
            if (!pathFindingAgent.IsStopped ())
                SetInputVector ();
            else
                ResetInputVector ();
                */

        }

        public void SetInputVector () {
            Vector3 dir = pathFindingAgent.gameObject.transform.position - gameObject.transform.position;
            dir.y = 0f;
            dir.Normalize ();
            aiUnit.inputVector.x = dir.x;
            aiUnit.inputVector.y = dir.z;

        }
        public void ResetInputVector () {
            aiUnit.inputVector.x = 0f;
            aiUnit.inputVector.y = 0f;
        }

        [Task]
        public bool AttackCommandInput () {
            SetInputVector ();
            aiUnit.CommandAttack = true;
            return true;

        }

        [Task]
        public bool AttackCommandCancel () {
            ResetInputVector ();
            aiUnit.CommandAttack = false;
            return true;
        }

        [Task]
        public bool StopMove () {
            ResetInputVector ();
            pathFindingAgent.Stop ();
            return true;

        }

        [Task]
        public bool IsDead () {
            return aiUnit.CharacterData.IsDead;
        }
    }
}