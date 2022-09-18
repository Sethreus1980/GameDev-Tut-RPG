using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;

        // LateUpdate is called once per frame but as last code
        void LateUpdate()
        {
            transform.position = target.position;
        }
    }

}