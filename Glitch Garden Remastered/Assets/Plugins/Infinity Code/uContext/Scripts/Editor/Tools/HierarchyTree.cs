/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContext.Tools
{
    [InitializeOnLoad]
    public static class HierarchyTree
    {
        private static Texture2D _endIcon;
        private static Texture2D _lineIcon;
        private static Texture2D _middleIcon;

        public static Texture2D endIcon
        {
            get
            {
                if (_endIcon == null) _endIcon = Resources.LoadIcon("Hierarchy_Tree_End");

                return _endIcon;
            }
        }

        public static Texture2D lineIcon
        {
            get
            {
                if (_lineIcon == null) _lineIcon = Resources.LoadIcon("Hierarchy_Tree_Line");
                return _lineIcon;
            }
        }

        public static Texture2D middleIcon
        {
            get
            {
                if (_middleIcon == null) _middleIcon = Resources.LoadIcon("Hierarchy_Tree_Middle");
                return _middleIcon;
            }
        }

        static HierarchyTree()
        {
            HierarchyItemDrawer.Register("HierarchyTree", DrawTree);
        }

        private static void DrawTree(HierarchyItem item)
        {
            if (!Prefs.hierarchyTree || Event.current.type != EventType.Repaint) return;
            if (item == null || item.gameObject == null) return;

            Transform transform = item.gameObject.transform;
            Transform parent = transform.parent;
            if (parent == null) return;

            Rect rect = item.rect;

            rect.width = 36;
            rect.x -= 32;

            Vector4 borderWidths = new Vector4(transform.childCount > 0 ? 8 : 0, 0, 0, 0);

            Color color = Color.gray;

            SceneReferences r = SceneReferences.Get(item.gameObject.scene, false);
            if (r != null)
            {
                SceneReferences.HierarchyBackground background = r.GetBackground(parent.gameObject, true);
                if (background != null) color = background.color;
            }

            if (parent.childCount == 1 || transform.GetSiblingIndex() == parent.childCount - 1)
            {
                GUI.DrawTexture(rect, endIcon, ScaleMode.ScaleToFit, true, 0, color, borderWidths, Vector4.zero);
            }
            else
            {
                GUI.DrawTexture(rect, middleIcon, ScaleMode.ScaleToFit, true, 0, color, borderWidths, Vector4.zero);
            }

            while (parent != null && parent.parent != null)
            {
                rect.x -= 14;

                if (parent.GetSiblingIndex() < parent.parent.childCount - 1)
                {
                    GUI.DrawTexture(rect, lineIcon, ScaleMode.ScaleToFit, true, 0, color, borderWidths, Vector4.zero);
                }

                parent = parent.parent;
            }
        }
    }
}