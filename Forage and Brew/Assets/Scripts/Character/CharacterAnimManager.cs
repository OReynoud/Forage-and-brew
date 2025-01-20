using NaughtyAttributes;
using UnityEngine;

public class CharacterAnimManager : Singleton<CharacterAnimManager>
{
    [SerializeField] public Animator animator;
    [SerializeField] public AudioSource purrSound;

    [BoxGroup("Blinking Animation")] [SerializeField] private float minTimeBetweenBlinks;
    [BoxGroup("Blinking Animation")] [SerializeField] private float maxTimeBetweenBlinks;
    private float timeForNextBlink;

    [BoxGroup("AFK")] [SerializeField] private float timeBeforeAfk;
    private float _currentTimeBeforeAfk;
    
    private static readonly int DoBlink = Animator.StringToHash("DoBlink");
    public static readonly int IsCarrying = Animator.StringToHash("IsCarrying");
    private static readonly int DoAfk = Animator.StringToHash("DoAfk");


    private void Start()
    {
        timeForNextBlink = Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks);
    }

    private void Update()
    {
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("A_Cat_Idle"))
        {
            _currentTimeBeforeAfk -= Time.deltaTime;
        
            if (_currentTimeBeforeAfk < 0f)
            {
                animator.SetTrigger(DoAfk);
                _currentTimeBeforeAfk = timeBeforeAfk;
            }
            
            timeForNextBlink -= Time.deltaTime;
        
            if (timeForNextBlink < 0f)
            {
                animator.SetTrigger(DoBlink);
                timeForNextBlink = Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks);
            }
        }
        else
        {
            _currentTimeBeforeAfk = timeBeforeAfk;
        }
        
        animator.SetBool(IsCarrying, CharacterInteractController.Instance.AreHandsFull);
    }
    
    
    public void PlayPurrSound()
    {
        purrSound.Play();
    }
    
    public void StopPurrSound()
    {
        purrSound.Stop();
    }
}
