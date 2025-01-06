using UnityEngine;

[CreateAssetMenu(fileName = "D_NarrativeBlockContent", menuName = "Letters/NarrativeBlockOfLettersContentSo")]
public class NarrativeBlockOfLettersContentSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public LetterContentSo[] Content { get; private set; }
    [field: SerializeField] public int RequiredQuestProgressionIndex { get; private set; }
    [field: SerializeField] public bool CanAdvanceQuestProgressionIndex { get; set; }
}
