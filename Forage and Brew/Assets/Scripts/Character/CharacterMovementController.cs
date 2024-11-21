using System;
using NaughtyAttributes;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    public float speed;
    [Range(0,1)]public float rotationSpeed = 0.1f;
    public float maxAngle;
    public AnimationCurve accelerationCurve;

    [Foldout("Debug")] [ReadOnly] private Vector3 playerDir;
    [Foldout("Debug")] [ReadOnly]  public float accelerationCurveIndex;
    [Foldout("Debug")] [ReadOnly] public bool isGrounded;
    [Foldout("Debug")] [ReadOnly] public bool isMoving;
    
    
    private Rigidbody rb;

    private LayerMask groundMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundMask = LayerMask.GetMask("Default");
    }

    public void Move(Vector2 inputDir)
    {
        playerDir = new Vector3(inputDir.x,0,inputDir.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,0.4f);
    }

    // Update is called once per frame
    private Vector3 angledVelocity;
    private float angle;
    void Update()
    {
        isGrounded = GroundCheck();
        PlayerMovement();
    }

    
    void FixedUpdate()
    {
        if (isMoving)
            RotatePlayer();
    }

    bool GroundCheck()
    {
        bool check = Physics.Raycast(transform.position + Vector3.up * 0.6f, Vector3.down, 0.7f, groundMask);
        
        return check;
    }
    
    void PlayerMovement()
    {
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
                angledVelocity *= (speed * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);
                rb.linearVelocity = angledVelocity + Vector3.down * 9.81f * Time.deltaTime;
            }
            else
            {
                angledVelocity = playerDir * (speed * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);

                rb.linearVelocity = new Vector3(angledVelocity.x, rb.linearVelocity.y, angledVelocity.z);
            }
            
            Debug.DrawRay(transform.position,angledVelocity* 5,Color.blue ,0);

        }
        else
        {
            angledVelocity = playerDir * (speed * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);

            rb.linearVelocity = new Vector3(angledVelocity.x, rb.linearVelocity.y - 9.81f * Time.deltaTime,angledVelocity.z);
        }

        if (playerDir.magnitude > 0)
        {
            accelerationCurveIndex += Time.deltaTime;
            isMoving = true;
        }
        else 
        {
            accelerationCurveIndex = 0;
            isMoving = false;
        }
    }


    void RotatePlayer()
    {
        float angle = Mathf.Atan2(playerDir.x, playerDir.z) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,angle,0), rotationSpeed);
        
    }
}
