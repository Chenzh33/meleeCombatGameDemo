using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum TransitionParameter {
        Move,
        AttackMelee,
        AttackExecute,
        Dodge,
        Charge,
        TransitionIndexer,
        ForcedTransitionDodge,
        ForcedTransitionExecute,
        ForcedTransitionAttackHold,
        ForcedTransitionAttackHoldFS,
        AttackMeleeLink,
        AtkReleaseTiming,
        ExcReleaseTiming,
        DdgReleaseTiming,
        ButtonSmashing,
        AtkButtonHold,
        ExcButtonHold,
        DdgButtonHold,
        GrapplingHit,
        SpeedMultiplier,
        Stunned,
        MoveHold,
        ForbidDodge,
        IdleState,
        EnergyTaken,
        Fire,
        LockOnEnemy,
        GetHitOnGuard,
        Guard,
        GetHitOnGuardPrecisely,
        EnemyCollision,
        GuardHoldOn,
        ForbidGuard,

    }

    public enum PoolObjectType {
        AttackInfo,
        ProjectileChargedAttack,
        ButtonSmasher,
        Grappler,
        PathFindingAgent,
        CameraShaker,
        VFXSlam,
        VFXAttackHold,
        ProjectileBulletRifle,
        Shadow,
        VFXChargedSlam,
        ProjectileChargedAttackHold,
        ProjectileChargedGuard,
        ProjectileBulletPistol
    }
    public enum TeamTag {
        Player,
        Bandit,

    }

    public enum InputKeyType {
        KEY_MELEE_ATTACK,
        KEY_EXECUTE_ATTACK,
        KEY_DODGE,
        KEY_CHARGE,
        KEY_GUARD
    }
    public enum InputKeyStateType {
        KEY_MELEE_ATTACK_DOWN,
        KEY_MELEE_ATTACK_UP,
        KEY_MELEE_ATTACK,
        KEY_EXECUTE_ATTACK_DOWN,
        KEY_EXECUTE_ATTACK_UP,
        KEY_EXECUTE_ATTACK,
        KEY_DODGE_DOWN,
        KEY_DODGE_UP,
        KEY_DODGE,
        KEY_CHARGE_DOWN,
        KEY_CHARGE_UP,
        KEY_CHARGE,
        KEY_GUARD_DOWN,
        KEY_GUARD_UP,
        KEY_GUARD
    }
    public enum AttackType {
        Null,
        MustCollide,
        AOE,
        Projectile

    }
    public enum ProjectileType {
        ChargedAttack,
        RifleBullet,
        ChargedAttackHold,
        ChargedGuard,
        PistolBullet,

    }
    public enum BarImageType {
        Bound,
        Fill,
        Red
    }
    public enum VFXType {
        Null,
        Trail,
        Hold,
        Slam,
        AttackHoldAOE,
        ChargedSlam

    }

    [System.Serializable]
    public class TimeInterval {
        public int index;

        [Range (0f, 10f)]
        public float st;

        [Range (0f, 10f)]
        public float ed;
    }

    public enum GrapplerTargetChosingMode {
        EveryoneInRange,
        LockedTargetOnly,
        LockedStunnedTargetOnly

    }
    public enum GrapplerType {
        FrontStab,
        DownStab

    }

    [System.Serializable]
    public class SpawnUnitInfo {
        public SpawnUnitInfo () {
            Name = "";
            //Count = 0;
            PosVector = Vector3.zero;
            RotVector = Vector3.zero;

        }
        public SpawnUnitInfo (string name, Vector3 pos, Vector3 rot) {
            Name = name;
            PosVector = pos;
            RotVector = rot;

        }
        public string Name;
        //public int Count;
        public Vector3 PosVector;
        public Vector3 RotVector;
    }
    /*
        static float AreaUnderCurve (AnimationCurve curve, float w, float h) {
            float areaUnderCurve = 0f;
            var keys = curve.keys;

            for (int i = 0; i < keys.Length - 1; i++) {
                // Calculate the 4 cubic Bezier control points from Unity AnimationCurve (a hermite cubic spline) 
                Keyframe K1 = keys[i];
                Keyframe K2 = keys[i + 1];
                Vector2 A = new Vector2 (K1.time * w, K1.value * h);
                Vector2 D = new Vector2 (K2.time * w, K2.value * h);
                float e = (D.x - A.x) / 3.0f;
                float f = h / w;
                Vector2 B = A + new Vector2 (e, e * f * K1.outTangent);
                Vector2 C = D + new Vector2 (-e, -e * f * K2.inTangent);

                float a, b, c, d;
                a = -A.y + 3.0f * B.y - 3.0f * C.y + D.y;
                b = 3.0f * A.y - 6.0f * B.y + 3.0f * C.y;
                c = -3.0f * A.y + 3.0f * B.y;
                d = A.y;

                float t = (K2.time - K1.time) * w;

                float area = ((a / 4.0f) + (b / 3.0f) + (c / 2.0f) + d) * t;

                areaUnderCurve += area;
            }
            return areaUnderCurve;
        }
        */

}