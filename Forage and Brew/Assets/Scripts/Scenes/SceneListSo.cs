using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_SceneList", menuName = "Scenes/SceneListSo")]
public class SceneListSo : ScriptableObject
{
    [field: SerializeField] public List<SceneName> SceneNames { get; private set; }
}
