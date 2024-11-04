using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    private Vector3 playerDir;
    public Rigidbody rb;
    public float speed;
    [Range(0,1)]public float rotationSpeed = 0.1f;
    public AnimationCurve accelerationCurve;
    public float accelerationCurveIndex;
    [Range(1,10)]public float brakeIntensity = 1;

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
    
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = playerDir * (speed * accelerationCurve.Evaluate(accelerationCurveIndex) * playerDir.magnitude);

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
