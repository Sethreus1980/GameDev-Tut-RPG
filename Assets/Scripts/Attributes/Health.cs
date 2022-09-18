using UnityEngine;
using GameDevTV.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
            //stays empty UnityEvent erlaubt nicht den Zugriff auf dynamische Float-Werte daher Umweg über diese Subclass
        }

        LazyValue<float> healthPoints;

        bool isDead = false;

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        { 
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        
        public void Start()
        {
            //if (healthPoints < 0)
            //{
            //    healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
            //}
            healthPoints.ForceInit(); //Setzen erzwingen über LazyValue            
        }
        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }
        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return isDead;
        }       

        public void TakeDamage(GameObject instigator, float damage)
        {
            //print(gameObject.name + " took damage: " + damage);
            
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);            
            // print(healthPoints);
            // Trigger Death Anim
            if (healthPoints.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }
        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }
        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if (healthPoints.value == 0)
            {
                Die();
            }
        }
        private void RegenerateHealth()
        {
            //healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health); //setting to 100% Health
            //im Folgenden prozentual abhängig von gesetzter Prozentzahl "regenerationPercentage"
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage / 100;
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

    }
}
