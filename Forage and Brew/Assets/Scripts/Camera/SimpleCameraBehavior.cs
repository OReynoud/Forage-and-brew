using NaughtyAttributes;
using UnityEngine;

public class SimpleCameraBehavior : Singleton<SimpleCameraBehavior>
{
    [HideInInspector] public Camera cam;
    public Transform player;
    protected CharacterMovementController movement;

    public Camera overlayUiCam;
    
    [Foldout("Calculated at Start")] public float targetFocalLength;
    [Foldout("Calculated at Start")] public Vector3 cameraRotation;

    [Foldout("Adjustable Variables")] public Vector3 cameraOffset;
    [Foldout("Adjustable Variables")] public float distanceFromPlayer;

    [Foldout("Adjustable Variables")] [Range(0, 1)]
    public float positionLerp = 0.07f;

    [Foldout("Adjustable Variables")] [Range(0, 1)]
    public float rotationLerp = 0.02f;

    [Foldout("Adjustable Variables")] [Range(0, 1)]
    public float focalLerp = 0.07f;

    [Foldout("Adjustable Variables")] public Vector3 posMaxClamp;
    [Foldout("Adjustable Variables")] public Vector3 posMinClamp;

    [BoxGroup] [Expandable] public CameraPreset scriptableCamSettings;
    private CameraPreset previousCamSettings;
    public CameraPreset TargetCamSettings { get; private set; }

    [BoxGroup("References")] public CameraPreset codexCamSettings;
    public float codexEnterTime;
    public float codexExitTime;

