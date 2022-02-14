/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using InfinityCode.uContext;
using InfinityCode.uContext.Inspector;
using InfinityCode.uContext.Windows;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.uContextPro.Inspector
{
    [InitializeOnLoad]
    public static class NestedEditor
    {
        private static GUIContent content;

        static NestedEditor()
        {
            ObjectFieldDrawer.OnGUIAfter += OnGUIAfter;
        }

        private static void DrawNestedEditor(Rect area, SerializedProperty property)
        {
            Object obj = property.objectReferenceValue;
            if (!ValidateTarget(obj)) return;

            area.xMin += EditorGUI.indentLevel * 15 - 16;
            area.width = 16;

            Color color = GUI.color;

            if (area.Contains(Event.current.mousePosition))
            {
                GUI.color = Color.gray;
            }

            if (content == null)
            {
                content = new GUIContent(EditorIconContents.editIcon);
            }

            StaticStringBuilder.Clear();
            StaticStringBuilder.Append("Open ");
            StaticStringBuilder.Append(obj.name);

            if (obj is Component)
            {
                StaticStringBuilder.Append(" (");
                StaticStringBuilder.Append(ObjectNames.NicifyVariableName(obj.GetType().Name));
                StaticStringBuilder.Append(")");
            }

            StaticStringBuilder.Append(" in window");

            content.tooltip = StaticStringBuilder.GetString(true);

            if (GUI.Button(area, content, GUIStyle.none))
            {
                if (obj is Component) ComponentWindow.Show(obj as Component, false);
                else ObjectWindow.Show(new []{obj}, false);
            }

            GUI.color = color;
        }

        private static void OnGUIAfter(Rect area, SerializedProperty property, GUIContent label)
        {
            if (!Prefs.nestedEditors) return;

            DrawNestedEditor(area, property);
        }

        private static bool ValidateTarget(Object target)
        {
            return target != null;
        }
    }
}