/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace InfinityCode.uContext.Tools
{
    [EditorTool("Pivot Tool")]
    public class PivotTool : EditorTool
    {
        private PRS[] oldValues;

        private static GUIContent activeContent;
        private static GUIContent passiveContent;

        public override GUIContent toolbarIcon
        {
            get
            {
#if UNITY_2020_2_OR_NEWER
                if (ToolManager.IsActiveTool(this))
#else
                if (EditorTools.IsActiveTool(this))
#endif
                {
                    if (activeContent == null) activeContent = new GUIContent(Icons.pivotToolActive, "Pivot Tool");
                    return activeContent;
                }

                if (passiveContent == null) passiveContent = new GUIContent(Icons.pivotTool, "Pivot Tool");
                return passiveContent;
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (Selection.gameObjects.Length == 0) return;

            Vector3 position = UnityEditor.Tools.handlePosition;
            Quaternion rotation = Selection.activeGameObject.transform.rotation;

            if (Event.current.modifiers == EventModifiers.Alt)
            {
                Color clr = Handles.color;

                Handles.color = Color.blue;
                Handles.DrawLine(position - rotation * Vector3.forward * 1000, position + rotation * Vector3.forward * 1000);

                Handles.color = Color.red;
                Handles.DrawLine(position - rotation * Vector3.left * 1000, position + rotation * Vector3.left * 1000);

                Handles.color = Color.green;
                Handles.DrawLine(position - rotation * Vector3.up * 1000, position + rotation * Vector3.up * 1000);

                Handles.color = clr;
            }

            EditorGUI.BeginChangeCheck();
            position = Handles.PositionHandle(position, rotation);
            rotation = Handles.RotationHandle(rotation, position);
            if (!EditorGUI.EndChangeCheck()) return;

            int childCount = Selection.gameObjects.Max(t => t.transform.childCount);
            if (oldValues == null)
            {
                oldValues = new PRS[Mathf.Max(8, Mathf.NextPowerOfTwo(childCount))];
                for (int i = 0; i < oldValues.Length; i++)
                {
                    oldValues[i] = new PRS();
                }
            }
            else if (oldValues.Length <= childCount)
            {
                int oldCount = oldValues.Length;
                oldValues = new PRS[Mathf.NextPowerOfTwo(childCount)];
                for (int i = oldCount; i < oldValues.Length; i++)
                {
                    oldValues[i] = new PRS();
                }
            }

            Undo.SetCurrentGroupName("Change Pivot");
            int group = Undo.GetCurrentGroup();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GameObject go = Selection.gameObjects[i];
                Transform t = go.transform;

                for (int j = 0; j < t.childCount; j++)
                {
                    oldValues[j].Save(t.GetChild(j));
                }

                Undo.RecordObject(t, "Change Pivot");

                t.position = position;
                t.rotation = rotation;

                for (int j = 0; j < t.childCount; j++)
                {
                    Transform child = t.GetChild(j);
                    Undo.RecordObject(child, "Change Pivot");
                    oldValues[j].Restore(child);
                }
            }

            Undo.CollapseUndoOperations(group);
        }

        private class PRS
        {
            public Vector3 position;
            public Quaternion rotation;

            public void Save(Transform transform)
            {
                position = transform.position;
                rotation = transform.rotation;
            }

            public void Restore(Transform transform)
            {
                transform.position = position;
                transform.rotation = rotation;
            }
        }
    }
}