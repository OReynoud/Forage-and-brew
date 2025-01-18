using NaughtyAttributes;
using UnityEngine;

public class CharacterAnimManager : Singleton<CharacterAnimManager>
{
    [SerializeField] public Animator animator;

    [BoxGroup("Blinking Animation")] [SerializeField] private float minTimeBetweenBlinks;
    [BoxGroup("Blinking Animation")] [SerializeField] private float maxTimeBetweenBlinks;
    [BoxGroup("Blinking Animation")] private float timeForNextBlink;
    
    private static readonly int DoBlink = Animator.StringToHash("DoBlink");
    public static readonly int IsCarrying = Animator.StringToHash("IsCarrying");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeForNextBlink -= Time.deltaTime;
        if (timeForNextBlink < 0)
        {
            animator.SetTrigger(DoBlink);
            timeForNextBlink = Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks);
        }
        
        animator.SetBool(IsCarrying, CharacterInteractController.Instance.AreHandsFull);
    }
}
