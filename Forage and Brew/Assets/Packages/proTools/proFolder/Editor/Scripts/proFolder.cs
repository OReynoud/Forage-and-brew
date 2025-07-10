using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace proTools.proFolder
{
    [InitializeOnLoad]
    public static class proFolder
    {
        private static Dictionary<string, Color> folderColors = new();
        private static Dictionary<string, int> folderMarkers = new();

        private static MarkerLibrary markerLibrary;

        static proFolder()
        {
            FolderDataManager.Load();

            LoadMarkerLibrary();

            EditorApplication.projectWindowItemOnGUI += OnProjectWindowGUI;
        }

        private static void LoadMarkerLibrary()
        {
            if (markerLibrary == null)
            {
                markerLibrary = AssetDatabase.LoadAssetAtPath<MarkerLibrary>("Assets/Packages/proTools/proFolder/SO/MarkerLibrary.asset");
            }
        }

        private static Texture2D gradientTexture;
        private static Color lastGradientColor = new Color(0, 0, 0, 0);

        private static void EnsureGradientTexture(Color baseColor)
        {
            if (gradientTexture != null && baseColor.Equals(lastGradientColor))
                return;

            lastGradientColor = baseColor;

            int width = 128;
            int height = 1;

            if (gradientTexture == null)
                gradientTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            else
                gradientTexture.Reinitialize(width, height);

            gradientTexture.wrapMode = TextureWrapMode.Clamp;

            float startAlpha = 0.4f;
            float endAlpha = 0.1f;

            for (int x = 0; x < width; x++)
            {
                float t = x / (float)(width - 1);
                float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                Color pixelColor = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                gradientTexture.SetPixel(x, 0, pixelColor);
            }

            gradientTexture.Apply();
        }

        private static void OnProjectWindowGUI(string guid, Rect rect)
        {
            if (!FolderEffectSettings.EffectsEnabled) return;

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path)) return;

            bool isColumnView = rect.width > rect.height;

            if (FolderDataManager.folderColors.TryGetValue(path, out var color))
            {
                if (isColumnView)
                {
                    EnsureGradientTexture(color);

                    Rect fullRect = new Rect(rect.x - 160, rect.y, rect.width + 160, rect.height);
                    GUI.DrawTexture(fullRect, gradientTexture, ScaleMode.StretchToFill, true);
                }
                else
                {
                    Texture2D whiteFolderIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Packages/proTools/proFolder/UI/Marks/Base/d_Folder.png");
                    if (whiteFolderIcon != null)
                    {
                        Rect iconRect = new Rect(rect.x, rect.y, rect.width, rect.width);

                        Color oldColor = GUI.color;
                        GUI.color = color;
                        GUI.DrawTexture(iconRect, whiteFolderIcon, ScaleMode.ScaleToFit);
                        GUI.color = oldColor;
                    }
                }
            }

            if (FolderDataManager.folderMarkers.TryGetValue(path, out var index) && markerLibrary != null && index >= 0 && index < markerLibrary.entries.Count)
            {
                var icon = markerLibrary.entries[index].icon;
                if (icon != null && index != 0)
                {
                    float size = isColumnView ? rect.height * 0.5f : rect.height * 0.35f;

                    float offsetX = isColumnView ? rect.x + rect.height - size + rect.height * 0.1f : rect.xMax - size;
                    float offsetY = isColumnView ? rect.y + rect.height - size : rect.y + rect.height * 0.65f - size * 0.5f;

                    Rect iconRect = new Rect(offsetX, offsetY, size, size);

                    GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                }
            }

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                bool triggered = false;

                switch (FolderEffectSettings.HotkeyType)
                {
                    case false:
                        triggered = Event.current.button == 0 && Event.current.shift && Event.current.alt;
                        break;

                    case true:
                        triggered = Event.current.button == 0 && Event.current.control && Event.current.alt;
                        break;
                }

                if (triggered)
                {
                    Vector2 mouseScreenPos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

                    proFolderEditor.OnColorSelected = selectedColor =>
                    {
                        if (selectedColor.a == 0f || selectedColor == Color.clear)
                        {
                            FolderDataManager.folderColors.Remove(path);
                        }
                        else
                        {
                            FolderDataManager.folderColors[path] = selectedColor;
                        }

                        FolderDataManager.Save();
                        EditorApplication.RepaintProjectWindow();
                    };

                    proFolderEditor.OnMarkSelected = selectedMarker =>
                    {
                        if (selectedMarker == 0)
                        {
                            FolderDataManager.folderMarkers.Remove(path);
                        }
                        else
                        {
                            FolderDataManager.folderMarkers[path] = selectedMarker;
                        }

                        FolderDataManager.Save();
                        EditorApplication.RepaintProjectWindow();
                    };

                    proFolderEditor.Open(mouseScreenPos);
                    Event.current.Use();
                }
            }
        }
    }

    [InitializeOnLoad]
    public static class proFolderImportPrompt
    {
        static proFolderImportPrompt()
        {
            if (!SessionState.GetBool("proFolder_Reload", false))
            {
                SessionState.SetBool("proFolder_Reload", true);

                EditorApplication.delayCall += () =>
                {
                    EditorUtility.RequestScriptReload();
                };
            }
        }
    }
}