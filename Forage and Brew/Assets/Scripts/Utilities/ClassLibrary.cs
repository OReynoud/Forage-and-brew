using System;

[Serializable]
public class Letter
{
    public LetterContainer associatedLetter;
    public LetterContentSO LetterContent;
    public int days;

    public Letter(LetterContentSO letter, int delay )
    {
        LetterContent = letter;
        days = delay;
    }
}
