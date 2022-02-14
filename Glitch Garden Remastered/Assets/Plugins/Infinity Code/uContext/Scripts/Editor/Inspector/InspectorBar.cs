/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using InfinityCode.uContext.Actions;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace InfinityCode.uContext.Inspector
{
    [InitializeOnLoad]
    public static class InspectorBar
    {
        public static Vector2 lastPosition = Vector2.zero;
        public static Action<EditorWindow, Editor[]> OnDrawBefore;
        public static Action<EditorWindow, Editor[]> OnDrawAfter;
        public static Func<Component, Component[]> OnInitRelatedComponents;
        public static Action<EditorWindow, List<Component>> OnDrawRelatedComponents;

        private const string ELEMENT_NAME = "InspectorBar";
        private const int LINE_HEIGHT = 20;
        private const int LAST_LINE_OFFSET = 50;

        private static GUIStyle _selectedContentStyle;
        private static Dictionary<int, ContentWrapper> contentCache;
        private static Dictionary<EditorWindow, VisualElementWrapper> visualElements;
        private static double initTime;
        private static List<EditorWindow> failedWindows;
        //private static List<Component> relatedComponents;

        private static GUIStyle selectedContentStyle
        {
            get
            {
                if (_selectedContentStyle == null || _selectedContentStyle.normal.background == null)
                {
                    GUIStyle s = EditorStyles.toolbarButton;
                    _selectedContentStyle = new GUIStyle
                    {
                        normal =
                        {
                            background = Resources.CreateSinglePixelTexture(1, 0.1f),
                            textColor = s.normal.textColor
                        },
                        margin = s.margin,
                        padding = s.padding,
                        fixedHeight = s.fixedHeight,
                        alignment = s.alignment
                    };
                }

                return _selectedContentStyle;
            }
        }

        static InspectorBar()
        {
            EditorApplication.delayCall += InitInspector;
            WindowManager.OnWindowFocused += OnWindowFocused;
            WindowManager.OnMaximizedChanged += OnMaximizedChanged;
            Selection.selectionChanged += OnSelectionChanged;
            contentCache = new Dictionary<int, ContentWrapper>();
            //relatedComponents = new List<Component>();
            visualElements = new Dictionary<EditorWindow, VisualElementWrapper>();
        }

        private static void DrawBar(EditorWindow wnd, VisualElement editorsList)
        {
            if (editorsList == null) return;
            if (editorsList.childCount < 1) return;
            VisualElement elements = editorsList[0];

            Editor[] editors = EditorElementRef.GetEditors(elements);
            if (editors == null || editors.Length < 2) return;

            lastPosition = Vector2.zero;
            //relatedComponents.Clear();
            if (OnDrawBefore != null) OnDrawBefore(wnd, editors);

            int editorIndex = 0;

            for (int i = 0; i < editorsList.childCount; i++)
            {
                DrawIcon(wnd, editorsList, i, ref editorIndex, editors);
            }

            //if (OnDrawRelatedComponents != null && relatedComponents.Count > 0) OnDrawRelatedComponents(wnd, relatedComponents);

            GUIContent addContentContent = TempContent.Get("+", "Add Component");

            GUIStyle style = EditorStyles.toolbarButton;
            Rect rect = GetRect(style.CalcSize(addContentContent).x, wnd.position.width);

            if (GUI.Button(rect, addContentContent, style))
            {
                Vector2 s = Prefs.defaultWindowSize;
                Rect wp = wnd.position;
                Vector2 p = GUIUtility.GUIToScreenPoint(rect.position);
                p.x = wp.x + (wp.width - s.x) / 2;
                p.y += 45;
                rect = new Rect(p, s);

                AddComponent.ShowAddComponent(rect);
            }

            if (OnDrawAfter != null) OnDrawAfter(wnd, editors);

            GUIContent helpContent = TempContent.Get("?");
            float helpContentWidth = style.CalcSize(helpContent).x;
            rect = new Rect(wnd.position.width - helpContentWidth, lastPosition.y, helpContentWidth, LINE_HEIGHT);

            if (GUI.Button(rect, helpContent, style)) Links.OpenDocumentation("inspector-bar");

            int rowCount = Mathf.RoundToInt(lastPosition.y / LINE_HEIGHT + 1);
            VisualElementWrapper visualElement = visualElements[wnd];
            if (rowCount != visualElement.rowCount)
            {
                visualElement.style.height = rowCount * LINE_HEIGHT;
                visualElement.rowCount = rowCount;
            }
        }

        private static void DrawIcon(EditorWindow wnd, VisualElement editorsList, int elementIndex, ref int editorIndex, Editor[] editors)
        {
            VisualElement el = editorsList[elementIndex];
            if (el.GetType().Name != "EditorElement") return;
            if (el.childCount < 2)
            {
                editorIndex++;
                return;
            }

            Editor editor = editors[editorIndex];
            if (editor == null || editor.target == null) return;
            if (!Prefs.inspectorBarShowMaterials && editor.target is Material) return;

            int id = editor.target.GetInstanceID();
            ContentWrapper wrapper;

            GUIStyle normalStyle = EditorStyles.toolbarButton;

            if (!contentCache.TryGetValue(id, out wrapper)) wrapper = InitContent(editor, normalStyle);
            //if (wrapper.relatedComponents != null) relatedComponents.AddRange(wrapper.relatedComponents);

            StyleEnum<DisplayStyle> display = el.style.display;
            bool isActive = display.keyword == StyleKeyword.Null || display == DisplayStyle.Flex;
            GUIStyle style = isActive ? normalStyle : selectedContentStyle;

            float maxWidth = wnd.position.width;
            Rect rect = GetRect(wrapper.width, maxWidth - (elementIndex == editorsList.childCount - 1 ? LAST_LINE_OFFSET : 0));

            ButtonEvent buttonEvent = GUILayoutUtils.Button(rect, wrapper.content, style);
            ProcessIconEvents(wnd, editorsList, elementIndex, editorIndex, buttonEvent, isActive, editor);

            editorIndex++;
        }

        public static Rect GetRect(float width, float maxWidth)
        {
            Rect rect = new Rect(lastPosition, new Vector2(width, LINE_HEIGHT));
            if (rect.xMax >= maxWidth)
            {
                rect.x = 0;
                rect.y += LINE_HEIGHT;
                lastPosition.x = width;
                lastPosition.y = rect.y;
            }
            else lastPosition.x += width;

            return rect;
        }

        private static VisualElement GetMainContainer(EditorWindow wnd)
        {
            return wnd != null ? GetVisualElement(wnd.rootVisualElement, "unity-inspector-main-container") : null;
        }

        private static VisualElement GetVisualElement(VisualElement element, string className)
        {
            for (int i = 0; i < element.childCount; i++)
            {
                VisualElement el = element[i];
                if (el.ClassListContains(className)) return el;
                el = GetVisualElement(el, className);
                if (el != null) return el;
            }

            return null;
        }

        private static ContentWrapper InitContent(Editor editor, GUIStyle normalStyle)
        {
            Texture thumbnail = AssetPreview.GetMiniThumbnail(editor.target);
            string tooltip = ObjectNames.NicifyVariableName(editor.target.GetType().Name);

            ContentWrapper wrapper = new ContentWrapper
            {
                content = new GUIContent(thumbnail, tooltip),
            };

            bool useIcon = true;
            if (thumbnail.name == "cs Script Icon" || thumbnail.name == "d_cs Script Icon")
            {
                GameObjectUtils.GetPsIconContent(wrapper.content);
                useIcon = false;
            }

            Vector2 s = new Vector2();

            if (!useIcon)
            {
                s = normalStyle.CalcSize(wrapper.content);
                if (s.x < 25) s.x = 25;
            }
            else s.x = 25;

            wrapper.width = s.x;
            contentCache.Add(editor.target.GetInstanceID(), wrapper);
            return wrapper;
        }

        private static void InitInspector()
        {
            Object[] windows = UnityEngine.Resources.FindObjectsOfTypeAll(InspectorWindowRef.type);
            failedWindows = new List<EditorWindow>();
            foreach (EditorWindow wnd in windows)
            {
                if (wnd == null) continue;
                if (!InjectBar(wnd)) failedWindows.Add(wnd);
            }

            if (failedWindows.Count > 0)
            {
                initTime = EditorApplication.timeSinceStartup;
                EditorApplication.update += TryReinit;
            }
        }

        private static bool InjectBar(EditorWindow wnd)
        {
            if (!Prefs.inspectorBar) return false;

            VisualElement mainContainer = GetMainContainer(wnd);
            if (mainContainer == null) return false;
            if (mainContainer.childCount < 2) return false;

            if (mainContainer[0].name == ELEMENT_NAME) mainContainer.RemoveAt(0);

            VisualElement editorsList = GetVisualElement(mainContainer, "unity-inspector-editors-list");
            if (editorsList.childCount < 2) return false;
            VisualElement elements = editorsList[0];

            Editor[] editors = EditorElementRef.GetEditors(elements);
            if (editors == null || editors.Length < 2) return false;
            Object target = editors[0].target;

            if (!(target is GameObject) && target.GetType() != PrefabImporterRef.type) return false;

            VisualElement visualElement = new IMGUIContainer(() => DrawBar(wnd, editorsList));
            visualElement.name = ELEMENT_NAME;
            visualElement.style.height = LINE_HEIGHT;
            visualElement.style.position = Position.Relative;
            mainContainer.Insert(0, visualElement);

            visualElements[wnd] = new VisualElementWrapper(visualElement, 1);

            return true;
        }

        private static bool IsTransform(VisualElement el)
        {
            return el.name == "Transform" || el.name == "Rect Transform";
        }

        private static void OnMaximizedChanged(EditorWindow w)
        {
            Object[] windows = UnityEngine.Resources.FindObjectsOfTypeAll(InspectorWindowRef.type);
            foreach (EditorWindow wnd in windows)
            {
                if (wnd == null) continue;

                InjectBar(wnd);
            }
        }

        private static void OnSelectionChanged()
        {
            contentCache.Clear();
            InitInspector();
        }

        private static void OnWindowFocused(EditorWindow wnd)
        {
            if (wnd == null) return;
            if (wnd.GetType() != InspectorWindowRef.type) return;
            InjectBar(wnd);
        }

        private static void ProcessIconEvents(EditorWindow wnd, VisualElement editorsList, int elementIndex, int editorIndex, ButtonEvent buttonEvent, bool isActive, Editor editor)
        {
            Event e = Event.current;
            if (buttonEvent == ButtonEvent.hover)
            {
                wnd.Focus();
            }
            else if (buttonEvent == ButtonEvent.click)
            {
                if (e.button == 0)
                {
                    if (e.command || e.control || e.shift) ToggleVisible(wnd, editorsList, elementIndex, editorIndex, !isActive);
                    else
                    {
                        if (!isActive) SetSoloVisible(wnd, editorsList, elementIndex, editorIndex, false);
                        else
                        {
                            int countActive = 0;
                            for (int j = 0; j < editorsList.childCount; j++)
                            {
                                VisualElement el2 = editorsList[j];
                                if (el2.childCount < 2) continue;
                                StyleEnum<DisplayStyle> display = el2.style.display;
                                if (display.keyword == StyleKeyword.Null || display == DisplayStyle.Flex) countActive++;
                            }

                            SetSoloVisible(wnd, editorsList, elementIndex, editorIndex, countActive == 1);
                        }
                    }
                }
                else if (e.button == 1) ComponentUtils.ShowContextMenu(editor.target);

                e.Use();
            }
            else if (buttonEvent == ButtonEvent.drag)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = editor.targets;
                DragAndDrop.StartDrag("Drag " + editor.target.name);
                e.Use();
            }
        }

        private static void SetSoloVisible(EditorWindow wnd, VisualElement element, int index, int editorIndex, bool show)
        {
            if (show)
            {
                for (int i = 0; i < element.childCount; i++)
                {
                    VisualElement el = element[i];
                    el.style.display = DisplayStyle.Flex;
                    if (IsTransform(el)) el.style.marginTop = 0;
                }
            }
            else
            {
                for (int i = 0; i < element.childCount; i++)
                {
                    VisualElement el = element[i];
                    if (i == index)
                    {
                        el.style.display = DisplayStyle.Flex;
                        if (IsTransform(el)) el.style.marginTop = 7;
                        object inspectorElement = EditorElementRef.GetInspectorElement(el);
                        EditorElementRef.SetElementVisible(inspectorElement, false);

                    }
                    else el.style.display = DisplayStyle.None;
                }

                ActiveEditorTracker tracker = InspectorWindowRef.GetTracker(wnd);
                tracker.SetVisible(editorIndex, 1);
            }
        }

        private static void ToggleVisible(EditorWindow wnd, VisualElement element, int index, int editorIndex, bool show)
        {
            VisualElement el = element[index];
            if (show)
            {
                el.style.display = DisplayStyle.Flex;
                if (IsTransform(el)) el.style.marginTop = 0;
            }
            else
            {
                el.style.display = DisplayStyle.None;
                for (int i = 0; i < element.childCount; i++)
                {
                    el = element[i];
                    if (el.childCount < 2) continue;

                    if (IsTransform(el))
                    {
                        el.style.marginTop = 7;
                        break;
                    }
                    if (el.style.display == DisplayStyle.Flex) break;
                }
                ActiveEditorTracker tracker = InspectorWindowRef.GetTracker(wnd);
                tracker.SetVisible(editorIndex, 1);
            }
        }

        private static void TryReinit()
        {
            if (EditorApplication.timeSinceStartup - initTime <= 0.5) return;
            EditorApplication.update -= TryReinit;
            if (failedWindows != null)
            {
                TryReinit(failedWindows);
                failedWindows = null;
            }
        }

        private static void TryReinit(List<EditorWindow> windows)
        {
            foreach (EditorWindow wnd in windows)
            {
                if (wnd == null) continue;
                InjectBar(wnd);
            }
        }

        internal class ContentWrapper
        {
            public GUIContent content;
            //public Component[] relatedComponents;
            public float width;
        }

        internal class VisualElementWrapper
        {
            public VisualElement visualElement;
            public int rowCount;

            public IStyle style
            {
                get { return visualElement.style; }
            }

            public VisualElementWrapper(VisualElement visualElement, int rowCount)
            {
                this.visualElement = visualElement;
                this.rowCount = rowCount;
            }
        }
    }
}