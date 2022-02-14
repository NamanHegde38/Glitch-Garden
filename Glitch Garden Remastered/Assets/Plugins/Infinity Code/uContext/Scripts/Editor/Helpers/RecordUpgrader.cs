/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using InfinityCode.uContext.JSON;
using InfinityCode.uContext.UnityTypes;
using InfinityCode.uContext.Windows;
using UnityEditor;

namespace InfinityCode.uContext
{
    [InitializeOnLoad]
    public static class RecordUpgrader
    {
        private const int CurrentUpgradeID = 4;
        private const string BookmarkItemSeparator = "|";

        static RecordUpgrader()
        {
            int upgradeID = LocalSettings.upgradeID;
            if (upgradeID < 1)
            {
                UpgradeFavoriteWindows();
                UpgradeSceneHistory();
                InitDefaultQuickAccessItems();
            }

            if (upgradeID < 2)
            {
                UpgradeEditQuickAccessItem();
            }

            if (upgradeID < 3)
            {
                TryRemoveOldDoc();
            }

            if (upgradeID < 4)
            {
                RemoveMissedBookmarks();
            }

            LocalSettings.upgradeID = CurrentUpgradeID;
        }

        public static void InitDefaultQuickAccessItems()
        {
            List<QuickAccessItem> items = ReferenceManager.quickAccessItems;
            if (items.Count > 0) return;

            QuickAccessItem save = new QuickAccessItem(QuickAccessItemType.menuItem)
            {
                settings = new[] { "File/Save" },
                icon = QuickAccessItemIcon.texture,
                iconSettings = Resources.iconsFolder + "Save.png",
                tooltip = "Save",
                expanded = false
            };

            QuickAccessItem hierarchy = new QuickAccessItem(QuickAccessItemType.window)
            {
                settings = new[] { SceneHierarchyWindowRef.type.AssemblyQualifiedName },
                icon = QuickAccessItemIcon.texture,
                iconSettings = Resources.iconsFolder + "Hierarchy2.png",
                tooltip = "Hierarchy",
                visibleRules = SceneViewVisibleRules.onMaximized,
                expanded = false
            };

            QuickAccessItem project = new QuickAccessItem(QuickAccessItemType.window)
            {
                settings = new[] { ProjectBrowserRef.type.AssemblyQualifiedName },
                icon = QuickAccessItemIcon.texture,
                iconSettings = Resources.iconsFolder + "Project.png",
                tooltip = "Project",
                visibleRules = SceneViewVisibleRules.onMaximized,
                expanded = false
            };

            QuickAccessItem inspector = new QuickAccessItem(QuickAccessItemType.window)
            {
                settings = new[] { InspectorWindowRef.type.AssemblyQualifiedName },
                icon = QuickAccessItemIcon.texture,
                iconSettings = Resources.iconsFolder + "Inspector.png",
                tooltip = "Inspector",
                visibleRules = SceneViewVisibleRules.onMaximized,
                expanded = false
            };

            QuickAccessItem bookmarks = new QuickAccessItem(QuickAccessItemType.window)
            {
                settings = new[] { "InfinityCode.uContext.Windows.Bookmarks, uContext-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" },
                icon = QuickAccessItemIcon.texture,
                iconSettings = Resources.iconsFolder + "Star-White.png",
                tooltip = "Bookmarks",
                expanded = false
            };

            QuickAccessItem viewGallery = new QuickAccessItem(QuickAccessItemType.window)
            {
                settings = new[] { "InfinityCode.uContext.Windows.ViewGallery, uContext-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" },
                icon = QuickAccessItemIcon.editorIconContent,
                iconSettings = "d_ViewToolOrbit",
                tooltip = "View Gallery",
                expanded = false
            };

            QuickAccessItem distanceTool = new QuickAccessItem(QuickAccessItemType.window)
            {
                settings = new[] { "InfinityCode.uContext.Windows.DistanceTool, uContext-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" },
                icon = QuickAccessItemIcon.texture,
                iconSettings = Resources.iconsFolder + "Rule.png",
                tooltip = "Distance Tool",
                expanded = false
            };

            QuickAccessItem quickAccessSettings = new QuickAccessItem(QuickAccessItemType.settings)
            {
                settings = new[] { "Project/uContext/Scene View/Quick Access Bar" },
                icon = QuickAccessItemIcon.editorIconContent,
                iconSettings = "d_Settings",
                tooltip = "Edit Items",
                expanded = false
            };

            QuickAccessItem info = new QuickAccessItem(QuickAccessItemType.window)
            {
                settings = new[] { "InfinityCode.uContext.Windows.Welcome, uContext-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" },
                icon = QuickAccessItemIcon.editorIconContent,
                iconSettings = "_Help",
                tooltip = "Info",
                expanded = false
            };

            items.Add(save);
            items.Add(hierarchy);
            items.Add(project);
            items.Add(inspector);
            items.Add(bookmarks);
            items.Add(viewGallery);
            items.Add(distanceTool);
            items.Add(new QuickAccessItem(QuickAccessItemType.flexibleSpace));
            items.Add(quickAccessSettings);
            items.Add(info);
        }

        private static void RemoveMissedBookmarks()
        {
            ReferenceManager.bookmarks.RemoveAll(b => b.target == null);
            ReferenceManager.Save(); 
        }

        private static void TryRemoveOldDoc()
        {
            string filename = Resources.assetFolder + "Documentation/Documentation.pdf";
            if (!File.Exists(filename)) return;

            try
            {
                File.Delete(filename);
            }
            catch
            {
                return;
            }

            filename += ".meta";
            if (!File.Exists(filename)) return;

            try
            {
                File.Delete(filename);
            }
            catch
            {

            }
        }

        private static string Unescape(string s)
        {
            return s.Replace("%2C", BookmarkItemSeparator).Replace("%25", "%");
        }

        private static void UpgradeEditQuickAccessItem()
        {
            foreach (QuickAccessItem item in ReferenceManager.quickAccessItems)
            {
                if (item.type != QuickAccessItemType.window) continue;
                if (item.settings[0] != "InfinityCode.uContext.QuickAccessEditor, uContext-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") continue;

                item.type = QuickAccessItemType.settings;
                item.settings[0] = "Project/uContext/Quick Access Bar";
                item.typeName = null;
            }

            ReferenceManager.Save();
        }

        private static void UpgradeFavoriteWindows()
        {
            const string Filename = "uContextFavoriteWindows.json";

            if (!File.Exists(Filename)) return;
            string text = File.ReadAllText(Filename, Encoding.UTF8);
            JsonArray items = JsonArray.ParseArray(text);
            List<FavoriteWindowItem> records = ReferenceManager.favoriteWindows;
            records.Clear();
            foreach (JsonItem item in items) records.Add(new FavoriteWindowItem(item));
            ReferenceManager.Save();

            File.Delete(Filename);
        }

        private static void UpgradeSceneHistory()
        {
            const string FILENAME = "SceneHistory.json";

            if (!File.Exists(FILENAME)) return;

            ReferenceManager.sceneHistory.Clear();

            string json = File.ReadAllText(FILENAME, Encoding.UTF8);
            ReferenceManager.sceneHistory = Json.Deserialize<List<SceneHistoryItem>>(json);
            ReferenceManager.Save();

            File.Delete(FILENAME);
        }
    }
}