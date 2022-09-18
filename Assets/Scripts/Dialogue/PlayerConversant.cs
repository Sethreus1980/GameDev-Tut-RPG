using RPG.Core;
using RPG.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{

    public class PlayerConversant : MonoBehaviour, IAction
    {
        Dialogue currentDialogue;        
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;
        bool isChoosing = false;

        public event Action onConversationUpdated;
        AIConversant targetConversant = null;
        [SerializeField] string playerName;

        //brians way with IAction
        Dialogue targetDialogue = null;

        public void StartDialogueAction(AIConversant newConversant, Dialogue newDialogue)
        {
            if (newConversant == currentConversant) return;
            Quit();  //clear any old conversant 
            GetComponent<ActionScheduler>().StartAction(this);
            targetConversant = newConversant;
            targetDialogue = newDialogue;
        }


        public void Cancel()
        {
            Quit();
        }

        void Update()
        {
            if (!targetConversant) return;
            if (Vector3.Distance(transform.position, targetConversant.transform.position) > 3.0f)
            {
                GetComponent<Mover>().MoveTo(targetConversant.transform.position, 1.0f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                StartDialogue(targetConversant, targetDialogue);
                targetConversant = null;
            }
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetName();
            }
        }

        //StartDialogueAction und Update sind Test Bewegung zum Dialogue
        //public void StartDialogueAction(Dialogue dialogue, AIConversant speaker)
        //{
        //    Debug.Log($"StartDialogueAction {speaker}/{dialogue}");
        //    if (currentConversant != null && currentConversant == speaker) return;
        //    if (currentDialogue != null) Quit();
        //    if (dialogue == null)
        //    {
        //        return;
        //    }

        //    GetComponent<ActionScheduler>().StartAction(this);
        //    targetConversant = speaker;
        //    currentDialogue = dialogue;
        //}

        //private void Update()
        //{
        //    if (targetConversant)

        //    {
        //        if (Vector3.Distance(targetConversant.transform.position, transform.position) > 3)
        //        {
        //            GetComponent<Mover>().MoveTo(targetConversant.transform.position, 1);
        //        }
        //        else
        //        {
        //            GetComponent<Mover>().Cancel();
        //            StartDialogue();
        //        }
        //    }
        //}
        //private void StartDialogue()
        //{
        //    currentConversant = targetConversant;
        //    targetConversant = null;
        //    currentNode = currentDialogue.GetRootNode();
        //    TriggerEnterAction();
        //    onConversationUpdated();
        //}
        //entsprechend Lektion==>
        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }
        public void Quit()
        {            
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            onConversationUpdated();

        }
        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }
        public string GetText()
        {
            if(currentNode == null)
            {
                return "";
            }
            return currentNode.GetText();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }
        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next(); //advance to nextNode instead of showing chosenNode
        }

        public void Next()
        {
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, children.Count());
            TriggerExitAction();
            currentNode = children[randomIndex];
            TriggerEnterAction();
            onConversationUpdated();
        }

        public bool HasNext()
        {            
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {                
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }
        private void TriggerExitAction()
        {
            if (currentNode != null)
            {                
                TriggerAction(currentNode.GetOnExitAction());
            }
        }
               

        private void TriggerAction(string action)
        {
            if (action == "") return;            

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}