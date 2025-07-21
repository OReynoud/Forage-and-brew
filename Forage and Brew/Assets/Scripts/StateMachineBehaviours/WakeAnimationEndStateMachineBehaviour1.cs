using UnityEngine;

public class WakeAnimationEndStateMachineBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        CharacterInputManager.Instance.EnableInputs();
        if (GameDontDestroyOnLoadManager.Instance.DayPassed == 1)
        {
            TutorialManager.instance.NotifyFromNewDay();
        }
    }
}
