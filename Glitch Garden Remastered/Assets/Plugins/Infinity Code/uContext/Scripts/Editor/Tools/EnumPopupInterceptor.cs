/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using InfinityCode.uContext.UnityTypes;
using InfinityCode.uContext.Unsafe;
using InfinityCode.uContext.Windows;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    [InitializeOnLoad]
    public static class EnumPopupInterceptor
    {
        private static MemoryPatcher patcher;
        private static GUIContent s_MixedValueContent = new GUIContent("-", "Mixed Values");

        static EnumPopupInterceptor()
        {
            if (Prefs.searchInEnumFields) Patch();
        }

        private static void Patch()
        {
            try
            {
                MethodInfo method = typeof(EditorGUI).GetMethod(
                    "DoPopup",
                    Reflection.StaticLookup,
                    null,
                    new[]
                    {
                        typeof(Rect),
                        typeof(int),
                        typeof(int),
                        typeof(GUIContent[]),
                        typeof(Func<int, bool>),
                        typeof(GUIStyle)
                    },
                    null);
                MethodInfo replacement = typeof(EnumPopupInterceptor).GetMethod(
                    "DoPopup",
                    Reflection.StaticLookup,
                    null,
                    new[]
                    {
                        typeof(Rect),
                        typeof(int),
                        typeof(int),
                        typeof(GUIContent[]),
                        typeof(Func<int, bool>),
                        typeof(GUIStyle)
                    },
                    null);

                patcher = MemoryPatcher.SwapMethods(method, replacement);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal static int DoPopup(
            Rect position,
            int controlID,
            int selected,
            GUIContent[] popupValues,
            Func<int, bool> checkEnabled,
            GUIStyle style)
        {
            selected = PopupCallbackInfoRef.GetSelectedValueForControl(controlID, selected);
            GUIContent content = !EditorGUI.showMixedValue ? (selected >= 0 && selected < popupValues.Length ? popupValues[selected] : GUIContent.none) : s_MixedValueContent;
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && position.Contains(e.mousePosition))
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            position.y = (float)(-19.0 + position.y - selected * 16);
                        }

                        object instance = Activator.CreateInstance(PopupCallbackInfoRef.type, controlID);
                        PopupCallbackInfoRef.SetInstance(instance);
                        if (popupValues.Length < Prefs.searchInEnumFieldsMinValues) EditorUtility.DisplayCustomMenu(position, popupValues, checkEnabled, EditorGUI.showMixedValue ? -1 : selected, new EditorUtility.SelectMenuItemFunction(PopupCallbackInfoRef.GetSetEnumValueDelegate(instance)), null);
                        else
                        {
                            FlatSelectorWindow.Show(position, popupValues, EditorGUI.showMixedValue ? -1 : selected).OnSelect += i =>
                            {
                                PopupCallbackInfoRef.GetSetEnumValueDelegate(instance).Invoke(null, null, i);
                            };
                        }
                        GUIUtility.keyboardControl = controlID;
                        e.Use();
                    }
                    break;
                case EventType.KeyDown:
                    if (MainActionKeyForControl(e, controlID))
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            position.y = (float)(-19.0 + position.y - selected * 16);
                        }

                        object instance = Activator.CreateInstance(PopupCallbackInfoRef.type, controlID);
                        PopupCallbackInfoRef.SetInstance(instance);
                        if (popupValues.Length < Prefs.searchInEnumFieldsMinValues) EditorUtility.DisplayCustomMenu(position, popupValues, checkEnabled, EditorGUI.showMixedValue ? -1 : selected, new EditorUtility.SelectMenuItemFunction(PopupCallbackInfoRef.GetSetEnumValueDelegate(instance)), null);
                        else FlatSelectorWindow.Show(position, popupValues, EditorGUI.showMixedValue ? -1 : selected).OnSelect += i =>
                        {
                            PopupCallbackInfoRef.GetSetEnumValueDelegate(instance).Invoke(null, null, i);
                        };
                        e.Use();
                        break;
                    }
                    break;
                case EventType.Repaint:
                    style.Draw(position, content, controlID, false, position.Contains(e.mousePosition));
                    break;
            }
            return selected;
        }

        internal static bool MainActionKeyForControl(Event e, int controlId)
        {
            if (GUIUtility.keyboardControl != controlId) return false;
            bool flag = e.alt || e.shift || e.command || e.control;
            return e.type == EventType.KeyDown && (e.keyCode == KeyCode.Space || e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) && !flag;
        }

        public static void Refresh()
        {
            if (Prefs.searchInEnumFields) Patch();
            else patcher.Restore();
        }
    }
}