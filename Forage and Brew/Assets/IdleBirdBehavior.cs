using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class IdleBirdBehavior : MonoBehaviour
{
    private static readonly int CycleOffset = Animator.StringToHash("CycleOffset");
    private static readonly int DoFly = Animator.StringToHash("DoFly");
    private Collider col;

    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat(CycleOffset, Random.value);
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            animator.SetTrigger(DoFly);
            col.enabled = false;
        }
    }
}
