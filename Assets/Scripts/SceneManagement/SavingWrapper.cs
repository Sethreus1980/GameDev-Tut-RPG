using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;

namespace RPG.SceneManagement
{ 
public class SavingWrapper : MonoBehaviour
{
        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = 0.2f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }
        private IEnumerator LoadLastScene()
        {            
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }



        // Update is called once per frame
        void Update()
    {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }
        public void Load()
        {
            //call to the saving system load
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
        public void Save()
        {
            //call to the saving system load
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
