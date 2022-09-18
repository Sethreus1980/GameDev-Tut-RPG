using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SFX
{
    public class DamageSoundRandomizer : MonoBehaviour
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
        public void PlayRandomDamageSound()
        {                      
            int index = Random.Range(0, damageSounds.Count);
            //audioSource.volume = Random.Range(1 - volumeChangeMultiplier, 1);
            audioSource.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);
            audioSource.PlayOneShot(damageSounds[index]);
        }

        //[SerializeField] AudioClip[] damageSounds;
        //private AudioSource source;
        //[Range(0.1f, 0.5f)]
        //public float volumeChangeMultiplier = 0.2f;

        //[Range(0.1f, 0.5f)]
        //public float pitchChangeMultiplier = 0.2f;

        //private void PlayRandomDamageSound()
        //{
        //    source = GetComponent<AudioSource>();
        //    source.clip = damageSounds[Random.Range(0, damageSounds.Length)];
        //    source.volume = Random.Range(1 - volumeChangeMultiplier, 1);
        //    source.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);
        //    source.PlayOneShot(source.clip);
        //}
        //void Start()
        //{
        //    source = GetComponent<AudioSource>();
        //}
        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.O))
        //    {
        //        source.clip = sounds[Random.Range(0, sounds.Length)];
        //        source.volume = Random.Range(1 - volumeChangeMultiplier, 1);
        //        source.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);
        //        source.PlayOneShot(source.clip);

        //    }
        //}


    }
}
