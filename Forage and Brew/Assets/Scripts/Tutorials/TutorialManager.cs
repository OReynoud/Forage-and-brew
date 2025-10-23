using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : Singleton<TutorialManager>
{
    [field: SerializeField] [field: Expandable] public List<TutoBlockSo> ContentSo { get; set; } = new();
    [field: SerializeField] public List<TutorialBlock> TutorialPopups { get; set; } = new();
    public IngredientValuesSo brownCapValuesSo;
    public TextMeshProUGUI headText;
    public TextMeshProUGUI bodyText;
    public float potionCompleteDelay = 1f;

    public UnityEvent OnCodexClose { get; set; } = new UnityEvent();

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
        if (!CodexContentManager.instance.tutorialDissolves.ContainsKey(triggerID)) return;
        
        if (GameDontDestroyOnLoadManager.Instance.CollectedIngredients.FindAll(x => x == brownCapValuesSo).Count >= 2 && triggerID == "PotionTuto")
        {
            CodexContentManager.instance.pageIndexesToCheck.Insert(0, CodexContentManager.instance.tutorialDissolves[triggerID].pageToCheck);
            CodexContentManager.instance.tutorialDissolvesToCheck.Add(triggerID);
            CharacterInputManager.Instance.EnterCodexMethod();
            CharacterInputManager.Instance.DisableCodexInputs();
            CharacterInputManager.Instance.DisableInputs();
        
            AutoFlip.instance.ContinuePageDiscovery(true);
            return;
        }
    }


    public void NotifyFromIngredientReceived()
    {
        if (CodexContentManager.instance.tutorialDissolves.ContainsKey("2BrownCap"))
        {
            if (GameDontDestroyOnLoadManager.Instance.CollectedIngredients.FindAll(x => x == brownCapValuesSo).Count + 
                GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.FindAll(x => x.IngredientValuesSo == brownCapValuesSo).Count +
                GameDontDestroyOnLoadManager.Instance.FloorCollectedIngredients.FindAll(x => x.Ingredient == brownCapValuesSo).Count
                >= 2)
            {
                CodexContentManager.instance.pageIndexesToCheck.Insert(0, CodexContentManager.instance.tutorialDissolves["2BrownCap"].pageToCheck);
                CodexContentManager.instance.tutorialDissolvesToCheck.Add("2BrownCap");
                CharacterInputManager.Instance.EnterCodexMethod();
                CharacterInputManager.Instance.DisableCodexInputs();
                CharacterInputManager.Instance.DisableInputs();
        
                AutoFlip.instance.ContinuePageDiscovery(true);
            }
        }

    }
    public void NotifyFromRecipeReceived(string workshop)
    {
        if (!CodexContentManager.instance.tutorialDissolves.ContainsKey(workshop))
            return;
        
        CodexContentManager.instance.pageIndexesToCheck.Insert(0, CodexContentManager.instance.tutorialDissolves[workshop].pageToCheck);
        CodexContentManager.instance.tutorialDissolvesToCheck.Add(workshop);
        var temp = CodexContentManager.instance.pageIndexesToCheck.Count -
                   CodexContentManager.instance.tutorialDissolvesToCheck.Count;
        if (temp >= 1)
        {
            for (int i = 0; i < temp; i++)
            {
                CodexContentManager.instance.tutorialDissolvesToCheck.Add("");
            }
        }

    }
    public void NotifyFromCompletePotion()
    {
        if (!CodexContentManager.instance.tutorialDissolves.ContainsKey("CompletePotion"))
            return;
        StartCoroutine(CompletePotionRoutine());

    }

    private IEnumerator CompletePotionRoutine()
    {
        yield return new WaitForSeconds(potionCompleteDelay);
        OnCodexClose.AddListener(FinishNotifyFromPotion);
        CharacterAnimManager.instance.animator.SetLayerWeight(1, 0);
        CodexContentManager.instance.pageIndexesToCheck.Add(CodexContentManager.instance.tutorialDissolves["CompletePotion"]
            .pageToCheck);
        if (CodexContentManager.instance.pageIndexesToCheck[^1] % 2 == 1)
        {
            AutoFlip.instance.ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1] + 1);
        }
        else
        {
            AutoFlip.instance.ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1]);
        }

        CharacterInputManager.Instance.EnterCodexMethod();


        CodexContentManager.instance.tutorialDissolvesToCheck.Add("CompletePotion");
        CharacterInputManager.Instance.DisableCodexInputs();
        CharacterInputManager.Instance.DisableInputs();

        AutoFlip.instance.ContinuePageDiscovery(true);
    }

    public void FinishNotifyFromPotion()
    {
        OnCodexClose.RemoveListener(FinishNotifyFromPotion);
        CharacterAnimManager.instance.animator.SetLayerWeight(1, 1);
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
