using UnityEngine;

public class WakeAnimationEndStateMachineBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterInputManager.Instance.EnableInputs();
        
    }
}
