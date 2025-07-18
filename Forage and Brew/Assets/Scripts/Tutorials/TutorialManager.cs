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
    public void NotifyFromRecipeReceived()
    {

    }
    public void NotifyFromCompletePotion()
    {

    }

    public void NotifyFromCompleteOrder()
    {
        
    }

    public void NotifyFromNewDay()
    {
        CharacterInputManager.Instance.EnterCodexMethod();
        if (CodexContentManager.instance.pageIndexesToCheck[^1] % 2 == 1)
        {
            AutoFlip.instance.ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1] + 1);
        }
        else
        {
            AutoFlip.instance.ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1]);
        }
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
