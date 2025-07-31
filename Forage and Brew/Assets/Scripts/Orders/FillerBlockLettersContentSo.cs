using UnityEngine;

[CreateAssetMenu(fileName = "D_NarrativeBlockContent", menuName = "Letters/FillerBlockOfLettersContentSo")]
public class FillerBlockLettersContentSo : NarrativeBlockOfLettersContentSo
{
    [field: SerializeField] public LetterContentSo FirstFiller { get; private set; } = new();
    
    
    
}
