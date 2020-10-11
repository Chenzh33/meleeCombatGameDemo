using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class ReflectionDetector : MonoBehaviour {
        CharacterControl control;
/*
        void FixedUpdate () {
            if (AttackManager.Instance.CurrentAttackInfo.Count > 0)
                CheckProjectile ();

        }

        private void CheckProjectile () {
            foreach (AttackInfo info in AttackManager.Instance.CurrentAttackInfo) {
                if (!info.IsRegistered || info.IsFinished)
                    continue;
                if (info.Targets.Contains (control))
                    continue;
                if (info.Attacker == control || info.Attacker.CharacterData.Team == control.CharacterData.Team)
                    continue;
                if (info.CurrentTargetNum >= info.MaxTargetNum) {
                    info.IsFinished = true;
                    continue;
                }

                if (info.Type == AttackType.MustCollide) {
                    if (IsCollidedWithAttackParts (info)) {
                        ProcessDamage (info);
                    }

                } else if (info.Type == AttackType.AOE) {
                    if (IsInRange (info)) {
                        ProcessDamage (info);

                    }
                } else if (info.Type == AttackType.Projectile) {
                    if (IsInRange (info)) {
                        ProcessDamage (info);
                    }

                }
            }
            if (extraAttackInfo.Count > 0) {

                foreach (AttackInfo info in extraAttackInfo) {
                    AttackManager.Instance.CurrentAttackInfo.Add (info);
                }
                extraAttackInfo.Clear ();
            }

        }
        private void ReflectProjectile (AttackInfo info) {
            info.ProjectileObject.Dead ();
            GameObject obj = PoolManager.Instance.GetObject (PoolObjectType.AttackInfo);
            AttackInfo reflectionInfo = obj.GetComponent<AttackInfo> ();
            reflectionInfo.Init (null, info.ProjectileSkill, control);
            obj.SetActive (true);
            extraAttackInfo.Add (reflectionInfo);
            //AttackManager.Instance.CurrentAttackInfo.Add(reflectionInfo);
            reflectionInfo.Register ();
            Vector3 dir = info.Attacker.gameObject.transform.position - control.gameObject.transform.position;
            dir.y = 0f;
            reflectionInfo.ProjectileSkill.Launch (reflectionInfo, control, control.GetReflectProjSpawnPoint ().position, dir.normalized);
        }

*/
    }
}