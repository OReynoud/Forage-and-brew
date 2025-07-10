using System.Collections.Generic;
using UnityEngine;

namespace proTools.proFolder
{
    [System.Serializable]
    public class MarkerEntry
    {
        public Texture2D icon;
        public string groupName;
        public List<string> fileExtensions = new List<string>();
    }

    [CreateAssetMenu(fileName = "MarkerLibrary", menuName = "proFolder/Marker Library")]
    public class MarkerLibrary : ScriptableObject
    {
        public List<MarkerEntry> entries = new List<MarkerEntry>();
    }
}