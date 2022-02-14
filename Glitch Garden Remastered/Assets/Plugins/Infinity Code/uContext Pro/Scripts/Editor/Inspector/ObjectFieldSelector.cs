/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Linq;
using System.Reflection;
using InfinityCode.uContext;
using InfinityCode.uContext.Inspector;
using InfinityCode.uContext.Windows;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Resources = UnityEngine.Resources;

namespace InfinityCode.uContextPro.Inspector
{
    [InitializeOnLoad]
    public static class ObjectFieldSelector
    {
        private static bool blockMouseUp = false;

        static ObjectFieldSelector()
        {
            ObjectFieldDrawer.OnGUIBefore += OnGUIBefore;
        }

        private static void OnGUIBefore(Rect area, SerializedProperty property, GUIContent label)
        {
            if (!Prefs.objectFieldSelector) return;

            Event e = Event.current;
            if (e.type == EventType.MouseUp && blockMouseUp)
            {
                blockMouseUp = false;
                e.Use();
                return;
            }

            if (e.type != EventType.MouseDown || e.button != 1) return;

            Rect rect = new Rect(area);
            rect.xMin = rect.xMax - 16;
            if (!rect.Contains(e.mousePosition)) return;

            Object[] targets = property.serializedObject.targetObjects;
            Object target = targets[0];
            Type type = target.GetType();
            FieldInfo field = Reflection.GetField(type, property.propertyPath, true);
            if (field == null) return;

            Object[] objects = null;
            GUIContent[] contents = null;

            if (field.FieldType.IsSubclassOf(typeof(Component)))
            {
#if UNITY_2020_3_OR_NEWER
                objects = Object.FindObjectsOfType(field.FieldType, true);
#else
                objects = Object.FindObjectsOfType(field.FieldType);
#endif
                if (objects.Length == 1)
                {
                    Undo.RecordObjects(targets, "Modified Property");
                    property.objectReferenceValue = objects[0];
                }
                else
                {
                    objects = objects.OrderBy(o => o.name).ToArray();
                    contents = new GUIContent[objects.Length];
                    
                    for (int i = 0; i < objects.Length; i++)
                    {
                        Component component = objects[i] as Component;
                        StaticStringBuilder.Clear();
                        StaticStringBuilder.Append(component.name)
                            .Append(" (")
                            .Append(component.GetType().Name)
                            .Append(")");

                        contents[i] = new GUIContent(StaticStringBuilder.GetString(true), GameObjectUtils.GetGameObjectPath(component.gameObject).ToString());
                    }
                }
            }
            else if (field.FieldType.IsSubclassOf(typeof(ScriptableObject)))
            {
                objects = Resources.FindObjectsOfTypeAll(field.FieldType);
                if (objects.Length == 1)
                {
                    Undo.RecordObjects(targets, "Modified Property");
                    property.objectReferenceValue = objects[0];
                }
                else
                {
                    objects = objects.OrderBy(o => o.name).ToArray();
                    contents = new GUIContent[objects.Length];
                    for (int i = 0; i < objects.Length; i++)
                    {
                        Object obj = objects[i];
                        ScriptableObject so = obj as ScriptableObject;
                        contents[i] = new GUIContent(so.name, AssetDatabase.GetAssetPath(so));
                    }
                }
            }
            else return;

            blockMouseUp = true;
            e.Use();

            if (contents == null || contents.Length == 0) return;

            area.xMin += EditorGUIUtility.labelWidth;

            FlatSelectorWindow.Show(area, contents, -1).OnSelect += index =>
            {
                Undo.SetCurrentGroupName("Modified Property");
                int group = Undo.GetCurrentGroup();
                for (int i = 0; i < targets.Length; i++)
                {
                    Undo.RecordObject(targets[i], "Modified Property");
                    field.SetValue(targets[i], objects[index]);
                }
                Undo.CollapseUndoOperations(group);
                GUI.changed = true;
            };
        }
    }
}