using UnityEngine;

public class CodexCloseEndStateBehavior : StateMachineBehaviour
{
    public bool isExitTriggered;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Walk_LowerBody"),1f);
        
        animator.SetLayerWeight(animator.GetLayerIndex("Walk_FullBody"),0f);
        
        CharacterInputManager.Instance.DisableCodexInputs();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log(stateInfo.normalizedTime);
        animator.SetLayerWeight(animator.GetLayerIndex("Walk_LowerBody"),1 - stateInfo.normalizedTime/1.2f);
        
        animator.SetLayerWeight(animator.GetLayerIndex("Walk_FullBody"),stateInfo.normalizedTime/1.2f);
        if (isExitTriggered) return;
        if (stateInfo.normalizedTime > 0.95f)
        {
            isExitTriggered = true;
            CharacterAnimManager.instance.codexObject.SetActive(false);
            
            CharacterInputManager.Instance.EnableInteractInputs();
            CharacterInputManager.Instance.EnableHapticChallengeInputs(); 
            CharacterInputManager.Instance.EnableCodexInputs();
            

        }
    }

     //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isExitTriggered = false;
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
