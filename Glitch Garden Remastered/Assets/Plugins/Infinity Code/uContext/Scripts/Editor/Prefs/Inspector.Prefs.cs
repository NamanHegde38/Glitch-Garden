/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using UnityEditor;

namespace InfinityCode.uContext
{
    public static partial class Prefs
    {
        public static bool inspectorBar = true;
        public static bool inspectorBarShowMaterials = false;
        //public static bool inspectorBarRelatedComponents = true;
        public static bool nestedEditors = true;
        public static bool dragObjectFields = true;
        public static bool objectFieldSelector = true;

        public class InspectorManager : StandalonePrefManager<InspectorManager>
        {
            public override IEnumerable<string> keywords
            {
                get
                {
                    return new[]
                    {
                        "Inspector Bar",
                        "Nested Editor",
                        "Drag Object Field"
                    };
                }
            }

            public override void Draw()
            {
                dragObjectFields = EditorGUILayout.ToggleLeft("Drag Object Fields", dragObjectFields);
                inspectorBar = EditorGUILayout.ToggleLeft("Inspector Bar", inspectorBar);
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!inspectorBar);

                //inspectorBarRelatedComponents = EditorGUILayout.ToggleLeft("Related Components", inspectorBarRelatedComponents);
                inspectorBarShowMaterials = EditorGUILayout.ToggleLeft("Show Materials", inspectorBarShowMaterials);

                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;

                nestedEditors = EditorGUILayout.ToggleLeft("Nested Editors", nestedEditors);
                objectFieldSelector = EditorGUILayout.ToggleLeft("Object Field Selector", objectFieldSelector);
            }
        }
    }
}