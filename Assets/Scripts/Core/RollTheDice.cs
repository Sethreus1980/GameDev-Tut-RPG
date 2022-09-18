using UnityEngine;

namespace RPG.Control
{
    public class RollTheDice : MonoBehaviour
    {
        public static int RollTheDwell(int min, int max)
        {
            int i = Random.Range(min, max);

            //print(i);
            return i;
        }
    }
}
