using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ChangeDirection")]
    public class ChangeDirection : SkillEffect {
        public bool AllowEarlyTurn;
        public bool AutoFaceEnemy;
        public bool FacePrevTargetWhenNeutral;
        public bool FacePrevTargetRegardlessOfPosition;
        public bool CheckFarRange;
        public bool AutoFaceStunnedEnemy;
        //public bool ContinuallyFacing;
        //[Range (0f, 1f)]
        //public float AllowEarlyTurnTiming = 0f;
        [Range (0f, 3f)]
        public float AutoFaceEnemyTiming = 0f;
        [Range (0f, 3f)]
        public float AutoFaceEnemyDuration = 0f;

        public float CaptureDistanceFar = 8f;
        public float CaptureAngleRange = 30f;

        public float CaptureDistanceNear = 3f;
        //public float CaptureAngleRangeNear = 60f;
        public float smoothEarlyTurn = 20f;
        public float LockOnTargetDuration = 1.0f;


        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            bool IsFaceForward = true;
            bool KeepFaceFormerTarget = false;
            Vector3 inputDirection = Vector3.zero;
            Vector2 inputDirection2d = stateEffect.CharacterControl.inputVector;
            if (AllowEarlyTurn && inputDirection2d.magnitude > 0.01f) {
                inputDirection = new Vector3 (inputDirection2d.x, 0, inputDirection2d.y);
                stateEffect.CharacterControl.FaceTarget = inputDirection;
                IsFaceForward = false;
            } else if (FacePrevTargetWhenNeutral) {
                if (stateEffect.CharacterControl.CharacterData.FormerAttackTarget != null && !stateEffect.CharacterControl.CharacterData.FormerAttackTarget.CharacterData.IsDead) {
                    Vector3 diffVectorRaw = stateEffect.CharacterControl.CharacterData.FormerAttackTarget.gameObject.transform.position - stateEffect.CharacterControl.gameObject.transform.position;
                    Vector3 diffVector = new Vector3 (diffVectorRaw.x, 0f, diffVectorRaw.z);
                    Vector2 diffVector2d = new Vector2 (diffVector.x, diffVector.z);
                    Quaternion rotEnemy = Quaternion.LookRotation (diffVector, Vector3.up);
                    Quaternion rotSelf = Quaternion.LookRotation (animator.transform.root.forward, Vector3.up);
                    float Dist = diffVector2d.magnitude;
                    float AbsAngle = Mathf.Abs (Quaternion.Angle (rotEnemy, rotSelf));
                    if (FacePrevTargetRegardlessOfPosition ||
                        (Dist <= CaptureDistanceFar && Dist > CaptureDistanceNear && AbsAngle <= CaptureAngleRange)) {
                        stateEffect.CharacterControl.FaceTarget = diffVector.normalized;
                        KeepFaceFormerTarget = true;
                        IsFaceForward = false;
                        stateEffect.CharacterControl.SetFormerTarget (stateEffect.CharacterControl.CharacterData.FormerAttackTarget, LockOnTargetDuration);
                        stateEffect.CharacterControl.TurnToTarget (0f, 0f);
                    } else {
                        stateEffect.CharacterControl.CharacterData.FormerAttackTarget = null;
                    }
                } else {
                    stateEffect.CharacterControl.CharacterData.FormerAttackTarget = null;
                }
            }

            if (AutoFaceEnemy) {
                if (!KeepFaceFormerTarget || AutoFaceStunnedEnemy) {
                    if (!FacePrevTargetWhenNeutral || !IsFaceForward || stateEffect.CharacterControl.CharacterData.FormerAttackTarget == null)
                    {
                        Vector3 initFaceDirection = new Vector3();
                        if (!IsFaceForward)
                            initFaceDirection = inputDirection;
                        else
                            initFaceDirection = animator.transform.root.forward;

                        //stateEffect.CharacterControl.CharacterData.FormerAttackTarget = null;
                        bool EnemyCaptured = FaceEnemy(initFaceDirection, stateEffect, animator, stateInfo);
                        if (EnemyCaptured)
                            IsFaceForward = false;
                    }
                }
            }

            if (IsFaceForward) {
                stateEffect.CharacterControl.FaceTarget = animator.transform.root.forward;
                //stateEffect.CharacterControl.CharacterData.FormerAttackTarget = null;
            }


            stateEffect.CharacterControl.TurnToTarget (stateInfo.length * AutoFaceEnemyTiming, smoothEarlyTurn);

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (AutoFaceEnemyDuration > 0f && stateInfo.normalizedTime < AutoFaceEnemyTiming + AutoFaceEnemyDuration) {
                Vector3 initFaceDirection = animator.transform.root.forward;
                FaceEnemy (initFaceDirection, stateEffect, animator, stateInfo);
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

            //stateEffect.CharacterControl.FaceTarget = animator.transform.root.forward;
        }

        public bool FaceEnemy (Vector3 initFaceDirection, StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            Vector3 direction = Vector3.zero;
            bool EnemyCaptured = false;
            CharacterControl capturedTarget = null;

            RaycastHit rayCastHit;
            int layerMask = 0;
            if (stateEffect.CharacterControl.gameObject.layer == LayerMask.NameToLayer ("player"))
                layerMask = 1 << 9;
            else if (stateEffect.CharacterControl.gameObject.layer == LayerMask.NameToLayer ("enemy"))
                layerMask = 1 << 8;
            bool hit = Physics.BoxCast (stateEffect.CharacterControl.gameObject.transform.position, 2.0f * new Vector3 (1.0f, 1.0f, 0.5f),
                initFaceDirection,
                out rayCastHit,
                Quaternion.LookRotation (initFaceDirection, Vector3.up),
                CaptureDistanceNear, layerMask);
            //Debug.Log (hit);
            if (hit) {
                EnemyCaptured = true;
                capturedTarget = rayCastHit.collider.transform.root.gameObject.GetComponent<CharacterControl> ();
                Vector3 diffVectorRaw = capturedTarget.transform.position - stateEffect.CharacterControl.gameObject.transform.position;
                Vector3 diffVector = new Vector3 (diffVectorRaw.x, 0f, diffVectorRaw.z);
                direction = diffVector.normalized;

                Debug.Log("Capture NEAR Range Enemy!");
            } 
            if((!hit || AutoFaceStunnedEnemy ) && animator.GetBool(TransitionParameter.EnergyTaken.ToString()) || CheckFarRange) {
                //Debug.Log("test far range enemy capture");
                List<GameObject> enemyObjs = new List<GameObject> ();
                if (stateEffect.CharacterControl.isPlayerControl) {
                    foreach (AIProgress ai in AIAgentManager.Instance.TotalAIAgent) {
                        enemyObjs.Add (ai.gameObject);
                    }
                } else {
                    // AI controlled
                    enemyObjs.Add (stateEffect.CharacterControl.AIProgress.enemyTarget.gameObject);
                }

                // need update
                // get enemy list

                float finalDist = Mathf.Infinity;
                float stunnedEnemyDist = Mathf.Infinity;
                Vector3 directionFar = Vector3.zero;
                Vector3 directionStunned = Vector3.zero;
                CharacterControl nearestStunnedEnemy = null;
                CharacterControl farRangeCapturedEnemy = null;
                foreach (GameObject enemy in enemyObjs) {
                    Vector3 diffVectorRaw = enemy.transform.position - stateEffect.CharacterControl.gameObject.transform.position;
                    Vector3 diffVector = new Vector3 (diffVectorRaw.x, 0f, diffVectorRaw.z);
                    Vector2 diffVector2d = new Vector2 (diffVector.x, diffVector.z);
                    Quaternion rotEnemy = Quaternion.LookRotation (diffVector, Vector3.up);
                    Quaternion rotSelf = Quaternion.LookRotation (initFaceDirection, Vector3.up);

                    CharacterControl currentTarget = enemy.GetComponent<CharacterControl> ();

                    //Debug.Log ("dist = " + diffVector2d.magnitude.ToString ());
                    //Debug.Log ("rot = " + Quaternion.Angle (rotEnemy, rotSelf).ToString ());

                    float Dist = diffVector2d.magnitude;
                    float AbsAngle = Mathf.Abs (Quaternion.Angle (rotEnemy, rotSelf));

                    Debug.Log (Dist.ToString () + " --- " + AbsAngle.ToString());

                    if (Dist <= CaptureDistanceFar && AbsAngle <= CaptureAngleRange) {
                        if (AutoFaceStunnedEnemy && currentTarget.CharacterData.IsStunned && Dist < stunnedEnemyDist)
                        {
                            EnemyCaptured = true;
                            directionStunned = diffVector.normalized;
                            stunnedEnemyDist = Dist;
                            nearestStunnedEnemy = currentTarget;
                            //Debug.Log("enemy captured!");
                        }
                        if (Dist < finalDist)
                        {
                            EnemyCaptured = true;
                            directionFar = diffVector.normalized;
                            finalDist = Dist;
                            farRangeCapturedEnemy = currentTarget;
                            //Debug.Log("enemy captured!");
                        }
                    
                    }
                }
                if (nearestStunnedEnemy != null)
                {
                    capturedTarget = nearestStunnedEnemy;
                    direction = directionStunned;
                    Debug.Log("Capture STUNNED Enemy!");
                }
                else if (!hit && EnemyCaptured)
                {
                    capturedTarget = farRangeCapturedEnemy;
                    direction = directionFar;
                    Debug.Log("Capture FAR Range Enemy!");
                }

            }

            if (EnemyCaptured) {
                stateEffect.CharacterControl.FaceTarget = direction;
                //Debug.DrawRay (stateEffect.CharacterControl.gameObject.transform.position, direction * 10f, Color.red);
                stateEffect.CharacterControl.TurnToTarget (0f, 0f);
                //if (!FacePrevTargetWhenNeutral)
                if (FacePrevTargetWhenNeutral || AutoFaceStunnedEnemy)
                    //stateEffect.CharacterControl.CharacterData.FormerAttackTarget = capturedTarget;
                    stateEffect.CharacterControl.SetFormerTarget (capturedTarget, LockOnTargetDuration);
                return true;
                //IsFaceForward = false;
            } else
                return false;

        }
    }
}