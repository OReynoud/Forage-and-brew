using UnityEngine;

public class CollectAnimationEndStateMachineBehaviour : StateMachineBehaviour
{
    private bool IsCollectectAnimFinished;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsCollectectAnimFinished) return;

        if (stateInfo.normalizedTime > stateInfo.length - 0.25f)
        {
            IsCollectectAnimFinished = true;
            animator.transform.parent.GetComponent<CollectHapticChallengeManager>().OnCollectAnimationEnd();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IsCollectectAnimFinished = false;
    }
}
