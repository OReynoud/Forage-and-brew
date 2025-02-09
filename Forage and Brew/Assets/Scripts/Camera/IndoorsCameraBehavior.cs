using UnityEngine;

public class IndoorsCameraBehavior : Singleton<IndoorsCameraBehavior>
{
    [HideInInspector] public Camera cam;
    public Transform player;
    private CharacterMovementController movement;
    public bool posClampX;
    public bool posClampY;
    public bool posClampZ;
    public bool rotationClampX;
    public bool rotationClampY;
    public bool rotationClampZ;
    
    
    public override void Awake()
    {
        base.Awake();

        cam = Camera.main;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
