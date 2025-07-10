using System;
using System.Collections.Generic;
using UnityEngine;

namespace proTools.proFolder
{
    [Serializable]
    public class FolderData
    {
        public string path;
        public Color color;
        public int markerIndex;
    }

    [Serializable]
    public class FolderDataList
    {
        public List<FolderData> folders = new List<FolderData>();
    }
}