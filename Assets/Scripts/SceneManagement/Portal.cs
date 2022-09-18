using System;
using System.Collections;
using RPG.Control;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifer
        {
            A, B      //Hier geht es darum dass die Portale auf einander synchronisiert werden PortalScene1 = A PortalScene2=A
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifer destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            //print("");
            if (other.tag == "Player")
            {                   
                StartCoroutine(Transition());                
            }
        }
        private IEnumerator Transition()
        {
            if(sceneToLoad <0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;

            }
                        
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            PlayerController playerController =GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            yield return fader.FadeOut(fadeOutTime);

            //Save current Level            
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            //Load current Level
            wrapper.Load();

            //print("Scene Loaded");
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);
            newPlayerController.enabled = true;
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //Variante 1 Kollision Navmesh      player.GetComponent<NavMeshAgent>().enabled = false;
            //Variante 1 Kollision Navmesh      player.transform.position = otherPortal.spawnPoint.position;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position); //Variante 2 (besser)

            player.transform.rotation = otherPortal.spawnPoint.rotation; //Rotation hat keine Konflikte mit NavMesh
            //Variante 1 Kollision Navmesh      player.GetComponent<NavMeshAgent>().enabled = true;
            // Weitere Kollision bei mehreren Portalen in einer Scene, daher Player NavMesh deaktiviert als Default wird durch Health.cs beim ersten Update aktiviert
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return (portal);
            }
            return null;
        }
    }
}
