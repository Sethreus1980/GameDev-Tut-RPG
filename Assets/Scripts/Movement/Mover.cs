using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using GameDevTV.Saving;
using RPG.Attributes;
using System.Collections.Generic;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;

        NavMeshAgent navMeshAgent;
        Health health;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }
        //Debug.DrawRay(lastRay.origin, lastRay.direction * 100);

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            
            GetComponent<ActionScheduler>().StartAction(this);            
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }
               
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;            
        }

        //GetComponent<NavMeshAgent>().destination = target.position;

        public void Cancel()
        {            
            navMeshAgent.isStopped = true;
        }
        
        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity; //global velocity
            Vector3 localVelocity = transform.InverseTransformDirection(velocity); //umwandlung in local velocity
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);

        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            //return new SerializableVector3(transform.position);
            return data;
        }

        public void RestoreState(object state) // beim Laden wird RestoreState immer zwischen Awake und Start ausgeführt
        {
            //SerializableVector3 position = (SerializableVector3)state;
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            GetComponent<NavMeshAgent>().enabled = false; //NavMeshAgent deaktivieren damit keine Konflikte entstehen
            //transform.position = position.ToVector();
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }
    }
}
