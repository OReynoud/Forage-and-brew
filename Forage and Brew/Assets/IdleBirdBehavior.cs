using UnityEngine;

public class IdleBirdBehavior : MonoBehaviour
{
    private static readonly int CycleOffset = Animator.StringToHash("CycleOffset");

    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat(CycleOffset, Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
