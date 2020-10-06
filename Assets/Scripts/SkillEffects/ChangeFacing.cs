using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum AutoFaceEnemyType {
        Never,
        OnlyWhenNeutral,
        Always,
        Always_FirstCheckFarWhenNotNeutral
    }

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ChangeFacing")]
    public class ChangeFacing : SkillEffect {
        public bool AllowEarlyTurn;
        public AutoFaceEnemyType AutoFaceType;
        public bool FaceStunnedEnemyFirst;
        public bool FaceFormerTargetFirst;
        public bool ForceFaceFormerTarget;

        [Range (0f, 3f)]
        public float AutoFaceTiming = 0f;
        [Range (0f, 3f)]
        public float AutoFaceDuration = 0f;

        public float CaptureDistanceFar = 8f;
        public float CaptureAngleRangeFar = 45f;

        public float CaptureRadiusNear = 2.5f;
        public float CaptureCenterOffsetNear = 0.5f;
        public float smoothEarlyTurn = 20f;
        public float LockOnTargetDuration = 1.0f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            ChangeFaceDirection (stateEffect, animator, stateInfo, true);
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (AutoFaceDuration > 0f && stateInfo.normalizedTime < AutoFaceTiming + AutoFaceDuration) {
                ChangeFaceDirection (stateEffect, animator, stateInfo, false);
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) { }

        public void ChangeFaceDirection (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo, bool allowInput) {
            //bool IsFaceForward = true;
            //bool KeepFaceFormerTarget = false;
            bool InputNotNeutral = false;
            Vector3 inputDirection = Vector3.zero;
            Vector2 inputDirection2d = stateEffect.CharacterControl.inputVector;
            if (AllowEarlyTurn && allowInput && inputDirection2d.magnitude > 0.01f) {
                inputDirection = new Vector3 (inputDirection2d.x, 0, inputDirection2d.y);
                //stateEffect.CharacterControl.FaceTarget = inputDirection;
                //stateEffect.CharacterControl.SetFormerTarget(null, 0f);
                //IsFaceForward = false;
                InputNotNeutral = true;
            }

            switch (AutoFaceType) {
                case AutoFaceEnemyType.Never:
                    if (InputNotNeutral) {
                        stateEffect.CharacterControl.FaceTarget = inputDirection;
                        //stateEffect.CharacterControl.SetFormerTarget (null, 0f);

                    } else {
                        stateEffect.CharacterControl.FaceTarget = animator.transform.root.forward;
                        //stateEffect.CharacterControl.SetFormerTarget (null, 0f);
                    }

                    break;
                case AutoFaceEnemyType.OnlyWhenNeutral:
                    if (InputNotNeutral) {
                        stateEffect.CharacterControl.FaceTarget = inputDirection;
                        //stateEffect.CharacterControl.SetFormerTarget (null, 0f);
                    } else {
                        stateEffect.CharacterControl.FaceTarget = FaceEnemy (stateEffect, animator, stateInfo, animator.transform.root.forward, false);
                    }
                    break;
                case AutoFaceEnemyType.Always:
                    Vector3 initFaceDirection = animator.transform.root.forward;
                    if (InputNotNeutral)
                        initFaceDirection = inputDirection;
                    stateEffect.CharacterControl.FaceTarget = FaceEnemy (stateEffect, animator, stateInfo, initFaceDirection, false);
                    break;
                case AutoFaceEnemyType.Always_FirstCheckFarWhenNotNeutral:
                    Vector3 initDirection = animator.transform.root.forward;
                    if (InputNotNeutral) {
                        initDirection = inputDirection;
                        stateEffect.CharacterControl.FaceTarget = FaceEnemy (stateEffect, animator, stateInfo, initDirection, true);
                    } else {
                        stateEffect.CharacterControl.FaceTarget = FaceEnemy (stateEffect, animator, stateInfo, initDirection, false);
                    }
                    break;
            }

            stateEffect.CharacterControl.TurnToTarget (stateInfo.length * AutoFaceTiming, smoothEarlyTurn);
            //stateEffect.CharacterControl.TurnToTarget (0f, 0f);

        }

        public Vector3 FaceEnemy (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo, Vector3 initFaceDirection, bool firstCheckFar) {
            Vector3 FaceTarget = Vector3.zero;
            Vector3 formerTargetPos = Vector3.zero;
            Vector3 originPos = stateEffect.CharacterControl.gameObject.transform.position;
            bool faceFormerTarget = false;
            if (stateEffect.CharacterControl.CharacterData.FormerAttackTarget != null && !stateEffect.CharacterControl.CharacterData.FormerAttackTarget.CharacterData.IsDead) {
                formerTargetPos = stateEffect.CharacterControl.CharacterData.FormerAttackTarget.gameObject.transform.position;
                faceFormerTarget = (ForceFaceFormerTarget || (FaceFormerTargetFirst && (CheckTargetInRange (formerTargetPos, originPos, initFaceDirection, firstCheckFar) > 0f)));
            }
            if (faceFormerTarget) {
                // face former target
                Vector3 diffVectorRaw = formerTargetPos - originPos;
                Vector3 diffVector = new Vector3 (diffVectorRaw.x, 0f, diffVectorRaw.z);
                FaceTarget = diffVector.normalized;
                stateEffect.CharacterControl.SetFormerTarget (stateEffect.CharacterControl.CharacterData.FormerAttackTarget, LockOnTargetDuration);
            } else {
                // find new enemy
                CharacterControl enemy = CaptureEnemy (initFaceDirection, stateEffect, animator, stateInfo, firstCheckFar);
                if (enemy == null) {
                    FaceTarget = initFaceDirection;
                    //stateEffect.CharacterControl.SetFormerTarget (null, 0f);
                } else {
                    Vector3 diffVectorRaw = enemy.gameObject.transform.position - originPos;
                    Vector3 diffVector = new Vector3 (diffVectorRaw.x, 0f, diffVectorRaw.z);
                    FaceTarget = diffVector.normalized;
                    stateEffect.CharacterControl.SetFormerTarget (enemy, LockOnTargetDuration);
                }

            }
            return FaceTarget;
        }

        public float CheckTargetInRange (Vector3 target, Vector3 origin, Vector3 initFaceDirection, bool firstCheckFar) {
            Vector3 diffVectorRaw = target - origin;
            Vector3 diffVector = new Vector3 (diffVectorRaw.x, 0f, diffVectorRaw.z);
            float DistFar = diffVector.magnitude;

            Vector3 offsetCenter = origin + initFaceDirection * CaptureCenterOffsetNear;
            Vector3 diffVectorRawOffset = target - offsetCenter;
            Vector3 diffVectorOffset = new Vector3 (diffVectorRawOffset.x, 0f, diffVectorRawOffset.z);
            float DistNear = diffVectorOffset.magnitude;

            Quaternion rotEnemy = Quaternion.LookRotation (diffVector, Vector3.up);
            Quaternion rotSelf = Quaternion.LookRotation (initFaceDirection, Vector3.up);
            float AbsAngle = Mathf.Abs (Quaternion.Angle (rotEnemy, rotSelf));

            if (firstCheckFar) {
                if ((DistFar <= CaptureDistanceFar && AbsAngle <= CaptureAngleRangeFar))
                    return DistFar;
                else
                    return 0f;
            } else {
                if ((DistFar <= CaptureDistanceFar && AbsAngle <= CaptureAngleRangeFar) ||
                    (DistNear < CaptureRadiusNear))
                    return Mathf.Min (DistFar, DistNear);
                else
                    return 0f;
            }

        }

        public CharacterControl CaptureEnemy (Vector3 initFaceDirection, StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo, bool firstCheckFar) {
            Vector3 direction = Vector3.zero;
            bool EnemyCaptured = false;
            CharacterControl capturedTarget = null;

            //Debug.Log("test far range enemy capture");
            List<GameObject> enemyObjs = new List<GameObject> ();
            if (stateEffect.CharacterControl.isPlayerControl) {
                foreach (AIProgress ai in AIAgentManager.Instance.TotalAIAgent) {
                    enemyObjs.Add (ai.gameObject);
                }
            } else {
                // AI controlled
                if (stateEffect.CharacterControl.AIProgress.enemyTarget != null)
                    enemyObjs.Add (stateEffect.CharacterControl.AIProgress.enemyTarget.gameObject);
            }

            // need update
            // get enemy list

            float finalDist = Mathf.Infinity;
            float stunnedEnemyDist = Mathf.Infinity;
            CharacterControl nearestStunnedEnemy = null;
            CharacterControl CapturedEnemy = null;
            foreach (GameObject enemy in enemyObjs) {
                float Dist = CheckTargetInRange (enemy.transform.position, stateEffect.CharacterControl.gameObject.transform.position, initFaceDirection, firstCheckFar);
                if (Dist > 0f) {
                    CharacterControl currentTarget = enemy.GetComponent<CharacterControl> ();
                    if (FaceStunnedEnemyFirst && currentTarget.CharacterData.IsStunned && Dist < stunnedEnemyDist) {
                        EnemyCaptured = true;
                        //directionStunned = diffVector.normalized;
                        stunnedEnemyDist = Dist;
                        nearestStunnedEnemy = currentTarget;
                        //Debug.Log("enemy captured!");
                    }
                    if (Dist < finalDist) {
                        EnemyCaptured = true;
                        //directionFar = diffVector.normalized;
                        finalDist = Dist;
                        CapturedEnemy = currentTarget;
                        //Debug.Log("enemy captured!");
                    }

                }
            }
            if (nearestStunnedEnemy != null) {
                capturedTarget = nearestStunnedEnemy;
                //direction = directionStunned;
            } else if (EnemyCaptured) {
                capturedTarget = CapturedEnemy;
            }
            //direction = directionFar;

            return capturedTarget;
        }
    }
}