/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.uContext;
using InfinityCode.uContext.Windows;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uContextPro.Windows
{
    [InitializeOnLoad]
    public static class DistanceToolPick
    {
        private static DistanceTool wnd;

        static DistanceToolPick()
        {
            DistanceTool.OnPickStarted += OnPickStarted;
        }

        private static void OnPickStarted(DistanceTool tool)
        {
            wnd = tool;
            if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.Focus();
            SceneViewManager.AddListener(OnSceneView);
        }

        private static void OnSceneView(SceneView sceneView)
        {
            Event e = Event.current;
            if (DistanceTool.pickTarget == null || wnd == null)
            {
                DistanceTool.pickTarget = null;
                SceneViewManager.RemoveListener(OnSceneView);
                return;
            }

            if (e.type == EventType.Repaint)
            {
                if (SceneViewManager.lastGameObjectUnderCursor != null)
                {
                    Vector3 p = SceneViewManager.lastWorldPosition;
                    float d = (DistanceTool.pickTarget.point - p).sqrMagnitude;
                    DistanceTool.pickTarget.point = p;
                    if (d > 0) wnd.Repaint();
                }
                else if (sceneView.in2DMode)
                {
                    Vector3 point = sceneView.camera.ScreenToWorldPoint(new Vector3(e.mousePosition.x, Screen.height - e.mousePosition.y - 40));
                    point.z = 0;
                    DistanceTool.pickTarget.point = point;
                    wnd.Repaint();
                }
            }
            else if (e.type == EventType.KeyDown && 
                (e.keyCode == KeyCode.Return || 
                 e.keyCode == KeyCode.KeypadEnter || 
                 e.keyCode == KeyCode.Escape))
            {
                StopPick();
                e.Use();
            }
            else if (e.type == EventType.MouseDown && e.button == 0)
            {
                StopPick();
                e.Use();
                SceneViewManager.BlockMouseUp();
            }
        }

        private static void StopPick()
        {
            wnd.Repaint();
            DistanceTool.pickTarget = null;
            wnd = null;
            SceneViewManager.RemoveListener(OnSceneView);
        }
    }
}