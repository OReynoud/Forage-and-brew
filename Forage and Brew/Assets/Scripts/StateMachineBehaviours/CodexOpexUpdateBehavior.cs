using UnityEngine;

public class CodexOpexUpdateBehavior : StateMachineBehaviour
{
    public static bool isCodexOpenTriggered;
    private bool isCodexActiveTriggered;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterInputManager.Instance.DisableMoveInputs();
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isCodexOpenTriggered) return;

        if (stateInfo.normalizedTime / stateInfo.length > CharacterInputManager.Instance.percentAnimDelayOpenCodex)
        {
            isCodexOpenTriggered = true;
            CharacterInputManager.Instance.showCodex = true;
            
            InfoDisplayManager.instance.ShowBackground();
            if (CodexContentManager.instance.pageIndexesToCheck.Count > 0 || CodexContentManager.instance.isDiscoveringNewIngredient)
                return;
            CharacterInputManager.Instance.EnableMoveInputs();
            
            if (!CodexPickUpBehaviour.doTutorialPages)
            {
                CharacterInputManager.Instance.EnableCodexInputs();
                CharacterInputManager.Instance.EnableCodexExit();
            }
        }

        if (isCodexActiveTriggered) return;
        var transitionInfo = animator.GetAnimatorTransitionInfo(layerIndex);

        if (transitionInfo.normalizedTime > transitionInfo.duration + 0.1f)
        {
            CharacterAnimManager.instance.codexObject.SetActive(true);
            isCodexActiveTriggered = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        isCodexOpenTriggered = false;
        isCodexActiveTriggered = false;
        CodexContentManager.instance.isDiscoveringNewIngredient = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
