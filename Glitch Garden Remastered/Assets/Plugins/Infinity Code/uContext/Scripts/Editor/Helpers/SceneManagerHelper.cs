/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfinityCode.uContext
{
    [InitializeOnLoad]
    public static class SceneManagerHelper
    {
        static SceneManagerHelper()
        {
            EditorApplication.delayCall += () =>
            {
                EditorSceneManager.sceneOpened -= OnSceneOpened;
                EditorSceneManager.sceneOpened += OnSceneOpened;
                
                EditorSceneManager.sceneClosed -= OnSceneClosed;
                EditorSceneManager.sceneClosed += OnSceneClosed;
            };

            SceneReferences.OnCreate += OnCreateSceneReferences;
            UpdateInstances();
        }

        public static bool AskForSave(params Scene[] scenes)
        {
            if (scenes.Length == 0) return true;

            List<string> paths = new List<string>();

            for (int i = 0; i < scenes.Length; i++)
            {
                Scene scene = scenes[i];

                if (scene.isDirty) paths.Add(scene.path);
            }

            if (paths.Count > 0)
            {
                int result = EditorUtility.DisplayDialogComplex("Scene(s) Have Been Modified", "Do you want to save the changes you made in the scenes:\n" + String.Join("\n", paths) + "\n\nYour changes will be lost if you don't save them.", "Save", "Don't Save", "Cancel");
                if (result == 2) return false;

                if (result == 0)
                {
                    for (int i = 0; i < scenes.Length; i++)
                    {
                        Scene scene = scenes[i];
                        if (scene.isDirty) EditorSceneManager.SaveScene(scene);
                    }
                }
            }

            return true;
        }

        private static void OnCreateSceneReferences(SceneReferences r)
        {
            r.gameObject.hideFlags = Prefs.hideSceneReferences ? HideFlags.HideInHierarchy : HideFlags.None;
            Undo.RegisterCreatedObjectUndo(r.gameObject, "References");
        }

        private static void OnSceneClosed(Scene scene)
        {
            EditorApplication.delayCall += UpdateInstances;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            EditorApplication.delayCall += UpdateInstances;
        }

        public static void UpdateInstances()
        {
            HideFlags hideFlags = Prefs.hideSceneReferences? HideFlags.HideInHierarchy: HideFlags.None;
            
            SceneReferences.UpdateInstances();
            foreach (SceneReferences r in SceneReferences.instances)
            {
                GameObject go = r.gameObject;
                if (go.hideFlags == hideFlags) continue;

                go.hideFlags = hideFlags;
                EditorUtility.SetDirty(go);
            }
        }
    }
}