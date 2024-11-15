using System;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    private Vector3 playerDir;
    public Rigidbody rb;
    public float speed;
    [Range(0,1)]public float rotationSpeed = 0.1f;
    public float maxAngle;
    public AnimationCurve accelerationCurve;
    public float accelerationCurveIndex;
    [Range(1,10)]public float brakeIntensity = 1;

    public bool isGrounded;

    public bool isMoving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        isGrounded = GroundCheck(out RaycastHit hit);
        angledVelocity = playerDir;
        if (isGrounded)
        {
            if (Physics.Raycast(transform.position , transform.forward, out RaycastHit hitForward, 0.7f, LayerMask.GetMask("Default")))
            {
                angle = Vector3.Angle(hitForward.normal, transform.forward) - 90;
            }
            else if (Physics.Raycast(transform.position , -transform.forward, out RaycastHit hitBack, 1f, LayerMask.GetMask("Default")))
            {
                angle = Vector3.Angle(hitBack.normal, transform.forward) - 90;
            }

            if (Mathf.Abs(angle) < maxAngle)
            {
                angledVelocity = Quaternion.AngleAxis(-angle,transform.right) * angledVelocity;
                angledVelocity *= (speed * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);
                rb.linearVelocity = angledVelocity + Vector3.down * 9.81f * Time.deltaTime;
            }
            else
            {
                Debug.Log("Too steep");
                angledVelocity = playerDir * (speed * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);

                rb.linearVelocity = new Vector3(angledVelocity.x, rb.linearVelocity.y, angledVelocity.z);
            }
            
            Debug.Log(angle);
            Debug.DrawRay(transform.position,angledVelocity* 5,Color.blue ,0);

        }
        else
        {
            angledVelocity = playerDir * (speed * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude * Time.deltaTime);

            rb.linearVelocity = new Vector3(rb.linearVelocity.x + angledVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z + angledVelocity.z);
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

    bool GroundCheck(out RaycastHit hit)
    {
        bool check = Physics.Raycast(transform.position + Vector3.up * 0.6f, Vector3.down, out hit, 0.7f, LayerMask.GetMask("Default"));
        
        return check;
    }
    void FixedUpdate()
    {
        if (isMoving)
            RotatePlayer();
    }


    void RotatePlayer()
    {
        float angle = Mathf.Atan2(playerDir.x, playerDir.z) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,angle,0), rotationSpeed);
        
    }
}
