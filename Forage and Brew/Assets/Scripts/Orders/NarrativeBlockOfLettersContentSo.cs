using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_NarrativeBlockContent", menuName = "Letters/NarrativeBlockOfLettersContentSo")]
public class NarrativeBlockOfLettersContentSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public List<LetterContentSo> Content { get; private set; } = new();
    [field: SerializeField] public int RequiredQuestProgressionIndex { get; private set; }
    
    
    public void SetData(string newName, int requiredQuestProgressionIndex)
    {
        Name = newName;
        RequiredQuestProgressionIndex = requiredQuestProgressionIndex;
    }
}