    private float transitionTime = 0.001f;
    [SerializeField] [ReadOnly] private float counter;
    [SerializeField] [ReadOnly] private bool applyXYClamping;
    [SerializeField] [ReadOnly] private bool applyZClamping;
    [SerializeField] [ReadOnly] private bool fixedPos;
    [SerializeField] [ReadOnly] private bool fixedRotation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            transform.parent.position = player.position + cameraOffset;
            transform.localPosition = -transform.forward * distanceFromPlayer;
            transform.LookAt(transform.parent);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.parent.position, 0.4f);
    }


    public override void Awake()
    {
        base.Awake();
        applyXYClamping = Mathf.Abs(posMaxClamp.x + posMaxClamp.y) >= 1 ||
                          Mathf.Abs(posMinClamp.x + posMinClamp.y) >= 1;
        applyZClamping = Mathf.Abs(posMaxClamp.z) >= 1 || Mathf.Abs(posMinClamp.z) >= 1;

        cam = Camera.main;
        targetFocalLength = cam.focalLength;
        cameraRotation = transform.localRotation.eulerAngles;
        previousCamSettings = scriptableCamSettings;
        TargetCamSettings = scriptableCamSettings;


        if (scriptableCamSettings == null)
        {
            Debug.LogError("No Scriptable Cam Settings found, camera might work unpredictably");
        }
        else
        {
            ApplyScriptableCamSettings();
            //Debug.Log(transform.localRotation.eulerAngles);
        }
    }


    [Button]
    public void ApplyScriptableCamSettings()
    {
        if (scriptableCamSettings == null)
        {
            Debug.LogError("No Scriptable Cam Settings found!");
            return;
        }

        cam.focalLength = scriptableCamSettings.targetFocalLength;
        overlayUiCam.focalLength = scriptableCamSettings.targetFocalLength;
        targetFocalLength = scriptableCamSettings.targetFocalLength;
        transform.rotation = Quaternion.Euler(scriptableCamSettings.cameraRotation);
        cameraRotation = scriptableCamSettings.cameraRotation;
        cameraOffset = scriptableCamSettings.cameraOffset;

        distanceFromPlayer = scriptableCamSettings.distanceFromPlayer;
        positionLerp = scriptableCamSettings.positionLerp;
        rotationLerp = scriptableCamSettings.rotationLerp;
        focalLerp = scriptableCamSettings.focalLerp;


        transform.localPosition = -transform.forward * distanceFromPlayer;
    }

    [Button]
    public void SaveCurrentSettingsToCameraPreset()
    {
        if (scriptableCamSettings == null)
        {
            Debug.LogError("No Scriptable Cam Settings found!");
            return;
        }

        scriptableCamSettings.targetFocalLength = cam.focalLength;
        scriptableCamSettings.cameraRotation = transform.rotation.eulerAngles;
        scriptableCamSettings.cameraOffset = cameraOffset;

        scriptableCamSettings.distanceFromPlayer = distanceFromPlayer;
        scriptableCamSettings.positionLerp = positionLerp;
        scriptableCamSettings.rotationLerp = rotationLerp;
        scriptableCamSettings.focalLerp = focalLerp;
    }

    public virtual void ApplyScriptableCamSettings(CameraPreset preset, float TransitionTime)
    {
        previousCamSettings = TargetCamSettings;
        TargetCamSettings = preset;
        
        counter = 0;
        

        transitionTime = TransitionTime == 0 ? 0.001f : TransitionTime;
        
        
        fixedPos = TargetCamSettings.isFixedCameraPos;
        fixedRotation = TargetCamSettings.isFixedCameraRotation;

        
        applyXYClamping = Mathf.Abs(TargetCamSettings.posMaxClamp.x + TargetCamSettings.posMaxClamp.y) >= 1 ||
                          Mathf.Abs(TargetCamSettings.posMinClamp.x + TargetCamSettings.posMinClamp.y) >= 1;
        applyZClamping = Mathf.Abs(TargetCamSettings.posMaxClamp.z) >= 1 ||
                         Mathf.Abs(TargetCamSettings.posMinClamp.z) >= 1;

        //Debug.Log("Cam Settings: " + preset.name);
    }

    private void ApplyScriptableCamSettings(float TransitionTime)
    {
        transitionTime = TransitionTime == 0 ? 0.001f : TransitionTime;
        counter = 0;
    }

    private void UpdateCamWithCodex()
    {
        if (CharacterInputManager.Instance.showCodex)
        {
            ApplyScriptableCamSettings(codexEnterTime);
            InfoDisplayManager.instance.ShowBackground();
        }
        else
        {
            ApplyScriptableCamSettings(codexExitTime);
            InfoDisplayManager.instance.HideBackground();
        }
    }

    public virtual void InstantCamUpdate()
    {
        transform.parent.position = player.position + TargetCamSettings.cameraOffset;
        transform.localRotation = Quaternion.Euler(TargetCamSettings.cameraRotation);
        transform.localPosition = -transform.forward * TargetCamSettings.distanceFromPlayer;
        cam.focalLength = TargetCamSettings.targetFocalLength;
        overlayUiCam.focalLength = TargetCamSettings.targetFocalLength;
        ClampCamPos();
        //Debug.Log("Instant Cam Settings: " + TargetCamSettings.name);
    }

    private void Start()
    {
        movement = CharacterMovementController.Instance;
        CharacterInputManager.Instance.OnCodexUse.AddListener(UpdateCamWithCodex);
    }


    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (!fixedPos)
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, player.position + cameraOffset,
                movement.isRunning ? positionLerp * 2.5f : positionLerp);
            transform.localPosition =
                Vector3.Lerp(transform.localPosition, -transform.forward * distanceFromPlayer, positionLerp);
            ClampCamPos();
        }
        else
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position,TargetCamSettings.fixedCameraPos,positionLerp * (counter / transitionTime));            
            transform.localPosition =
                Vector3.Lerp(transform.localPosition, Vector3.zero, positionLerp * (counter / transitionTime));
        }

        if (fixedPos && !fixedRotation)
        {
            Quaternion rotation = Quaternion.LookRotation((player.position - transform.position) + Vector3.up, Vector3.up);
            transform.localRotation =
                Quaternion.Lerp(transform.localRotation, rotation, rotationLerp * (counter / transitionTime));
            
        }
        else
        {
            cam.focalLength = Mathf.Lerp(cam.focalLength, targetFocalLength, focalLerp);
            overlayUiCam.focalLength = Mathf.Lerp(overlayUiCam.focalLength, targetFocalLength, focalLerp);
            transform.localRotation =
                Quaternion.Lerp(transform.localRotation, Quaternion.Euler(cameraRotation), rotationLerp);
        }

    }

    private void ClampCamPos()
    {
        if (!applyXYClamping || GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge)
            return;

        if (transform.parent.position.x > posMaxClamp.x)
        {
            transform.parent.position =
                new Vector3(posMaxClamp.x, transform.parent.position.y, transform.parent.position.z);
        }

        if (transform.parent.position.x < posMinClamp.x)
        {
            transform.parent.position =
                new Vector3(posMinClamp.x, transform.parent.position.y, transform.parent.position.z);
        }

        if (transform.parent.position.z > posMaxClamp.y)
        {
            transform.parent.position =
                new Vector3(transform.parent.position.x, transform.parent.position.y, posMaxClamp.y);
        }

        if (transform.parent.position.z < posMinClamp.y)
        {
            transform.parent.position =
                new Vector3(transform.parent.position.x, transform.parent.position.y, posMinClamp.y);
        }

        if (!applyZClamping)
            return;

        if (transform.parent.position.y > posMaxClamp.z)
        {
            transform.parent.position =
                new Vector3(transform.parent.position.x, posMaxClamp.z, transform.parent.position.z);
        }

        if (transform.parent.position.y < posMinClamp.z)
        {
            transform.parent.position =
                new Vector3(transform.parent.position.x, posMinClamp.z, transform.parent.position.z);
        }
    }

    public virtual void Update()
    {
        if (counter < transitionTime)
        {
            counter += Time.deltaTime;
        }


        if (CharacterInputManager.Instance.showCodex)
        {
            targetFocalLength = Mathf.Lerp(TargetCamSettings.targetFocalLength,
                TargetCamSettings.targetFocalLength - codexCamSettings.targetFocalLength, counter / transitionTime);


            distanceFromPlayer = Mathf.Lerp(TargetCamSettings.distanceFromPlayer,
                TargetCamSettings.distanceFromPlayer + codexCamSettings.distanceFromPlayer, counter / transitionTime);
        }
        else
        {
            cameraRotation = Vector3.Lerp(previousCamSettings.cameraRotation, TargetCamSettings.cameraRotation,
                counter / transitionTime);
            cameraOffset = Vector3.Lerp(previousCamSettings.cameraOffset, TargetCamSettings.cameraOffset,
                counter / transitionTime);

            targetFocalLength = Mathf.Lerp(previousCamSettings.targetFocalLength, TargetCamSettings.targetFocalLength,
                counter / transitionTime);
            distanceFromPlayer = Mathf.Lerp(previousCamSettings.distanceFromPlayer,
                TargetCamSettings.distanceFromPlayer, counter / transitionTime);
        }

        positionLerp = Mathf.Lerp(previousCamSettings.positionLerp, TargetCamSettings.positionLerp,
            counter / transitionTime);
        rotationLerp = Mathf.Lerp(previousCamSettings.rotationLerp, TargetCamSettings.rotationLerp,
            counter / transitionTime);
        focalLerp = Mathf.Lerp(previousCamSettings.focalLerp, TargetCamSettings.focalLerp, counter / transitionTime);

        posMaxClamp = Vector3.Lerp(previousCamSettings.posMaxClamp, TargetCamSettings.posMaxClamp,
            counter / transitionTime);
        posMinClamp = Vector3.Lerp(previousCamSettings.posMinClamp, TargetCamSettings.posMinClamp,
            counter / transitionTime);
    }
}