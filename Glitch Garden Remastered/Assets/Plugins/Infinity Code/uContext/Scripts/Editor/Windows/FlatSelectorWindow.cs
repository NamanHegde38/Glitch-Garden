/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using InfinityCode.uContext.UnityTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfinityCode.uContext.Windows
{
    public class FlatSelectorWindow : EditorWindow
    {
        private const string SEARCHFIELD_NAME = "uContextFlatSelectorSearchTextField";
        private const int ITEM_HEIGHT = 16;
        private const int EXTRA_HEIGHT = 28;

        public Action<int> OnSelect;

        private GUIContent[] contents;
        private int selected;
        private string filterText;
        private bool resetSelection = true;
        private ListView listView;
        private static FlatSelectorWindow instance;
        private List<int> items;
        private bool ignoreItemSelect;

        static FlatSelectorWindow()
        {
            EventManager.AddBinding(EventManager.ClosePopupEvent).OnInvoke += b =>
            {
                if (instance != null) instance.Close(); 
            };
        }

        private void DrawFilterTextField()
        {
            GUI.SetNextControlName(SEARCHFIELD_NAME);
            EditorGUI.BeginChangeCheck();
            filterText = EditorGUILayoutEx.ToolbarSearchField(filterText);
            if (EditorGUI.EndChangeCheck())
            {
                items = new List<int>();
                if (string.IsNullOrEmpty(filterText))
                {
                    for (int i = 0; i < contents.Length; i++) items.Add(i);
                }
                else
                {
                    string pattern = SearchableItem.GetPattern(filterText);

                    for (int i = 0; i < contents.Length; i++)
                    {
                        if (SearchableItem.GetAccuracy(pattern, contents[i].text) > 0)
                        {
                            items.Add(i);
                        }
                    }
                }

                listView.itemsSource = items;
#if UNITY_2021_2_OR_NEWER
                listView.Rebuild();
#else
                listView.Refresh();
#endif

                ignoreItemSelect = true;
                listView.selectedIndex = items.IndexOf(selected);
                ignoreItemSelect = false;

                Rect rect = position;
                rect.height = Mathf.Min(Prefs.defaultWindowSize.y, items.Count * ITEM_HEIGHT + EXTRA_HEIGHT);
                position = rect;
            }

            if (resetSelection && Event.current.type == EventType.Repaint)
            {
                GUI.FocusControl(SEARCHFIELD_NAME);
                resetSelection = false;
            }
        }

        private void Init()
        {
            items = new List<int>();
            for (int i = 0; i < contents.Length; i++) items.Add(i);

            Func<VisualElement> makeItem = () =>
            {
                VisualElement el = new VisualElement();
                el.style.flexDirection = FlexDirection.Row;

                Image image = new Image();
                image.style.width = image.style.height = 12;
                image.style.marginTop = 1;
                image.style.marginRight = 5;

                el.Add(image);
                el.Add(new Label());
                return el;
            };

            Action<VisualElement, int> bindItem = (el, i) =>
            {
                Image image = el[0] as Image;
                image.image = selected == items[i] ? EditorGUIUtility.IconContent("FilterSelectedOnly").image : null;

                Label label = el[1] as Label;
                label.text = contents[items[i]].text;
                label.tooltip = contents[items[i]].tooltip;
            };

            listView = new ListView(items, ITEM_HEIGHT, makeItem, bindItem);
            listView.selectionType = SelectionType.Single;
#if UNITY_2020_1_OR_NEWER
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
#endif

#if UNITY_2020_1_OR_NEWER
            listView.onSelectionChange += objects =>
#else
            listView.onSelectionChanged += objects => 
#endif
            {
                if (ignoreItemSelect) return;
                if (OnSelect != null) OnSelect(items[listView.selectedIndex]);
                Close();
            };

            listView.style.flexGrow = 1.0f;

            rootVisualElement.style.paddingBottom = 2;
            rootVisualElement.style.paddingLeft = 2;
            rootVisualElement.style.paddingRight = 2;
            rootVisualElement.style.paddingTop = 2;
            rootVisualElement.style.borderBottomColor = Color.gray;
            rootVisualElement.style.borderBottomWidth = 1;
            rootVisualElement.style.borderLeftColor = Color.gray;
            rootVisualElement.style.borderLeftWidth = 1;
            rootVisualElement.style.borderRightColor = Color.gray;
            rootVisualElement.style.borderRightWidth = 1;
            rootVisualElement.style.borderTopColor = Color.gray;
            rootVisualElement.style.borderTopWidth = 1;

            rootVisualElement.Add(new IMGUIContainer(DrawFilterTextField));
            rootVisualElement.Add(listView);

            ignoreItemSelect = true;
            listView.selectedIndex = selected;
            ignoreItemSelect = false;
        }

        private void OnDestroy()
        {
            contents = null;
            listView = null;
            instance = null;
            OnSelect = null;
        }

        private void OnGUI()
        {
            if (focusedWindow != this) Close();

            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Escape) Close();
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    ignoreItemSelect = true;
                    if (listView.selectedIndex == items.Count - 1) listView.selectedIndex = 0;
                    else listView.selectedIndex++;
                    ignoreItemSelect = false;
                }
                else if (e.keyCode == KeyCode.UpArrow)
                {
                    ignoreItemSelect = true;
                    if (listView.selectedIndex == 0 || listView.selectedIndex == -1) listView.selectedIndex = items.Count - 1;
                    else listView.selectedIndex--;
                    ignoreItemSelect = false;
                }
                else if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Space || e.keyCode == KeyCode.KeypadEnter)
                {
                    if (listView.selectedIndex != -1 && OnSelect != null) OnSelect(items[listView.selectedIndex]);
                    Close();
                }
            }
        }

        public static FlatSelectorWindow Show(Rect rect, GUIContent[] contents, int selected)
        {
            if (instance != null) instance.Close();
            if (contents == null || contents.Length == 0) return null;

            float width = rect.width;

            GUIStyle style = EditorStyles.label;

            for (int i = 0; i < contents.Length; i++)
            {
                Vector2 size = style.CalcSize(contents[i]);
                if (size.x + 50 > width) width = size.x + 50;
            }

            FlatSelectorWindow wnd = instance = CreateInstance<FlatSelectorWindow>();
            rect.y += rect.height;
            rect.width = width;
            rect.height = Mathf.Min(Prefs.defaultWindowSize.y, contents.Length * ITEM_HEIGHT + EXTRA_HEIGHT);
            rect.position = GUIUtility.GUIToScreenPoint(rect.position);
            wnd.minSize = Vector2.one;
            wnd.position = rect;
            wnd.contents = contents;
            wnd.selected = selected;
            wnd.ShowPopup();
            wnd.Init();

            return wnd;
        }
    }
}