using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SFX
{
    public class DieSoundRandomizer : MonoBehaviour
    {
        [SerializeField] List<AudioClip> damageSounds;
        [Range(0.1f, 0.5f)]
        public float volumeChangeMultiplier = 0.2f;
        [Range(0.1f, 0.5f)]
        public float pitchChangeMultiplier = 0.2f;

        AudioSource audioSource;
        private void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        public void PlayRandomDeathSound()
        {
            int index = Random.Range(0, damageSounds.Count);
            //audioSource.volume = Random.Range(1 - volumeChangeMultiplier, 1);
            audioSource.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);
            audioSource.PlayOneShot(damageSounds[index]);
        }
    }
}