using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        GUIStyle playerNodeStyle;
        [NonSerialized]
        DialogueNode draggingNode = null;
        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode linkingParentNode = null;
        Vector2 scrollPosition;
        //Vector2 windowSize = new Vector2(4000, 4000); //for flexible scrollfield
        [NonSerialized]
        bool draggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;
        const float backgroundSize = 50;
        const float windowSize = 4000;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            //Debug.Log("ShowEditorWindow");
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                //Debug.Log("OpenDialogue");
                ShowEditorWindow();
                return true;
            }
            return false;
        }
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;            
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            
            Selection.selectionChanged += OnSelectionChange;
            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChange()
        {
            //Debug.Log("On Selection Changed");
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }

        }

        private void OnGUI()
        {      
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                //GUILayoutUtility.GetRect(4000, 4000); static scrollfield
                Rect canvas = GUILayoutUtility.GetRect(windowSize, windowSize); //for flexible scrollfield
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, windowSize / backgroundSize, windowSize / backgroundSize); //backgroundSize = 50 = pixel backgroundImage
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {                 
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);                    
                }

                EditorGUILayout.EndScrollView();

                if(creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }

            }
            
        }
        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);// selectedDialogue.GetRootNode();
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    //Record dragOffset and dragging Canvas Drag total elseif
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                //Repaint(); one option GUI.change is better
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                //Update scrollPosition Canvas Drag total elseif
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                ////following for flexible scrollfield
                //Vector2 maxSize = new Vector2();
                //foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                //{
                //    maxSize.x = node.rect.xMax > maxSize.x ? node.rect.xMax : maxSize.x;
                //    maxSize.y = node.rect.yMax > maxSize.y ? node.rect.yMax : maxSize.y;                    
                //}
                
                //windowSize = maxSize + new Vector2(200, 200);
               
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {                
                draggingCanvas = false;
            }
        }      

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            
            GUILayout.BeginArea(node.GetRect(), style);            
            node.SetText(EditorGUILayout.TextField(node.GetText()));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Dialogue"))
            {
                creatingNode = node;
            }
            if (GUILayout.Button("Delete"))
            {
                deletingNode = node;
            }
            GUILayout.EndHorizontal();
            DrawLinkButtons(node);

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if(linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {                    
                    linkingParentNode = null;
                }
            }
            else if(linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {                
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset,Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;                    
                }
            }
            return foundNode;
        }
    }
}
