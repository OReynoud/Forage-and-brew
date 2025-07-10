using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace proTools.proFolder
{
    public static class FolderEffectSettings
    {
        private const string PrefKey_Effect = "proFolder_Effect";
        private const string PrefKey_Hotkey = "proFolder_Hotkey"; 

        public static bool EffectsEnabled
        {
            get => EditorPrefs.GetBool(PrefKey_Effect, true);
            set => EditorPrefs.SetBool(PrefKey_Effect, value);
        }

        public static bool HotkeyType
        {
            get => EditorPrefs.GetBool(PrefKey_Hotkey, true);
            set => EditorPrefs.SetBool(PrefKey_Hotkey, value);
        }
    }

    public class proFolderEditorWindow : EditorWindow
    {
        private static readonly string[] effectOptions = new[] { "Enable", "Disable" };
        private static readonly string[] hotkeyOptions = { "Ctrl + Alt + Click", "Shift + Alt + Click" };

        public static readonly List<Color> colorPresets = new List<Color>
        {
            new Color(0.91f, 0.27f, 0.27f),
            new Color(0.99f, 0.60f, 0.30f),
            new Color(0.98f, 0.88f, 0.37f),
            new Color(0.71f, 0.91f, 0.42f),
            new Color(0.49f, 0.84f, 0.53f),
            new Color(0.38f, 0.84f, 0.78f),
            new Color(0.38f, 0.70f, 0.99f),
            new Color(0.58f, 0.55f, 0.97f),
            new Color(0.76f, 0.45f, 0.91f),
            new Color(0.91f, 0.47f, 0.76f),
        };

        private Texture2D logoTexture;

        [MenuItem("Tools/proTools/proFolder")]
        public static void ShowWindow()
        {
            GetWindow<proFolderEditorWindow>("proFolder");
        }

        private void OnEnable()
        {
            logoTexture = Resources.Load<Texture2D>("proFolderBanner");
        }

        private void OnGUI()
        {
            if (logoTexture != null)
            {
                GUILayout.Label(logoTexture, GUILayout.Height(80));
            }

            int currentIndex = FolderEffectSettings.EffectsEnabled ? 0 : 1;
            int selectedIndex = EditorGUILayout.Popup("Enable / Disable", currentIndex, effectOptions);

            if (selectedIndex != currentIndex)
            {
                FolderEffectSettings.EffectsEnabled = selectedIndex == 0;
                EditorApplication.RepaintProjectWindow();
            }

            int currentHotkeyIndex = FolderEffectSettings.HotkeyType ? 0 : 1;
            int selectedHotkeyIndex = EditorGUILayout.Popup("Hotkey", currentHotkeyIndex, hotkeyOptions);

            if (selectedHotkeyIndex != currentHotkeyIndex)
            {
                FolderEffectSettings.HotkeyType = selectedHotkeyIndex == 0;
            }

            GUILayout.Space(10);

            GUILayout.Label("Folder Color Presets", EditorStyles.boldLabel);

            for (int i = 0; i < colorPresets.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                colorPresets[i] = EditorGUILayout.ColorField($"Color {i + 1}", colorPresets[i]);

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    colorPresets.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("New Color"))
            {
                colorPresets.Add(Color.white);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear All Colors"))
            {
                colorPresets.Clear();
            }

            if (GUILayout.Button("Restore Default Colors"))
            {
                colorPresets.Clear();
                LoadDefaultColors();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(16);

            if (GUILayout.Button("Auto Assign Marks"))
            {
                FolderDataManager.AutoAssignMarkers();
                AssetDatabase.Refresh();
                EditorApplication.RepaintProjectWindow();
            }

            if (GUILayout.Button("Reset Effects"))
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Reset Colors"), false, () => {
                    if (EditorUtility.DisplayDialog("Reset Colors", "Remove all folder colors?", "Yes", "Cancel"))
                    {
                        FolderDataManager.folderColors.Clear();
                        FolderDataManager.Save();
                        EditorApplication.RepaintProjectWindow();
                    }
                });

                menu.AddItem(new GUIContent("Reset Markers"), false, () => {
                    if (EditorUtility.DisplayDialog("Reset Markers", "Remove all folder markers?", "Yes", "Cancel"))
                    {
                        FolderDataManager.folderMarkers.Clear();
                        FolderDataManager.Save();
                        EditorApplication.RepaintProjectWindow();
                    }
                });

                menu.AddItem(new GUIContent("Reset All"), false, () => {
                    if (EditorUtility.DisplayDialog("Reset All", "Remove all folder colors and markers?", "Yes", "Cancel"))
                    {
                        FolderDataManager.folderColors.Clear();
                        FolderDataManager.folderMarkers.Clear();
                        FolderDataManager.Save();
                        EditorApplication.RepaintProjectWindow();
                    }
                });

                menu.ShowAsContext();
            }
        }

        private void LoadDefaultColors()
        {
            colorPresets.AddRange(new List<Color>
            {
                new Color(0.91f, 0.27f, 0.27f),
                new Color(0.99f, 0.60f, 0.30f),
                new Color(0.98f, 0.88f, 0.37f),
                new Color(0.71f, 0.91f, 0.42f),
                new Color(0.49f, 0.84f, 0.53f),
                new Color(0.38f, 0.84f, 0.78f),
                new Color(0.38f, 0.70f, 0.99f),
                new Color(0.58f, 0.55f, 0.97f),
                new Color(0.76f, 0.45f, 0.91f),
                new Color(0.91f, 0.47f, 0.76f),
            });
        }
    }
}