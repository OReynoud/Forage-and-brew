using NaughtyAttributes;
using UnityEngine;

public class CharacterAnimManager : Singleton<CharacterAnimManager>
{
    [SerializeField] public Animator animator;
    [SerializeField] public AudioSource purrSound;
    [SerializeField] public GameObject codexObject;
    [SerializeField] public GameObject dropShadow;
    public bool isSitting;
    private float sitTimer;
    public AnimationCurve sitCurve;
    public Vector3 couchPlayerOffset;
    public Vector3 couchDropShadowOffset;
    private Vector3 dropShadowOriginalPos;
    [SerializeField] public GameObject catPelvis;

    [BoxGroup("Blinking Animation")] [SerializeField] private float minTimeBetweenBlinks;
    [BoxGroup("Blinking Animation")] [SerializeField] private float maxTimeBetweenBlinks;
    [BoxGroup("Debug")] private float timeForNextBlink;
    
    [BoxGroup("Wagging Animation")] [SerializeField] private float minTimeBetweenTail;
    [BoxGroup("Wagging Animation")] [SerializeField] private float maxTimeBetweenTail;
    [BoxGroup("Debug")] private float timeForNextTail;
    
    [BoxGroup("Flick Animation")] [SerializeField] private float minTimeBetweenFlick;
    [BoxGroup("Flick Animation")] [SerializeField] private float maxTimeBetweenFlick;
    [BoxGroup("Debug")] private float timeForNextFlick;

    [BoxGroup("AFK")] [SerializeField] private float timeBeforeAfk;
    private float _currentTimeBeforeAfk;
    
    private static readonly int DoBlink = Animator.StringToHash("DoBlink");
    private static readonly int DoWag = Animator.StringToHash("DoWag");
    private static readonly int DoFlick = Animator.StringToHash("DoFlick");
    public static readonly int IsCarrying = Animator.StringToHash("IsCarrying");
    private static readonly int DoAfk = Animator.StringToHash("DoAfk");
    private static readonly int DoCodexOpen = Animator.StringToHash("DoCodexOpen");
    private static readonly int DoCodexClose = Animator.StringToHash("DoCodexClose");


    private void Start()
    {
        timeForNextBlink = Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks);
        timeForNextTail = Random.Range(minTimeBetweenTail, maxTimeBetweenTail);
        timeForNextFlick = Random.Range(minTimeBetweenFlick, maxTimeBetweenFlick);
        _currentTimeBeforeAfk = timeBeforeAfk;
        CharacterInputManager.Instance.OnCodexUse.AddListener(UseCodex);
        dropShadowOriginalPos = dropShadow.transform.localPosition;
    }

    private void UseCodex(bool state)
    {

        if (state)
        {
            animator.SetTrigger(DoCodexOpen);
        }
        else
        {
            CharacterInputManager.Instance.showCodex = false;
            animator.SetTrigger(DoCodexClose);
            InfoDisplayManager.instance.HideBackground();
        }
        

    }

    private void Update()
    {
        timeForNextFlick -= Time.deltaTime;
        if (timeForNextFlick < 0f)
        {
            animator.SetTrigger(DoFlick);
            timeForNextFlick = Random.Range(minTimeBetweenFlick, maxTimeBetweenFlick);
        }
        
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Sleep"))
        {
            timeForNextBlink -= Time.deltaTime;
            if (timeForNextBlink < 0f)
            {
                animator.SetTrigger(DoBlink);
                timeForNextBlink = Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks);
            }
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("NoTailWag") && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Sleep"))
        {
            timeForNextTail -= Time.deltaTime;
            if (timeForNextTail < 0f)
            {
                animator.SetTrigger(DoWag);
                timeForNextTail = Random.Range(minTimeBetweenTail, maxTimeBetweenTail);
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(2).IsName("A_Cat_Idle") && animator.GetCurrentAnimatorStateInfo(0).IsName("A_Cat_Idle"))
        {
            _currentTimeBeforeAfk -= Time.deltaTime;
            if (_currentTimeBeforeAfk < 0f)
            {
                animator.SetTrigger(DoAfk);
                _currentTimeBeforeAfk = timeBeforeAfk;
            }
        

        }
        else
        {
            _currentTimeBeforeAfk = timeBeforeAfk;
        }
        
        animator.SetBool(IsCarrying, CharacterInteractController.Instance.AreHandsFull);

        if (isSitting)
        {
            if (sitTimer < 1)
            {
                sitTimer += Time.deltaTime;        
                dropShadow.transform.localPosition = Vector3.Lerp(dropShadowOriginalPos, dropShadowOriginalPos + Vector3.forward, sitCurve.Evaluate(sitTimer/1));
            }
        }
        else
        {
            if (sitTimer > 0)
            {
                sitTimer -= Time.deltaTime;        
                dropShadow.transform.localPosition = Vector3.Lerp(dropShadowOriginalPos + couchDropShadowOffset, dropShadowOriginalPos + Vector3.forward, sitCurve.Evaluate(sitTimer/1));
            }
        }

        
    }


    public void UseCouch()
    {
        isSitting = true;
    }
    public void LeaveCouch()
    {
        isSitting = false;
    }

    public void RepositionPlayerAfterCouch()
    {
        CharacterInputManager.Instance.EnableInputs();
        transform.LookAt(transform.position -transform.forward);
        dropShadow.transform.localPosition = dropShadowOriginalPos;
        transform.position += couchPlayerOffset;
        Debug.Log(transform.eulerAngles);
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
