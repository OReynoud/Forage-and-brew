using UnityEngine;

public class CollectAnimationEndStateMachineBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<CollectHapticChallengeManager>().OnCollectAnimationEnd();
    }
}
