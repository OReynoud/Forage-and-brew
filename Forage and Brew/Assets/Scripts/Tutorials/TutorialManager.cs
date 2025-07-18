using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [field: SerializeField] [field: Expandable] public List<TutoBlockSo> ContentSo { get; set; } = new();
    [field: SerializeField] public List<TutorialBlock> TutorialPopups { get; set; } = new();
    public TextMeshProUGUI headText;
    public TextMeshProUGUI bodyText;

    public override void Awake()
    {
        base.Awake();
        foreach (var tuto in ContentSo)
        {
            TutorialPopups.Add(new TutorialBlock(tuto));
        }
    }

    public void NotifyFromZoneTrigger(string triggerID)
    {

    }


    public void NotifyFromIngredientReceived()
    {

    }
    public void NotifyFromRecipeReceived(string workshop)
    {
        if (!CodexContentManager.instance.tutorialDissolves.ContainsKey(workshop))
            return;
        
        CodexContentManager.instance.pageIndexesToCheck.Insert(0, CodexContentManager.instance.tutorialDissolves[workshop].pageToCheck);
        CodexContentManager.instance.tutorialDissolvesToCheck.Add(workshop);
    }
    public void NotifyFromCompletePotion()
    {

    }

    public void NotifyFromCompleteOrder()
    {
        if (!CodexContentManager.instance.tutorialDissolves.ContainsKey("CompleteOrder"))
            return;
        CodexContentManager.instance.pageIndexesToCheck.Add(CodexContentManager.instance.tutorialDissolves["CompleteOrder"].pageToCheck);
        if (CodexContentManager.instance.pageIndexesToCheck[^1] % 2 == 1)
        {
            AutoFlip.instance.ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1] + 1);
        }
        else
        {
            AutoFlip.instance.ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1]);
        }
        CharacterInputManager.Instance.EnterCodexMethod();
        
        
        CodexContentManager.instance.tutorialDissolvesToCheck.Add("CompleteOrder");
        CharacterInputManager.Instance.DisableCodexInputs();
        CharacterInputManager.Instance.DisableInputs();
        
        AutoFlip.instance.ContinuePageDiscovery(true);
    }

    public void NotifyFromNewDay()
    {
        if (!CodexContentManager.instance.tutorialDissolves.ContainsKey("IngredientApparition"))
            return;
        
        CodexContentManager.instance.pageIndexesToCheck.Add(CodexContentManager.instance.tutorialDissolves["IngredientApparition"].pageToCheck);
        if (CodexContentManager.instance.pageIndexesToCheck[^1] % 2 == 1)
        {
            AutoFlip.instance.ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1] + 1);
        }
        else
        {
            AutoFlip.instance.ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1]);
        }
        CharacterInputManager.Instance.EnterCodexMethod();
        
        
        CodexContentManager.instance.tutorialDissolvesToCheck.Add("IngredientApparition");
        CharacterInputManager.Instance.DisableCodexInputs();
        CharacterInputManager.Instance.DisableInputs();
        
        AutoFlip.instance.ContinuePageDiscovery(true);
    }

    // foreach (var tutorial in TutorialPopups)
    // {
    //     if (CheckValidTags(tutorial, TutorialTriggerConditions.ObtainPotion)) 
    //         continue;
    //
    //     ShowTutorialPopup(tutorial);
    //     break;
    // }
    public void ShowTutorialPopup(TutorialBlock tutorial)
    {
        tutorial.hasBeenTriggered = true;
        headText.text = tutorial.data.title;
        bodyText.text = tutorial.data.content;
        InfoDisplayManager.instance.tutorialTimer = tutorial.data.stayTime;
    }
    private bool CheckValidTags(TutorialBlock tutorial, TutorialTriggerConditions triggerType)
    {
        if (tutorial.hasBeenTriggered)
            return true;
        if ((tutorial.data.triggerConditions & triggerType) !=
            triggerType)
            return true;
        if ((tutorial.data.triggerConditions & TutorialTriggerConditions.IsCarryingObject) ==
            TutorialTriggerConditions.IsCarryingObject)
        {
            if (!CarryObjectsCheck())
                return true;
        }

        if ((tutorial.data.triggerConditions & TutorialTriggerConditions.TimeOfTheDay) ==
            TutorialTriggerConditions.TimeOfTheDay)
        {
            if (!TimeOfDayCheck(tutorial.data.timeOftheDay))
                return true;
        }

        return false;
    }

    private bool CarryObjectsCheck()
    {
        return CharacterInteractController.Instance.AreHandsFull;
    }
    private bool TimeOfDayCheck(TimeOfDay check)
    {
        return check == GameDontDestroyOnLoadManager.Instance.CurrentTimeOfDay;
    }
}
