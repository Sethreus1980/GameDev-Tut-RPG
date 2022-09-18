using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Combat;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{

    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue;
        [SerializeField] string conversantName;

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogue == null || !enabled)
            {
                return false;
            }

            if (callingController.GetComponent<Fighter>().InCombat()) return false;
            
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogueAction(this, dialogue);
            }
            return true;
        }

        public string GetName()
        {
            return conversantName;
        }
    }
}