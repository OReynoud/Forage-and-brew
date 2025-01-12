using NaughtyAttributes;
using UnityEngine;

[ExecuteAlways]
public class LetterLoader : MonoBehaviour
{
    public LetterMailBoxDisplayBehaviour display;
    public LetterContentSo letterToLoad;

    [Button]
    public void LoadLetter()
    {
        display.InitLetter(letterToLoad);
    }
}
