using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public enum MessageType {
        HealthChange,
        EnergyChange,
        EnergyGet,
        EnergyNotEnough,
        EnergyTake

    }

    [System.Serializable]
    public class CharacterData {
        public float HP;
        public float MaxHP;

        public float Armour;
        public float MaxArmour;
        public float ArmourRegenerationInStun;
        public float ArmourRegeneration;
        public float ArmourRegenerationDelay;

        public float Energy;
        public float MaxEnergy;
        public float EnergyRegeneration;
        public float EnergyDecayOverCharge;
        public float EnergyRegenerationDelay;
        public float EnergyGetOnExecuteHit;
        //public float EnergyGetOnEnemyDeath;
        public float EnergyGetOnEnemyDeathByExecute;

        public float GuardKnockbackReduction;
        public float GuardDamageReduction;
        public float GuardStunReduction;

        public float TargetGroupWeight;
        public float TargetGroupRadius;

        public float DodgeCoolDown;
        public float DamageMultiplier = 1.0f;
        public bool IsRagdollOn;
        public bool IsColliderOff;
        public bool IsAnimationPause;
        public bool IsGrappled;
        public bool IsDead;
        public bool IsInvincible;
        public bool IsSuperArmour;
        public bool IsRunning;
        public bool IsStunned;
        public bool IsGuarding;
        public bool OffEnergyRegen;
        public bool OffArmourRegen;
        public CharacterControl GrapplingTarget;
        public CharacterControl FormerAttackTarget;
        public TeamTag Team;

        public delegate void StateChangeEvent ();
        public event StateChangeEvent OnHealthChange;
        public event StateChangeEvent OnEnergyChange;
        public event StateChangeEvent OnEnergyTake;
        public event StateChangeEvent OnEnergyNotEnough;
        public event StateChangeEvent OnEnergyGet;

        public delegate void EnemyGetDamagedEvent (SkillEffect skill, CharacterControl enemy);
        public event EnemyGetDamagedEvent OnDamage;

        public const int STATE_BUFFER_SIZE = 3;
        public int[] StateBuffer = new int[STATE_BUFFER_SIZE];
        private int curIndex = 0;
        public int EffectShadow;
        public float GetHitTime;
        public int FirstFramesOfBlock;

        public List<PoolObject> VFXs;

        public void UpdateData () {
            if (!IsStunned && !OffArmourRegen && Armour < MaxArmour)
                Armour += ArmourRegeneration * Time.deltaTime;
            if (Armour > MaxArmour)
                Armour = MaxArmour;

            if (Energy > MaxEnergy) {
                Energy -= EnergyDecayOverCharge * Time.deltaTime;
                if (Energy < MaxEnergy)
                    Energy = MaxEnergy;
                SendMessage (MessageType.EnergyChange);
            } else if (Energy < MaxEnergy && !OffEnergyRegen) {
                Energy += EnergyRegeneration * Time.deltaTime;
                if (Energy > MaxEnergy)
                    Energy = MaxEnergy;
                SendMessage (MessageType.EnergyChange);

            }

        }

        public int GetPrevState () {
            return StateBuffer[(curIndex - 1 + STATE_BUFFER_SIZE) % STATE_BUFFER_SIZE];
        }

        public int GetCurrState () {
            return StateBuffer[curIndex];
        }
        public void LoadState (int name) {
            curIndex = (curIndex + 1) % STATE_BUFFER_SIZE;
            StateBuffer[curIndex] = name;

        }

        public void TakeDamage (float damage) {
            this.HP -= (damage * DamageMultiplier);
            SendMessage (MessageType.HealthChange);
        }

        public void TakeEnergy (float energy) {
            this.Energy -= energy;
            SendMessage (MessageType.EnergyChange);
            SendMessage (MessageType.EnergyTake);
        }

        public void GetEnergy (float energy) {
            this.Energy += energy;
            SendMessage (MessageType.EnergyChange);
            SendMessage (MessageType.EnergyGet);
        }
        public void GetEnergyToMaxOneUnit (float energy) {
            if (this.Energy + energy > this.MaxEnergy)
                this.Energy = Mathf.Max (this.Energy, this.MaxEnergy);
            else
                this.Energy += energy;

            SendMessage (MessageType.EnergyChange);
            SendMessage (MessageType.EnergyGet);
        }

        public void CurrentEnergyUnitChargeToFull () {
            int count;
            if (Energy / MaxEnergy > 1.0f) {
                count = (int) (Energy / MaxEnergy);
            } else {
                count = 0;
            }
            if ((Energy - count * MaxEnergy) > 0f || Energy == 0f) {
                Energy = (float) (count + 1) * MaxEnergy;
                SendMessage (MessageType.EnergyChange);
                SendMessage (MessageType.EnergyGet);

            }
        }

        public void SendMessage (MessageType type) {
            switch (type) {
                case MessageType.HealthChange:
                    if (OnHealthChange != null)
                        OnHealthChange ();
                    break;

                case MessageType.EnergyChange:
                    if (OnEnergyChange != null)
                        OnEnergyChange ();
                    break;

                case MessageType.EnergyNotEnough:
                    if (OnEnergyNotEnough != null)
                        OnEnergyNotEnough ();
                    break;

                case MessageType.EnergyGet:
                    if (OnEnergyGet != null)
                        OnEnergyGet ();
                    break;

                case MessageType.EnergyTake:
                    if (OnEnergyTake != null)
                        OnEnergyTake ();
                    break;

            }

        }

        public void SendGetDamageEvent (SkillEffect skill, CharacterControl enemy) {
            if (OnDamage != null)
                OnDamage (skill, enemy);

        }

    }
}