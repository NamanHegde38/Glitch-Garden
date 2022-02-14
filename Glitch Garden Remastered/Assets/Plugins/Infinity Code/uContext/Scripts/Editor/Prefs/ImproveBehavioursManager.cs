/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext
{
    public static partial class Prefs
    {
        public static bool improveAddComponentBehaviour = true;
        public static bool improveDragAndDropBehaviour = true;
        public static bool improveMaximizeGameViewBehaviour = true;
        public static bool changeNumberFieldValueByArrow = true;
        public static bool searchInEnumFields = true;
        public static int searchInEnumFieldsMinValues = 6;

        private class ImproveBehavioursManager : PrefManager, IHasShortcutPref
        {
            public override IEnumerable<string> keywords
            {
                get
                {
                    return new[]
                    {
                        "Improve Behaviours",
                        "Add Component By Shortcut",
                        "Drag And Drop To Canvas",
                        "Maximize Game View By Shortcut (SHIFT + Space)",
                        "Number Fields"
                    };
                }
            }

            public override float order
            {
                get { return Order.improveBehaviors; }
            }

            public override void Draw()
            {
                EditorGUILayout.LabelField("Improve Behaviors");

                EditorGUI.indentLevel++;

                improveAddComponentBehaviour = EditorGUILayout.ToggleLeft("Add Component By Shortcut", improveAddComponentBehaviour);
                EditorGUI.BeginChangeCheck();
                changeNumberFieldValueByArrow = EditorGUILayout.ToggleLeft("Change Number Fields Value By Arrows", changeNumberFieldValueByArrow);
                if (EditorGUI.EndChangeCheck()) NumberFieldInterceptor.Refresh();
                improveDragAndDropBehaviour = EditorGUILayout.ToggleLeft("Drag And Drop To Canvas", improveDragAndDropBehaviour);
                improveMaximizeGameViewBehaviour = EditorGUILayout.ToggleLeft("Maximize Game View By Shortcut (SHIFT + Space)", improveMaximizeGameViewBehaviour);
                EditorGUI.BeginChangeCheck();
                searchInEnumFields = EditorGUILayout.ToggleLeft("Search In Enum Fields", searchInEnumFields);
                if (EditorGUI.EndChangeCheck()) EnumPopupInterceptor.Refresh();
                if (searchInEnumFields)
                {
                    EditorGUI.indentLevel++;
                    searchInEnumFieldsMinValues = EditorGUILayout.IntField("Min Values", searchInEnumFieldsMinValues);
                    EditorGUI.indentLevel--;
                }
                

                EditorGUI.indentLevel--;
            }

            public IEnumerable<Shortcut> GetShortcuts()
            {
                List<Shortcut> shortcuts = new List<Shortcut>();
                if (improveAddComponentBehaviour)
                {
                    shortcuts.Add(new Shortcut("Add Component To Selected GameObject", "Everywhere",
#if !UNITY_EDITOR_OSX
                    "CTRL + SHIFT + A"
#else
                        "CMD + SHIFT + A"
#endif
                    ));
                }

                if (improveMaximizeGameViewBehaviour)
                {
                    shortcuts.Add(new Shortcut("Maximize GameView", "Game View", "SHIFT + SPACE"));
                }

                if (changeNumberFieldValueByArrow)
                {
                    EventModifiers m1 = EventModifiers.Shift;
#if !UNITY_EDITOR_OSX
                    EventModifiers m2 = EventModifiers.Control;
#else
                    EventModifiers m2 = EventModifiers.Command;
#endif

                    shortcuts.Add(new Shortcut("Increase Value By 1", "Number Field", EventModifiers.None, KeyCode.UpArrow));
                    shortcuts.Add(new Shortcut("Increase Value By 10", "Number Field", m1, KeyCode.UpArrow));
                    shortcuts.Add(new Shortcut("Increase Value By 100", "Number Field", m2, KeyCode.UpArrow));
                    shortcuts.Add(new Shortcut("Decrease Value By 1", "Number Field", EventModifiers.None, KeyCode.DownArrow));
                    shortcuts.Add(new Shortcut("Decrease Value By 10", "Number Field", m1, KeyCode.DownArrow));
                    shortcuts.Add(new Shortcut("Decrease Value By 100", "Number Field", m2, KeyCode.DownArrow));
                }

                return shortcuts;
            }
        }
    }
}