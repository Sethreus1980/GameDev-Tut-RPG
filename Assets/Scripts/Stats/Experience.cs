using UnityEngine;
using GameDevTV.Saving;
using System;

namespace RPG.Stats
{

    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        // public delegate void ExperienceGainedDelegate();
        public event Action onExperienceGained;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetPoints()
        {
            return experiencePoints;
        }

        object ISaveable.CaptureState()
        {
            return experiencePoints;
        }

        void ISaveable.RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}