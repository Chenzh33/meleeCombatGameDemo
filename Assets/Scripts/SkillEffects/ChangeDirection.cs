using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    [CreateAssetMenu (fileName = "New State", menuName = "SkillEffects/ChangeDirection")]
    public class ChangeDirection : SkillEffect {
        public bool AllowEarlyTurn;
        public bool AutoFaceEnemy;
        //public bool ContinuallyFacing;

        //[Range (0f, 1f)]
        //public float AllowEarlyTurnTiming = 0f;
        [Range (0f, 3f)]
        public float AutoFaceEnemyTiming = 0f;
        [Range (0f, 3f)]
        public float AutoFaceEnemyDuration = 0f;

        public float CaptureDistanceFar = 5f;
        public float CaptureAngleRangeFar = 20f;
        public float CaptureDistanceNear = 2f;
        public float CaptureAngleRangeNear = 60f;
        public float smoothEarlyTurn = 20f;

        public override void OnEnter (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            bool IsFaceForward = true;
            Vector3 inputDirection = Vector3.zero;
            if (AllowEarlyTurn) {
                Vector2 inputDirection2d = stateEffect.CharacterControl.inputVector;
                if (inputDirection2d.magnitude > 0.01f) {
                    inputDirection = new Vector3 (inputDirection2d.x, 0, inputDirection2d.y);
                    stateEffect.CharacterControl.FaceTarget = inputDirection;
                    IsFaceForward = false;
                }
            }

            if (AutoFaceEnemy)
            {
                Vector3 initFaceDirection = new Vector3();
                if (!IsFaceForward)
                    initFaceDirection = inputDirection;
                else
                    initFaceDirection = animator.transform.forward;

                FaceEnemy(initFaceDirection, stateEffect, animator, stateInfo);
                IsFaceForward = false;
            }

            if (IsFaceForward)
                stateEffect.CharacterControl.FaceTarget = animator.transform.forward;

            stateEffect.CharacterControl.TurnToTarget (stateInfo.length * AutoFaceEnemyTiming, smoothEarlyTurn);

        }
        public override void UpdateEffect (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo) {
            if(AutoFaceEnemyDuration > 0f && stateInfo.normalizedTime < AutoFaceEnemyTiming + AutoFaceEnemyDuration)
            {
                Vector3 initFaceDirection = animator.transform.forward;
                FaceEnemy(initFaceDirection, stateEffect, animator, stateInfo);
            }

        }
        public override void OnExit (StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo) {

        }

        public void FaceEnemy(Vector3 initFaceDirection, StatewithEffect stateEffect, Animator animator, AnimatorStateInfo stateInfo)
        {
            Vector3 direction = Vector3.zero;
            bool EnemyCaptured = false;
            List<GameObject> enemyObjs = new List<GameObject>();
            if (stateEffect.CharacterControl.isPlayerControl)
            {
                CharacterControl[] characterControls = FindObjectsOfType(typeof(CharacterControl)) as CharacterControl[];
                foreach (CharacterControl c in characterControls)
                {
                    if (!c.isPlayerControl && !c.CharacterData.IsDead)
                        enemyObjs.Add(c.gameObject);
                }
            }
            else
            {
                // AI controlled
                enemyObjs.Add(stateEffect.CharacterControl.AIProgress.enemyTarget.gameObject);
            }

            // need update
            // get enemy list
            foreach (GameObject enemy in enemyObjs)
            {
                Vector3 diffVectorRaw = enemy.transform.position - stateEffect.CharacterControl.gameObject.transform.position;
                Vector3 diffVector = new Vector3(diffVectorRaw.x, 0, diffVectorRaw.z);
                Vector2 diffVector2d = new Vector2(diffVector.x, diffVector.z);
                Quaternion rotEnemy = Quaternion.LookRotation(diffVector, Vector3.up);
                Quaternion rotSelf = Quaternion.LookRotation(initFaceDirection, Vector3.up);
                /*
                if (!IsFaceForward)
                    rotSelf = Quaternion.LookRotation(inputDirection, Vector3.up);
                else
                    rotSelf = Quaternion.LookRotation(animator.transform.forward, Vector3.up);
                    */
                //Debug.Log ("dist = " + diffVector2d.magnitude.ToString ());
                //Debug.Log ("rot = " + Quaternion.Angle (rotEnemy, rotSelf).ToString ());

                float finalDist = Mathf.Infinity;
                float Dist = diffVector2d.magnitude;
                float AbsAngle = Mathf.Abs(Quaternion.Angle(rotEnemy, rotSelf));
                if (Dist <= CaptureDistanceFar && Dist > CaptureDistanceNear)
                {
                    if (AbsAngle <= CaptureAngleRangeFar && Dist < finalDist)
                    {
                        EnemyCaptured = true;
                        finalDist = Dist;
                        direction = diffVector.normalized;
                    }
                }
                else if (Dist <= CaptureDistanceNear)
                {
                    if (AbsAngle <= CaptureAngleRangeNear && Dist < finalDist)
                    {
                        EnemyCaptured = true;
                        finalDist = Dist;
                        direction = diffVector.normalized;
                    }
                }
            }
            if (EnemyCaptured)
            {
                stateEffect.CharacterControl.FaceTarget = direction;
                Debug.DrawRay(stateEffect.CharacterControl.gameObject.transform.position, direction * 10f, Color.red);
                stateEffect.CharacterControl.TurnToTarget(0f, 0f);
                //IsFaceForward = false;
            }
        }
    }
}