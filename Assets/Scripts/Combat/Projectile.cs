using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Movement;
using UnityEngine.Events;

namespace RPG.Combat
{

    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        [SerializeField] bool isSeaking = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        GameObject instigator = null;
        float damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());            
        }
        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            if (isSeaking && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());                
            }            
                transform.Translate(Vector3.forward * speed * Time.deltaTime);            
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifeTime);
        }
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }
        //any Collider above only Capsule
        //private Collider targetCollider;

        //private Vector3 GetAimLocation()
        //{
        //    if (!targetCollider)
        //    {
        //        targetCollider = target.GetComponent<Collider>();
        //    }

        //    if (!targetCollider)
        //    {
        //        return target.transform.position;
        //    }
        //    return targetCollider.bounds.center;
        //}

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            target.TakeDamage(instigator, damage);
            onHit.Invoke();
            if (this.GetComponent<Collider>() && this.GetComponentInChildren<Renderer>() && this.GetComponentInChildren<TrailRenderer>())
            {
                GetComponentInChildren<Renderer>().enabled = false; //deaktivieren + folgezeile Trail deaktivieren damit pfeil nicht mehr sichtbar
                GetComponentInChildren<TrailRenderer>().enabled = false;
                GetComponent<Collider>().enabled = false; //damit kein doppelter schaden entsteht
            }
            else if (GetComponentInChildren<ParticleSystem>())
            {
                GetComponentInChildren<ParticleSystem>().Clear();
                GetComponent<Collider>().enabled = false;
            }

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
            
            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy, 1f);   //Zeitkomponente damit der Sound noch abgespielt wird bevor es zerstört wird
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}