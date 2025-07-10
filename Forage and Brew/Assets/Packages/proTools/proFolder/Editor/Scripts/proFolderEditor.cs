using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace proTools.proFolder
{
    public class proFolderEditor : EditorWindow
    {
        private static proFolderEditor Instance;

        private static List<Color> colors;
        private static Texture2D[] colorIcons;

        private static MarkerLibrary markerLibrary;

        public static System.Action<Color> OnColorSelected;
        public static System.Action<int> OnMarkSelected;

        public static void Open(Vector2 position)
        {
            if (Instance != null)
            {
                Instance.Close();
                Instance = null;
            }

            colors = proFolderEditorWindow.colorPresets;
            GenerateColorIcons();
            LoadMarkerLibrary();

            int columns = 11;
            int rowHeight = 34;
            int baseHeight = 46;

            int colorRowCount = Mathf.CeilToInt((colors.Count + 1) / (float)columns);

            var groups = markerLibrary.entries.GroupBy(entry => entry.groupName);

            int totalMarkerRows = 0;
            foreach (var group in groups)
            {
                int groupIconCount = group.Count();
                int iconRows = Mathf.CeilToInt(groupIconCount / (float)columns);
                totalMarkerRows += iconRows;
            }

            int totalHeight = (colorRowCount + totalMarkerRows) * rowHeight + baseHeight;

            Instance = CreateInstance<proFolderEditor>();
            Instance.position = new Rect(position.x, position.y, 388, totalHeight);
            Instance.ShowPopup();
        }

        private void OnDisable()
        {
            Instance = null;
        }

        private void OnLostFocus()
        {
            this.Close();
            Instance = null;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                this.Close();
                Instance = null;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("proFolder", EditorStyles.boldLabel);
            GUILayout.Space(8);
            DrawColorGrid();
            GUILayout.Space(16);
            DrawMarkGrid();
        }

        private void DrawColorGrid()
        {
            int iconSize = 32;
            int maxColumns = 11;

            int count = colorIcons.Length + 1;

            for (int i = 0; i < count; i++)
            {
                if (i % maxColumns == 0)
                {
                    GUILayout.BeginHorizontal();
                }

                if (i == 0)
                {
                    if (GUILayout.Button(markerLibrary.entries[0].icon, GUILayout.Width(32), GUILayout.Height(32)))
                    {
                        OnColorSelected?.Invoke(Color.clear);
                        Close();
                    }
                }
                else
                {
                    int colorIndex = i - 1;
                    if (GUILayout.Button(colorIcons[colorIndex], GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
                    {
                        OnColorSelected?.Invoke(colors[colorIndex]);
                        Close();
                    }
                }

                if ((i % maxColumns) == (maxColumns - 1) || i == count - 1)
                {
                    GUILayout.EndHorizontal();
                }
            }
        }

        private void DrawMarkGrid()
        {
            int iconSize = 32;
            int maxColumns = 11;

            var groups = markerLibrary.entries
                .Select((entry, index) => new { entry, index })
                .GroupBy(x => x.entry.groupName);

            foreach (var group in groups)
            {
                int col = 0;
                GUILayout.BeginHorizontal();
                foreach (var item in group)
                {
                    if (col >= maxColumns)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        col = 0;
                    }

                    if (GUILayout.Button(item.entry.icon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
                    {
                        OnMarkSelected?.Invoke(item.index);
                        Close();
                    }

                    col++;
                }
                GUILayout.EndHorizontal();
            }
        }

        private static void GenerateColorIcons()
        {
            int size = 32;
            colorIcons = new Texture2D[colors.Count];

            for (int i = 0; i < colors.Count; i++)
            {
                var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
                tex.wrapMode = TextureWrapMode.Clamp;

                Color color = colors[i];
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        tex.SetPixel(x, y, color);
                    }
                }

                tex.Apply();
                colorIcons[i] = tex;
            }
        }

        private static void LoadMarkerLibrary()
        {
            if (markerLibrary == null)
            {
                markerLibrary = AssetDatabase.LoadAssetAtPath<MarkerLibrary>("Assets/Packages/proTools/proFolder/SO/MarkerLibrary.asset");
            }
        }
    }
}