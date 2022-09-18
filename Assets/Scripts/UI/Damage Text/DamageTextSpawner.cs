using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{

    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] GameObject damageTextPrefab = null;
        
        public void Spawn(float damageAmount)
        {
            GameObject instance = Instantiate<GameObject>(damageTextPrefab, transform);
            instance.GetComponentInChildren<DamageText>().SetValue(damageAmount);
        }
    }
}
