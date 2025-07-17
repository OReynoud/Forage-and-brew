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
    public int numberOfAfksToBlend = 2;
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
    [BoxGroup("AFK")] [SerializeField] private int[] layerIndexesToCheckForAfk;
    [BoxGroup("Debug")] [SerializeField] private float _currentTimeBeforeAfk;
    
    private static readonly int DoBlink = Animator.StringToHash("DoBlink");
    private static readonly int DoWag = Animator.StringToHash("DoWag");
    private static readonly int DoFlick = Animator.StringToHash("DoFlick");
    public static readonly int IsCarrying = Animator.StringToHash("IsCarrying");
    private static readonly int DoAfk = Animator.StringToHash("DoAfk");
    private static readonly int AfkIndex = Animator.StringToHash("IndexAFK");
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
        
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Sleep") || !animator.GetCurrentAnimatorStateInfo(0).IsName("Sit_AFK"))
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
        
        if (CheckLayersForAfk())
        {
            _currentTimeBeforeAfk -= Time.deltaTime;
            if (_currentTimeBeforeAfk < 0f)
            {
                animator.SetFloat(AfkIndex, Random.Range(0,numberOfAfksToBlend));
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
                dropShadow.transform.localPosition = Vector3.Lerp(dropShadowOriginalPos, dropShadowOriginalPos + Vector3.forward * 1.5f , sitCurve.Evaluate(sitTimer/1));
            }
        }
        else
        {
            if (sitTimer > 0)
            {
                sitTimer -= Time.deltaTime * 1.2f;        
                dropShadow.transform.localPosition = Vector3.Lerp(dropShadowOriginalPos + couchDropShadowOffset, dropShadowOriginalPos + Vector3.forward * 1.5f, sitCurve.Evaluate(sitTimer/1));
            }
        }

        
    }

    bool CheckLayersForAfk()
    {
        foreach (var i in layerIndexesToCheckForAfk)
        {
            if (!animator.GetCurrentAnimatorStateInfo(i).IsTag("Idle"))
            {
                return false;
            }
        }

        
        return true;
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
        transform.LookAt(transform.position -transform.forward);
        dropShadow.transform.localPosition = dropShadowOriginalPos;
        transform.position += couchPlayerOffset;
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
