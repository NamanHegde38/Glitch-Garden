/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uContext;
using InfinityCode.uContext.Windows;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContextPro.Windows
{
    [InitializeOnLoad]
    public static class DistanceToolCursor
    {
        private static bool lastPointUnderCursor;

        static DistanceToolCursor()
        {
            DistanceTool.OnUseCursorGUI += OnUseCursor;
            DistanceTool.OnUseCursorSceneGUI += OnUseCursorSceneGUI;
        }

        private static void DrawLine(Vector3 prev, Vector3 p)
        {
            Handles.DrawLine(p, prev);
            Handles.Label((p + prev) / 2, DistanceTool.GetDistance(p, prev).ToString("F1"), DistanceTool.distanceStyle);
        }

        private static void OnUseCursor(DistanceTool tool, Vector3 prev, bool hasPrev, ref float distance)
        {
            lastPointUnderCursor = EditorGUILayout.ToggleLeft("Last point is the cursor in Scene View", lastPointUnderCursor);

            if (!lastPointUnderCursor) return;
            
            if (hasPrev)
            {
                if (SceneViewManager.lastGameObjectUnderCursor != null)
                {
                    distance += DistanceTool.GetDistance(SceneViewManager.lastWorldPosition, prev);
                }
            }

            DistanceTool.isDirty = true;

            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Escape)
                {
                    lastPointUnderCursor = false;
                    e.Use();
                }
            }
        }

        private static void OnUseCursorSceneGUI(DistanceTool tool, Vector3 prev, bool hasPrev, ref float distance)
        {
            if (!lastPointUnderCursor) return;

            Event e = Event.current;

            if (hasPrev)
            {
                if (SceneViewManager.lastGameObjectUnderCursor != null)
                {
                    Vector3 p = SceneViewManager.lastWorldPosition;
                    DrawLine(prev, p);
                }
                else if (SceneView.lastActiveSceneView.in2DMode)
                {
                    Vector3 p = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(e.mousePosition.x, Screen.height - e.mousePosition.y - 40));
                    p.z = 0;
                    DrawLine(prev, p);
                }
            }

            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    if (SceneView.lastActiveSceneView.in2DMode)
                    {
                        Vector3 point = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(e.mousePosition.x, Screen.height - e.mousePosition.y - 40));
                        point.z = 0;
                        tool.AddPoint(point);
                    }
                    else if (SceneViewManager.lastGameObjectUnderCursor != null)
                    {
                        tool.AddPoint(SceneViewManager.lastWorldPosition);
                    }
                    SceneViewManager.BlockMouseUp();
                    e.Use();
                }
                else if (e.button == 1)
                {
                    lastPointUnderCursor = false;
                    SceneViewManager.BlockMouseUp();
                    e.Use();
                }
            }
            else if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Escape)
                {
                    lastPointUnderCursor = false;
                    e.Use();
                }
            }
        }
    }
}