using UnityEngine;
using Random = UnityEngine.Random;

public class IdleBirdBehavior : MonoBehaviour
{
    [SerializeField] private bool canFlyAwayOnTrigger = true;
    
    private Collider _col;
    private Animator _animator;
    
    // Animator Hashes
    private static readonly int CycleOffset = Animator.StringToHash("CycleOffset");
    private static readonly int DoFly = Animator.StringToHash("DoFly");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetFloat(CycleOffset, Random.value);
        _col = GetComponent<Collider>();

        if (!canFlyAwayOnTrigger)
        {
            _col.enabled = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            _animator.SetTrigger(DoFly);
            _col.enabled = false;
        }
    }
}
