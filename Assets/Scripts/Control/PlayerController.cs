using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;


        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }                

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
        //        [SerializeField] float maxNavPathLength = 40f;

        bool isDraggingUI =false;

        private void Awake()
        {
            health = GetComponent<Health>();
        }
        private void Update()
        {
            if (InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponents()) return;            
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
            //print("No Action possible here."); //mouseover Bereich ausserhalb der Map
        }

        private bool InteractWithComponents()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            //get all hits
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            //Sort by distance
            //build array of stances
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            //Sort the hits
            Array.Sort(distances, hits);
            //return
            return hits;
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }
            if (EventSystem.current.IsPointerOverGameObject()) //nur für UI GameObjects wichtig alle Canvas müssen Interactiv & Block Raycast deaktiviert haben zBsp Fader
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }
            if (isDraggingUI)
            {
                return true;
            }
            return false;
        }

       
        private bool InteractWithMovement()
        {            
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) { return false; }
                
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1.0f);                    
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            // Raycast to terrain
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;
            // Find nearest NavMeshPoint
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;
            
            // return true if found
            target = navMeshHit.position;
            //zu Lange Wege werden nicht akzeptiert (andere Seite eines Flusses erreichen zum Beispiel
            //NavMeshPath path = new NavMeshPath();
            //bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            //if (!hasPath) return false;
            //if (path.status != NavMeshPathStatus.PathComplete) return false;
            //if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        //private float GetPathLength(NavMeshPath path)
        //{
        //    float total = 0;
        //    if (path.corners.Length < 2) return total;
        //    for (int i = 0; i < path.corners.Length - 1; i++)
        //    {
        //        total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        //    }

        //    return total;
        //}

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}