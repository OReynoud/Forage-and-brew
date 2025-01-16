using NaughtyAttributes;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    // Singleton
    public static CharacterMovementController Instance { get; private set; }
    
    [Header("Dependencies")]
    [SerializeField] private Animator animator;
    
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 12;
    [SerializeField] [Range(0,1)] private float rotationSpeed = 0.1f;
    [SerializeField] private float maxAngle;
    [SerializeField] private AnimationCurve accelerationCurve;

    [Foldout("Debug")] [SerializeField] [ReadOnly] private Vector3 playerDir;
    [Foldout("Debug")] [SerializeField] [ReadOnly] private float accelerationCurveIndex;
    [Foldout("Debug")] [SerializeField] [ReadOnly] private bool isGrounded;
    [Foldout("Debug")] [SerializeField] [ReadOnly] private bool isMoving;
    [Foldout("Debug")] public bool isRunning;
    
    private Rigidbody rb;
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
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    
    public void Move(Vector2 inputDir)
    {
        playerDir = Quaternion.AngleAxis(CameraController.instance.cameraRotation.y, Vector3.up) * new Vector3(inputDir.x,0,inputDir.y);
    }

    private bool GroundCheck()
    {
        bool check = Physics.Raycast(transform.position + Vector3.up * 0.4f, Vector3.down, 0.5f, groundMask);
        
        return check;
    }

    private void PlayerMovement()
    {
        
        animator.SetBool(IsRunning, isRunning);
        if (isRunning)
            playerDir.Normalize();
        
        angledVelocity = playerDir;
        if (Physics.Raycast(transform.position , transform.forward, out RaycastHit hitForward, 0.7f, groundMask))
        {
            angle = Vector3.Angle(hitForward.normal, transform.forward) - 90;
        }
        else if (Physics.Raycast(transform.position , -transform.forward, out RaycastHit hitBack, 1f, groundMask))
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
            angledVelocity = playerDir * ((isRunning ? runSpeed : walkSpeed) * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);

            rb.linearVelocity = new Vector3(angledVelocity.x, rb.linearVelocity.y - 9.81f * Time.deltaTime, angledVelocity.z);
        }

        if (playerDir.magnitude > 0)
        {
            accelerationCurveIndex += Time.deltaTime;
            isMoving = true;
            animator.SetFloat(WalkSpeed,isRunning? runSpeed / walkSpeed : playerDir.magnitude);
            animator.SetBool(IsWalking, true);
        }
        else 
        {
            accelerationCurveIndex = 0;
            isMoving = false;
            isRunning = false;
            animator.SetBool(IsWalking, false);
        }
    }
    
    private void RotatePlayer()
    {
        float finalAngle = Mathf.Atan2(playerDir.x, playerDir.z) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, finalAngle, 0), rotationSpeed);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,0.4f);
    }
#endif
}
