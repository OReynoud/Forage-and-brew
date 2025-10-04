using UnityEngine;

public class WateringCanBehavior : StateMachineBehaviour
{
    private float timer;
    public float timeEnableCan;
    private bool enabledCan;
    public float timeDisableCan;
    private bool disabledCan;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterInputManager.Instance.DisableMoveInputs();
        CharacterInputManager.Instance.DisableCodexInputs();
        CharacterInputManager.Instance.DisableInteractInputs();
        timer = 0;
        enabledCan = false;
        disabledCan = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > timeEnableCan && !enabledCan)
        {
            enabledCan = true;
            CharacterAnimManager.instance.wateringCan.SetActive(true);
            CharacterAnimManager.instance.wateringCanVfx.SetActive(true);
        }
        if (timer > timeDisableCan && !disabledCan)
        {
            disabledCan = true;
            CharacterAnimManager.instance.wateringCan.SetActive(false);
            CharacterAnimManager.instance.wateringCanVfx.SetActive(false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterInputManager.Instance.EnableMoveInputs();
        CharacterInputManager.Instance.EnableCodexInputs();
        CharacterInputManager.Instance.EnableInteractInputs();
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
