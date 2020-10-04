using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum AutoFaceEnemyType
    {
        Never,
        OnlyWhenNeutral,
        Always
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
            ChangeFaceDirection (stateEffect, animator, stateInfo);
        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if (AutoFaceDuration > 0f && stateInfo.normalizedTime < AutoFaceTiming + AutoFaceDuration) {
                ChangeFaceDirection (stateEffect, animator, stateInfo);
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {
        }

        public void ChangeFaceDirection (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            bool IsFaceForward = true;
            bool KeepFaceFormerTarget = false;
            bool InputNotNeutral = false;
            Vector3 inputDirection = Vector3.zero;
            Vector2 inputDirection2d = stateEffect.CharacterControl.inputVector;
            if (AllowEarlyTurn && inputDirection2d.magnitude > 0.01f) {
                inputDirection = new Vector3 (inputDirection2d.x, 0, inputDirection2d.y);
                //stateEffect.CharacterControl.FaceTarget = inputDirection;
                //stateEffect.CharacterControl.SetFormerTarget(null, 0f);
                IsFaceForward = false;
                InputNotNeutral = true;
            }

            switch (AutoFaceType)
            {
                case AutoFaceEnemyType.Never:
                    if (InputNotNeutral)
                    {
                        stateEffect.CharacterControl.FaceTarget = inputDirection;
                        stateEffect.CharacterControl.SetFormerTarget(null, 0f);

                    }

                    break;
                case AutoFaceEnemyType.OnlyWhenNeutral:
                    if (InputNotNeutral)
                    {
                        stateEffect.CharacterControl.FaceTarget = inputDirection;
                        stateEffect.CharacterControl.SetFormerTarget(null, 0f);
                    }
                    else
                    {
                        if(ForceFaceFormerTarget && stateEffect.CharacterControl.CharacterData.FormerAttackTarget != null && !stateEffect.CharacterControl.CharacterData.FormerAttackTarget.CharacterData.IsDead)
                        {
                            // force face former target
                            Vector3 diffVectorRaw = stateEffect.CharacterControl.CharacterData.FormerAttackTarget.gameObject.transform.position - stateEffect.CharacterControl.gameObject.transform.position;
                            Vector3 diffVector = new Vector3(diffVectorRaw.x, 0f, diffVectorRaw.z);
                            KeepFaceFormerTarget = true;
                            IsFaceForward = false;
                            stateEffect.CharacterControl.FaceTarget = diffVector.normalized;
                            stateEffect.CharacterControl.SetFormerTarget(stateEffect.CharacterControl.CharacterData.FormerAttackTarget, LockOnTargetDuration);
                            //stateEffect.CharacterControl.TurnToTarget(stateInfo.length * AutoFaceEnemyTiming, smoothEarlyTurn);
                        }
                        else
                        {
                            Vector3 initFaceDirection = animator.transform.root.forward;
                            CharacterControl enemy = CaptureEnemy(initFaceDirection, stateEffect, animator, stateInfo);

                        }

                    }
                    break;
                case AutoFaceEnemyType.Always:
                    break;
            }

          

            if (IsFaceForward) {
                stateEffect.CharacterControl.FaceTarget = animator.transform.root.forward;
                //stateEffect.CharacterControl.CharacterData.FormerAttackTarget = null;
            }

            stateEffect.CharacterControl.TurnToTarget (stateInfo.length * AutoFaceTiming, smoothEarlyTurn);
            //stateEffect.CharacterControl.TurnToTarget (0f, 0f);

        }

        public float CheckTargetInRange(Vector3 target, Vector3 origin, Vector3 initFaceDirection)
        {
            Vector3 diffVectorRaw = target - origin;
            Vector3 diffVector = new Vector3(diffVectorRaw.x, 0f, diffVectorRaw.z);
            float DistFar = diffVector.magnitude;

            Vector3 offsetCenter = origin + initFaceDirection * CaptureCenterOffsetNear;
            Vector3 diffVectorRawOffset = target - offsetCenter;
            Vector3 diffVectorOffset = new Vector3(diffVectorRawOffset.x, 0f, diffVectorRawOffset.z);
            float DistNear = diffVectorOffset.magnitude;

            Quaternion rotEnemy = Quaternion.LookRotation(diffVector, Vector3.up);
            Quaternion rotSelf = Quaternion.LookRotation(initFaceDirection, Vector3.up);
            float AbsAngle = Mathf.Abs(Quaternion.Angle(rotEnemy, rotSelf));

            if ((DistFar <= CaptureDistanceFar && AbsAngle <= CaptureAngleRangeFar) ||
                (DistNear < CaptureRadiusNear))
                return Mathf.Min(DistFar, DistNear);
            else
                return 0f;
        }



        public CharacterControl CaptureEnemy(Vector3 initFaceDirection, StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            Vector3 direction = Vector3.zero;
            bool EnemyCaptured = false;
            CharacterControl capturedTarget = null;

            //Debug.Log("test far range enemy capture");
            List<GameObject> enemyObjs = new List<GameObject>();
            if (stateEffect.CharacterControl.isPlayerControl)
            {
                foreach (AIProgress ai in AIAgentManager.Instance.TotalAIAgent)
                {
                    enemyObjs.Add(ai.gameObject);
                }
            }
            else
            {
                // AI controlled
                if (stateEffect.CharacterControl.AIProgress.enemyTarget != null)
                    enemyObjs.Add(stateEffect.CharacterControl.AIProgress.enemyTarget.gameObject);
            }

            // need update
            // get enemy list

            float finalDist = Mathf.Infinity;
            float stunnedEnemyDist = Mathf.Infinity;
            Vector3 directionFar = Vector3.zero;
            Vector3 directionStunned = Vector3.zero;
            CharacterControl nearestStunnedEnemy = null;
            CharacterControl farRangeCapturedEnemy = null;
            foreach (GameObject enemy in enemyObjs)
            {
                float Dist = CheckTargetInRange(enemy.transform.position, stateEffect.CharacterControl.gameObject.transform.position, initFaceDirection);
                if (Dist > 0f)
                {
                    CharacterControl currentTarget = enemy.GetComponent<CharacterControl> ();
                    if (FaceStunnedEnemyFirst && currentTarget.CharacterData.IsStunned && Dist < stunnedEnemyDist)
                    {
                        EnemyCaptured = true;
                        //directionStunned = diffVector.normalized;
                        stunnedEnemyDist = Dist;
                        nearestStunnedEnemy = currentTarget;
                        //Debug.Log("enemy captured!");
                    }
                    if (Dist < finalDist)
                    {
                        EnemyCaptured = true;
                        //directionFar = diffVector.normalized;
                        finalDist = Dist;
                        farRangeCapturedEnemy = currentTarget;
                        //Debug.Log("enemy captured!");
                    }

                }
            }
            if (nearestStunnedEnemy != null)
            {
                capturedTarget = nearestStunnedEnemy;
                //direction = directionStunned;
            }
            else if (EnemyCaptured)
            {
                capturedTarget = farRangeCapturedEnemy;
                //direction = directionFar;
            }

            return capturedTarget;
        }
    }
}