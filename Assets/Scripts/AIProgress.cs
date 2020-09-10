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
        public PandaBehaviour pandaTree;
        public CharacterControl aiUnit;
        public CharacterControl enemyTarget;
        public TeamTag team;
        public float aquisitionInterval = 1f;
        public float visionRange = 15f;
        public float attackRange = 2f;
        public float RunToWalkRange = 4f;
        public float WalkToRunRange = 8f;
        public float MomentumFactor = 0.1f;
        public float ProbExecute = 0.5f;
        public float ProbAttack = 0.3f;
        public Vector2 inputVectorIncremental = new Vector2 ();

        void Awake () {

            aiUnit = GetComponent<CharacterControl> ();
            animator = GetComponentInChildren<Animator> ();
            pandaTree = GetComponent<PandaBehaviour> ();
            team = aiUnit.CharacterData.Team;
            enemyTarget = null;
            AIAgentManager.Instance.TotalAIAgent.Add (this);

        }

        void Start () {
            GameObject pathFindingAgentObj = PoolManager.Instance.GetObject (PoolObjectType.PathFindingAgent);
            pathFindingAgentObj.SetActive (true);
            pathFindingAgent = pathFindingAgentObj.GetComponent<PathFindingAgent> ();
            pathFindingAgent.transform.position = new Vector3 (gameObject.transform.position.x, 0f, gameObject.transform.position.z);
            pathFindingAgent.Init ();

            // register event
            GameObject playerObj = (FindObjectOfType (typeof (ManualInput)) as ManualInput).gameObject;
            CharacterControl player = playerObj.GetComponent<CharacterControl> ();
            aiUnit.CharacterData.OnDamage += player.OnEnemyGetDamaged; 

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
                    //Debug.Log ("Find player!");
                    CameraManager.Instance.AddToTargetGroup (aiUnit);
                } else {
                    CameraManager.Instance.RemoveFromTargetGroup (aiUnit);
                    enemyTarget = null;
                }
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
            pathFindingAgent.gameObject.transform.position = gameObject.transform.position;
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
            if (Vector3.Distance (enemyTarget.gameObject.transform.position, gameObject.transform.position) < RunToWalkRange)
                return true;
            else
                return false;

        }

        [Task]
        public bool IsTargetOutWalkRange () {
            if (Vector3.Distance (enemyTarget.gameObject.transform.position, gameObject.transform.position) >= WalkToRunRange)
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

        public void TurnOnHighlight (float factor)
        {
            aiUnit.CharacterData.TargetGroupWeight *= factor;
            CameraManager.Instance.UpdateTargetWeight (aiUnit);
        }
        public void TurnOffHighlight (float factor)
        {
            aiUnit.CharacterData.TargetGroupWeight /= factor;
            CameraManager.Instance.UpdateTargetWeight (aiUnit);
        }
 
        public void SetInputVectorToFaceTarget () {
            Vector3 dir = enemyTarget.gameObject.transform.position - gameObject.transform.position;
            dir.y = 0f;
            dir.Normalize ();
            aiUnit.inputVector.x = dir.x;
            aiUnit.inputVector.y = dir.z;
        }
        public void SetInputVector () {
            Vector3 dir = new Vector3 (aiUnit.inputVector.x, 0, aiUnit.inputVector.y);
            Vector3 deltaDir = pathFindingAgent.gameObject.transform.position - gameObject.transform.position;
            //Vector3 dir = pathFindingAgent.gameObject.transform.position - gameObject.transform.position;
            deltaDir.y = 0f;
            deltaDir.Normalize ();
            deltaDir.x += inputVectorIncremental.x;
            deltaDir.z += inputVectorIncremental.y;
            deltaDir.Normalize ();
            dir.x += deltaDir.x * MomentumFactor;
            dir.z += deltaDir.z * MomentumFactor;
            dir.Normalize ();
            aiUnit.inputVector.x = dir.x;
            aiUnit.inputVector.y = dir.z;

            Vector3 forward = gameObject.transform.forward;
            forward.y = 0f;
            float angle = (Quaternion.LookRotation (forward, Vector3.up)).eulerAngles.y * Mathf.Deg2Rad;
            float cosA = Mathf.Cos (angle);
            float sinA = Mathf.Sin (angle);
            //Debug.Log(forward.ToString() + " " + Mathf.Cos(angle).ToString() + " " + Mathf.Sin(angle).ToString());
            float blendTreeInputX = aiUnit.inputVector.x * cosA - aiUnit.inputVector.y * sinA;
            float blendTreeInputY = aiUnit.inputVector.x * sinA + aiUnit.inputVector.y * cosA;
            animator.SetFloat ("InputX", blendTreeInputX);
            animator.SetFloat ("InputY", blendTreeInputY);

            //Vector3 finalDir = new Vector3(aiUnit.inputVector.x, 0, aiUnit.inputVector.y);
            //Debug.DrawRay(gameObject.transform.position, finalDir * 10f, Color.red);

        }
        public void ResetInputVector () {
            aiUnit.inputVector.x = 0f;
            aiUnit.inputVector.y = 0f;
        }

        [Task]
        public bool RandomAttack () {
            float r = Random.Range (0f, 1f);
            if (r < ProbAttack) {
                StopMove ();
                AttackCommandInput ();
            }
            return true;
        }

        [Task]
        public bool RandomExecute () {
            float r = Random.Range (0f, 1f);
            if (r < ProbExecute) {
                StopMove ();
                ExecuteCommandInput ();
            }
            return true;
        }

        [Task]
        public bool ExecuteCommandInput () {
            SetInputVectorToFaceTarget ();
            /*
            Vector3 direction = enemyTarget.gameObject.transform.position - gameObject.transform.position;
            direction.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            animator.transform.root.rotation = rotation;// Quaternion.Slerp(animator.transform.root.rotation, rotation, Time.deltaTime * 50f);
           
            */
            aiUnit.CommandExecute = true;
            return true;

        }

        [Task]
        public bool AttackCommandInput () {
            SetInputVectorToFaceTarget ();
            /*
            Vector3 direction = enemyTarget.gameObject.transform.position - gameObject.transform.position;
            direction.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            animator.transform.root.rotation = rotation;// Quaternion.Slerp(animator.transform.root.rotation, rotation, Time.deltaTime * 50f);
            */
            aiUnit.CommandAttack = true;
            return true;

        }

        [Task]
        public bool ExecuteCommandCancel () {
            ResetInputVector ();
            aiUnit.CommandExecute = false;
            return true;
        }

        [Task]
        public bool AttackCommandCancel () {
            ResetInputVector ();
            aiUnit.CommandAttack = false;
            return true;
        }

        [Task]
        public bool IsRunning () {
            return animator.GetBool ("IsRunning");
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

        public void Dead () {
            pandaTree.enabled = false;
            pathFindingAgent.Dead ();
            CameraManager.Instance.RemoveFromTargetGroup (aiUnit);

        }
        void OnDrawGizmos () {
            //Gizmos.DrawWireCube(aiUnit.gameObject.transform.position, aiUnit.gameObject.transform.forward * 1f,  new Vector3(1.0f, 1.0f, 0.2f));
        }
    }
}