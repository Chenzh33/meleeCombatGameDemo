﻿using System.Collections;
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
        public float closeRange = 0f;
        public float RunToWalkRange = 4f;
        public float WalkToRunRange = 8f;
        public float MomentumFactor = 0.1f;
        public float AITransition = 0f;
        public float FearArmourPercentThreshold = 0.4f;
        public float FearHealthPercentThreshold = 0.2f;
        public float LastGetHitTime;
        public float LastTargetAttackTime;
        public float LastTargetExecuteTime;
        public float LastTargetDodgeTime;
        public float LastTargetGuardTime;
        public bool IsInCrowd;
        public bool IsInFear;
        public int HitCountOnGuard;
        public Vector2 inputVectorIncremental = new Vector2 ();

        void Awake () {

            aiUnit = GetComponent<CharacterControl> ();
            animator = GetComponentInChildren<Animator> ();
            pandaTree = GetComponent<PandaBehaviour> ();
            team = aiUnit.CharacterData.Team;
            LastGetHitTime = Mathf.NegativeInfinity;
            LastTargetAttackTime = Mathf.NegativeInfinity;
            LastTargetExecuteTime = Mathf.NegativeInfinity;
            LastTargetDodgeTime = Mathf.NegativeInfinity;
            LastTargetGuardTime = Mathf.NegativeInfinity;
            enemyTarget = null;
            AIAgentManager.Instance.TotalAIAgent.Add (this);

        }

        void Start () {
            GameObject pathFindingAgentObj = PoolManager.Instance.GetObject (PoolObjectType.PathFindingAgent);
            pathFindingAgentObj.SetActive (true);
            pathFindingAgent = pathFindingAgentObj.GetComponent<PathFindingAgent> ();
            pathFindingAgent.transform.position = new Vector3 (gameObject.transform.position.x, 0f, gameObject.transform.position.z);
            pathFindingAgent.Init ();
            //RegisterDamageEvent ();

            // register event
        }

        public void RegisterDamageEvent () {
            //Debug.Log ("register damage event!");
            ManualInput playerObjManu = FindObjectOfType (typeof (ManualInput)) as ManualInput;
            if (playerObjManu != null) {
                CharacterControl player = playerObjManu.GetComponent<CharacterControl> ();
                /*
                foreach(Delegate exist in aiUnit.CharacterData.OnDamage.GetInvocationList())
                {
                    if(player.OnEnemyGetDamaged == exist)
                        return;
                }
                */
                aiUnit.CharacterData.OnDamage -= player.OnEnemyGetDamaged;
                aiUnit.CharacterData.OnDamage += player.OnEnemyGetDamaged;
                aiUnit.CharacterData.OnDead -= GameManager.Instance.OnUnitDead;
                aiUnit.CharacterData.OnDead += GameManager.Instance.OnUnitDead;
            }

        }
        public void RegisterTargetActionEvent (CharacterControl target) {
            Debug.Log ("register target action event!");
            if (target != null) {
                target.OnAction -= this.OnTargetAction;
                target.OnAction += this.OnTargetAction;
            }

        }
        public void DeregisterTargetActionEvent (CharacterControl target) {
            Debug.Log ("deregister target action event!");
            if (target != null) {
                target.OnAction -= this.OnTargetAction;
            }

        }
        public void OnTargetAction (InputKeyType type) {
            switch (type) {
                case InputKeyType.KEY_MELEE_ATTACK:
                    LastTargetAttackTime = Time.time;
                    break;
                case InputKeyType.KEY_EXECUTE_ATTACK:
                    LastTargetExecuteTime = Time.time;
                    break;
                case InputKeyType.KEY_DODGE:
                    LastTargetDodgeTime = Time.time;
                    break;
                case InputKeyType.KEY_GUARD:
                    LastTargetGuardTime = Time.time;
                    break;
            }

        }
        // simple version
        float lastFindTargetTime = float.NegativeInfinity;

        [Task]
        public void FindTarget () {
            if (Time.time - lastFindTargetTime > aquisitionInterval) {

                //GameObject player = (FindObjectOfType (typeof (ManualInput)) as ManualInput).gameObject;
                ManualInput playerObjManu = FindObjectOfType (typeof (ManualInput)) as ManualInput;
                if (playerObjManu != null && this.enabled == true) {
                    enemyTarget = playerObjManu.GetComponent<CharacterControl> ();
                    if (IsTargetVisible () && enemyTarget.CharacterData.Team != team) {
                        pathFindingAgent.SetTarget (enemyTarget.gameObject.transform);
                        //Debug.Log ("Find player!");
                        CameraManager.Instance.AddToTargetGroup (aiUnit);
                        RegisterTargetActionEvent (enemyTarget);
                    } else {
                        CameraManager.Instance.RemoveFromTargetGroup (aiUnit);
                        DeregisterTargetActionEvent (enemyTarget);
                        enemyTarget = null;
                    }
                    lastFindTargetTime = Time.time;
                } else
                    Task.current.Fail ();
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
        public void ForceEndCurrentTask () {
            Task.current.Fail ();
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
        public void WaitForArrival (float range) {
            var task = Task.current;
            if (!task.isStarting && Vector3.Distance (pathFindingAgent.gameObject.transform.position, gameObject.transform.position) < range) {
                //pathFindingAgent.Stop ();
                task.Succeed ();
            }
        }

        [Task]
        public bool HealthPercentLowerThan (float count) {
            float percent = aiUnit.CharacterData.HP / aiUnit.CharacterData.MaxHP;
            if (percent > count)
                return false;
            else
                return true;
        }

        [Task]
        public bool ArmourPercentLowerThan (float count) {
            float percent = aiUnit.CharacterData.Armour / aiUnit.CharacterData.MaxArmour;
            if (percent > count)
                return false;
            else
                return true;
        }

        [Task]
        public bool GetHit () {
            if (aiUnit.CharacterData.GetHitTime > 0f)
                return true;
            else
                return false;
        }

        [Task]
        public void WaitForTransition (int index) {
            var task = Task.current;
            if (!task.isStarting && animator.GetInteger (TransitionParameter.TransitionIndexer.ToString ()) == index) {
                task.Succeed ();
            }
        }

        [Task]
        public void WaitForTransitionNotLessThan (int index) {
            var task = Task.current;
            if (!task.isStarting && animator.GetInteger (TransitionParameter.TransitionIndexer.ToString ()) >= index) {
                task.Succeed ();
            }
        }

        [Task]
        public void WaitForForcedDodgeTransition () {
            var task = Task.current;
            if (!task.isStarting && animator.GetBool (TransitionParameter.ForcedTransitionDodge.ToString ())) {
                task.Succeed ();
            }
        }

        [Task]
        public bool LastGetHitTimeLessThan (float duration) {
            float t = Time.time;
            return (t - LastGetHitTime < duration);

        }

        [Task]
        public bool LastTargetAttackTimeLessThan (float duration) {
            float t = Time.time;
            return (t - LastTargetAttackTime < duration);
        }

        [Task]
        public bool LastTargetExecuteTimeLessThan (float duration) {
            float t = Time.time;
            return (t - LastTargetExecuteTime < duration);
        }

        [Task]
        public bool LastTargetDodgeTimeLessThan (float duration) {
            float t = Time.time;
            return (t - LastTargetDodgeTime < duration);
        }

        [Task]
        public bool LastTargetGuardTimeLessThan (float duration) {
            float t = Time.time;
            return (t - LastTargetGuardTime < duration);
        }

        [Task]
        public void RandomOrbitalDodge () {
            int dir = Random.Range (0, 2);
            if (dir == 0)
                animator.Play ("DodgeLeft");
            else
                animator.Play ("DodgeRight");
            Task.current.Succeed ();

        }

        [Task]
        public void DodgeFront () {
            animator.Play ("DodgeFront");
            Task.current.Succeed ();

        }

        [Task]
        public void DodgeBack () {
            animator.Play ("DodgeBack");
            Task.current.Succeed ();

        }

        [Task]
        public void SetInCrowd (bool inCrowd) {
            IsInCrowd = inCrowd;
            Task.current.Succeed ();

        }

        [Task]
        public bool CheckAITransitionInRange (float x1, float x2) {
            if (AITransition > x1 && AITransition <= x2)
                return true;
            else
                return false;

        }

        [Task]
        public bool IsTargetInRange (float range) {
            if (enemyTarget == null)
                return false;
            if (Vector3.Distance (enemyTarget.gameObject.transform.position, gameObject.transform.position) < range)
                return true;
            else
                return false;

        }

        [Task]
        public bool IsTargetInFacingRange (float AngleRange) {
            if (enemyTarget == null)
                return false;
            Vector3 diffVectorRaw = enemyTarget.transform.position - aiUnit.transform.position;
            Vector3 diffVector = new Vector3 (diffVectorRaw.x, 0f, diffVectorRaw.z);
            Quaternion rotEnemy = Quaternion.LookRotation (diffVector.normalized, Vector3.up);
            Quaternion rotSelf = Quaternion.LookRotation (aiUnit.transform.forward, Vector3.up);
            float AbsAngle = Mathf.Abs (Quaternion.Angle (rotEnemy, rotSelf));

            if (AbsAngle <= AngleRange)
                return true;
            else
                return false;

        }

        [Task]
        public bool IsTargetFacingSelf (float AngleRange) {
            if (enemyTarget == null)
                return false;
            Vector3 diffVectorRaw = aiUnit.transform.position - enemyTarget.transform.position;
            Vector3 diffVector = new Vector3 (diffVectorRaw.x, 0f, diffVectorRaw.z);
            Quaternion rotEnemy = Quaternion.LookRotation (diffVector.normalized, Vector3.up);
            Quaternion rotSelf = Quaternion.LookRotation (enemyTarget.transform.forward, Vector3.up);
            float AbsAngle = Mathf.Abs (Quaternion.Angle (rotEnemy, rotSelf));

            if (AbsAngle <= AngleRange)
                return true;
            else
                return false;

        }
        /*
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

                [Task]
                public bool IsTargetInCloseRange () {
                    if (enemyTarget == null)
                        return false;
                    if (Vector3.Distance (enemyTarget.gameObject.transform.position, gameObject.transform.position) < closeRange)
                        return true;
                    else
                        return false;

                }
                */
        void Update () {

            /*
                        if (!pathFindingAgent.IsStopped ())
                            SetInputVector ();
                        else
                            ResetInputVector ();
                            */

        }

        public void TurnOnHighlight (float factor) {
            aiUnit.CharacterData.TargetGroupWeight *= factor;
            CameraManager.Instance.UpdateTargetWeight (aiUnit);
        }
        public void TurnOffHighlight (float factor) {
            aiUnit.CharacterData.TargetGroupWeight /= factor;
            CameraManager.Instance.UpdateTargetWeight (aiUnit);
        }
        public void UpdateFearState () {
            if (HealthPercentLowerThan (FearHealthPercentThreshold) || ArmourPercentLowerThan (FearArmourPercentThreshold))
                IsInFear = true;
            else
                IsInFear = false;
        }

        [Task]
        public bool SetInputVectorToFaceTarget () {
            Vector3 dir = enemyTarget.gameObject.transform.position - gameObject.transform.position;
            dir.y = 0f;
            dir.Normalize ();
            aiUnit.inputVector.x = dir.x;
            aiUnit.inputVector.y = dir.z;
            return true;
        }

        [Task]
        public bool SetInputVectorBackToTarget () {
            Vector3 dir = enemyTarget.gameObject.transform.position - gameObject.transform.position;
            dir.y = 0f;
            dir.Normalize ();
            Vector3 runawayDir = -dir;
            aiUnit.inputVector.x = runawayDir.x;
            aiUnit.inputVector.y = runawayDir.z;
            animator.SetFloat ("InputX", runawayDir.x);
            animator.SetFloat ("InputY", runawayDir.z);
            return true;
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

            Vector3 finalDir = new Vector3 (aiUnit.inputVector.x, 0, aiUnit.inputVector.y);
            Debug.DrawRay (gameObject.transform.position, finalDir * 10f, Color.red);

        }

        [Task]
        public bool CheckInFear () {
            return IsInFear;
        }

        [Task]
        public bool UpdateFearStateTask () {
            UpdateFearState ();
            return true;
        }

        [Task]
        public bool CheckInIdleState () {
            return animator.GetBool ("IdleState");

        }

        [Task]
        public bool GenerateAITransition () {
            AITransition = Random.Range (0f, 1f);
            return true;

        }

        [Task]
        public bool ResetAITransition () {
            AITransition = 0f;
            return true;

        }

        [Task]
        public bool ResetInputTask () {
            ResetInput ();
            return true;
        }

        public void ResetInput () {
            ResetInputVector ();
            aiUnit.ResetAllCommand ();
        }
        public void ResetInputVector () {
            aiUnit.inputVector.x = 0f;
            aiUnit.inputVector.y = 0f;
            this.inputVectorIncremental.x = 0f;
            this.inputVectorIncremental.y = 0f;
        }

        [Task]
        public bool ExecuteCommandInput () {
            aiUnit.CommandExecute = true;
            return true;

        }

        [Task]
        public bool AttackCommandInput () {
            aiUnit.CommandAttack = true;
            return true;

        }

        [Task]
        public bool FireCommandInput () {
            aiUnit.CommandFire = true;
            return true;
        }

        [Task]
        public bool DodgeCommandInput () {
            aiUnit.CommandDodge = true;
            return true;
        }

        [Task]
        public bool GuardCommandInput () {
            aiUnit.CommandGuard = true;
            return true;
        }

        [Task]
        public bool ExecuteCommandCancel () {
            aiUnit.CommandExecute = false;
            return true;
        }

        [Task]
        public bool AttackCommandCancel () {
            aiUnit.CommandAttack = false;
            return true;
        }

        [Task]
        public bool FireCommandCancel () {
            aiUnit.CommandFire = false;
            return true;
        }

        [Task]
        public bool DodgeCommandCancel () {
            aiUnit.CommandDodge = false;
            return true;
        }

        [Task]
        public bool GuardCommandCancel () {
            aiUnit.CommandGuard = false;
            return true;
        }

        [Task]
        public bool IsRunning () {
            return animator.GetBool ("IsRunning");
        }

        [Task]
        public bool BeginMove () {
            pathFindingAgent.StartNav ();
            pathFindingAgent.GoToTarget ();
            return true;
        }

        [Task]
        public void KeepMove (float range) {
            if (pathFindingAgent != null) {
                pathFindingAgent.StartNav ();
                pathFindingAgent.GoToTarget ();
                WaitForArrival (range);
            }
        }

        [Task]
        public bool StopMove () {
            ResetInputVector ();
            pathFindingAgent.Stop ();
            return true;

        }

        [Task]
        public bool DodgeNotInCooldown () {
            return !animator.GetBool (TransitionParameter.ForbidDodge.ToString ());

        }

        [Task]
        public bool GuardNotInCooldown () {
            return !animator.GetBool (TransitionParameter.ForbidGuard.ToString ());

        }


        [Task]
        public bool IsDead () {
            return aiUnit.CharacterData.IsDead;
        }

        public void Dead () {
            pandaTree.enabled = false;
            pathFindingAgent.Dead ();
            if (AIAgentManager.Instance.TotalAIAgent.Contains (this))
                AIAgentManager.Instance.TotalAIAgent.Remove (this);
            if (AIAgentManager.Instance.CurrentAgentCrowd.Contains (this))
                AIAgentManager.Instance.CurrentAgentCrowd.Remove (this);
            CameraManager.Instance.RemoveFromTargetGroup (aiUnit);

        }
        void OnDrawGizmos () {
            //Gizmos.DrawWireCube(aiUnit.gameObject.transform.position, aiUnit.gameObject.transform.forward * 1f,  new Vector3(1.0f, 1.0f, 0.2f));
        }
    }
}