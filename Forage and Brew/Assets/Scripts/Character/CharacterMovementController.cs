using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class CharacterMovementController : MonoBehaviour
{
    // Singleton
    public static CharacterMovementController Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private Animator animator;
    [SerializeField] public AudioSource walkAudioSource;
    [SerializeField] public AudioResource walkHome;
    [SerializeField] public AudioResource walkForest;
    [SerializeField] private AudioResource walkSwamp;
    [SerializeField] private Collider coll;
    
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 12;
    [SerializeField] [Range(0,1)] private float rotationSpeed = 0.1f;
    [SerializeField] private float maxAngle;
    [SerializeField] private AnimationCurve accelerationCurve;

    [Foldout("Debug")] [SerializeField] [ReadOnly]
    internal Vector3 playerDir;
    [Foldout("Debug")] [SerializeField] [ReadOnly] private float accelerationCurveIndex;
    [Foldout("Debug")] [SerializeField] [ReadOnly] private bool isGrounded;
    [Foldout("Debug")] [SerializeField] [ReadOnly] private bool isMoving;
    [Foldout("Debug")] public bool isRunning;
    
    
    [SerializeField] private float walkFootStepInterval;
    [SerializeField] private float runFootStepInterval;
    [SerializeField] private float footStepTimer;
    
    [HideInInspector]public Rigidbody rb;
    private LayerMask groundMask;
    
    private Vector3 angledVelocity;
    private float angle;
    
    // Animator Hashes
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeedFactor");


    #region Unity Callbacks
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }
    
    private void Start()
    {
        groundMask = LayerMask.GetMask("Default");
    }
    
    

    private void Update()
    {
        isGrounded = GroundCheck();
        PlayerMovement();
    }

    private void FixedUpdate()
    {
        if (isMoving)
            RotatePlayer();
    }

    #endregion

    public void SetupAudio(Scene scene)
    {
        switch (scene)
        {
            case Scene.HouseOutdoor:
            case Scene.Biome1:
                walkAudioSource.resource = walkForest;
                break;
            case Scene.Biome2:
                walkAudioSource.resource = walkSwamp;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scene), scene, scene.ToString());
        }
    }
    
    public void Move(Vector2 inputDir)
    {
        if (transitionWalk)
            return;
        playerDir = Quaternion.AngleAxis(SimpleCameraBehavior.instance.cameraRotation.y, Vector3.up) * new Vector3(inputDir.x,0,inputDir.y);
    }

    private bool GroundCheck()
    {
        bool check = Physics.Raycast(transform.position + Vector3.up * 0.4f, Vector3.down, 0.5f, groundMask);
        
        return check;
    }

    private void PlayerMovement()
    {      
        if (playerDir.magnitude > 0)
        {
            accelerationCurveIndex += Time.deltaTime;
            isMoving = true;
            animator.SetFloat(WalkSpeed,isRunning? runSpeed / walkSpeed : playerDir.magnitude);
            animator.SetBool(IsWalking, true);
            if (footStepTimer <= 0)
            {
                footStepTimer = isRunning ? runFootStepInterval : walkFootStepInterval;
                walkAudioSource.Play();
            }
            else
            {
                footStepTimer -= Time.deltaTime * playerDir.magnitude;
            }
        }
        else
        {
            footStepTimer = 0;
            accelerationCurveIndex = 0;
            isMoving = false;
            isRunning = false;
            animator.SetBool(IsWalking, false);
        }
        
        angle = maxAngle;  
        animator.SetBool(IsRunning, isRunning);
        if (isRunning)
            playerDir.Normalize();
        
        angledVelocity = playerDir;
        if (Physics.Raycast(transform.position + Vector3.up * 0.3f , transform.forward, out RaycastHit hitForward, 0.7f, groundMask))
        {
            angle = Vector3.Angle(hitForward.normal, transform.forward) - 90;
        }
        else if (Physics.Raycast(transform.position + Vector3.up * 0.3f, -transform.forward, out RaycastHit hitBack, 1f, groundMask))
        {
            angle = Vector3.Angle(hitBack.normal, transform.forward) - 90;
        }
        if (isGrounded) 
        {
            if (Mathf.Abs(angle) < maxAngle)
            {
                angledVelocity = Quaternion.AngleAxis(-angle,transform.right) * angledVelocity;
                angledVelocity *= (isRunning ? runSpeed : walkSpeed) * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude;
                rb.linearVelocity = angledVelocity + Vector3.down * (9.81f * Time.deltaTime);
            }
            else
            {
                angledVelocity = playerDir * ((isRunning ? runSpeed : walkSpeed) * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);

                rb.linearVelocity = new Vector3(angledVelocity.x, rb.linearVelocity.y, angledVelocity.z);
            }
            
            Debug.DrawRay(transform.position, angledVelocity * 5, Color.blue, 0);
        }
        else
        {
            if (rb.isKinematic) return;
            
            //Debug.Log("falling");
            angledVelocity = playerDir * ((isRunning ? runSpeed : walkSpeed) * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);

            rb.linearVelocity = new Vector3(angledVelocity.x, rb.linearVelocity.y - 9.81f * Time.deltaTime, angledVelocity.z);
        }

        if (transitionWalk)
        {
            WalkToLocation();
        }
    }
    
    private void RotatePlayer()
    {
        if (playerDir.magnitude < 0.01f)
            return;
        playerDir.Normalize();
        float finalAngle = Mathf.Atan2(playerDir.x, playerDir.z) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, finalAngle, 0), rotationSpeed);
    }

    private bool transitionWalk;
    private bool manageFinalRotation;
    private Vector3 aimedLocation;
    private Quaternion aimedRotation;
    private float timeToWalk;
    private float transitionWalkDuration;
    private float transitionWalkSpeed;
    private float transitionWalkDistance;
    private float aimedLocationTolerance = 0.05f; // Tolerance for the final position
    
    public void TriggerWalkTransition(Vector3 locationToWalk)
    {
        CharacterInputManager.Instance.DisableMoveInputs();
        
        coll.isTrigger = true;
        rb.useGravity = false;
        
        manageFinalRotation = false;
        aimedLocation = locationToWalk;
        transitionWalk = true;
        float walkDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(aimedLocation.x, aimedLocation.z)) - 0.2f;
        transform.LookAt(locationToWalk);
        playerDir = transform.forward;
        timeToWalk = walkDistance / walkSpeed;
        Debug.DrawLine(transform.position, locationToWalk, Color.red, 5);
    }
    
    public void TriggerWalkTransition(Transform locationToWalk, float walkDuration)
    {
        CharacterInputManager.Instance.DisableMoveInputs();
        
        coll.isTrigger = true;
        rb.useGravity = false;
        
        manageFinalRotation = true;
        aimedLocation = locationToWalk.position;
        aimedRotation = locationToWalk.rotation;
        transitionWalk = true;
        transitionWalkDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
            new Vector2(aimedLocation.x, aimedLocation.z));
        transform.LookAt(locationToWalk);
        playerDir = transform.forward;
        transitionWalkDuration = walkDuration;
        timeToWalk = 0f;
        transitionWalkSpeed = transitionWalkDistance / walkDuration;
        Debug.DrawLine(transform.position, locationToWalk.position, Color.red, 5);
    }

    public UnityEvent FinishWalkToLocation = new();

    private void WalkToLocation()
    {
        if (manageFinalRotation)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                    new Vector2(aimedLocation.x, aimedLocation.z)) < aimedLocationTolerance)
            {
                playerDir *= 0;
                rb.linearVelocity *= 0;
                transitionWalk = false;
                coll.isTrigger = false;
                rb.useGravity = true;
                transform.rotation = aimedRotation;
                FinishWalkToLocation?.Invoke();
            }
            else
            {
                float currentTimePart = timeToWalk / transitionWalkDuration;
                rb.linearVelocity = playerDir * Mathf.Max(0.01f, transitionWalkSpeed *
                                                                 Mathf.LerpUnclamped(1.5f, 0.5f, currentTimePart));
                transform.rotation = Quaternion.Slerp(transform.rotation, aimedRotation, Mathf.Lerp(0f, 1f, currentTimePart));
                timeToWalk += Time.deltaTime;
            }
        }
        else
        {
            if (timeToWalk > 0)
            {
                rb.linearVelocity = playerDir * walkSpeed;
                timeToWalk -= Time.deltaTime;
            }
            else
            {
                playerDir *= 0;
                rb.linearVelocity *= 0;
                transitionWalk = false;
                coll.isTrigger = false;
                rb.useGravity = true;
                FinishWalkToLocation?.Invoke();
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,0.4f);
    }
#endif
}
