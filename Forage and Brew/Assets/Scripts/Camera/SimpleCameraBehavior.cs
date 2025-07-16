using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class SimpleCameraBehavior : Singleton<SimpleCameraBehavior>
{
    [HideInInspector] public Camera cam;
    public Transform player;
    protected CharacterMovementController movement;

    public Camera worldUiCam;
    public Camera overlayUiCam;

    [Foldout("Calculated at Start")] public float targetFocalLength;
    [Foldout("Calculated at Start")] public Vector3 cameraRotation;

    [Foldout("Adjustable Variables")] public AnimationCurve cameraTransitionCurve;
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
    public CameraPreset previousCamSettings;
    public CameraPreset TargetCamSettings;

    [BoxGroup("References")] public CameraPreset codexCamSettings;
    [BoxGroup("References")] public AnimationCurve alternateTransitionCurve;
    public float codexEnterTime;
    public float codexExitTime;

    public float transitionTime = 0.001f;
    [SerializeField] [ReadOnly] private float counter;
    [SerializeField] [ReadOnly] private bool applyXYClamping;
    [SerializeField] [ReadOnly] private bool applyZClamping;

    private Dictionary<string, bool> UsingCamPosToBlendClamps = new Dictionary<string, bool>();
    protected bool localCodexShow;


    [SerializeField] private Vector3 transitionStartPos;
    [SerializeField] private Vector3 transitionStartZDist;
    [SerializeField] private Quaternion transitionStartRot;


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

        SetupBlendClampBools();
    }


    void SetupBlendClampBools()
    {
        // UsingCamPosToBlendClamps.Add("XMax", false);
        // UsingCamPosToBlendClamps.Add("YMax", false);
        // UsingCamPosToBlendClamps.Add("ZMax", false);
        // UsingCamPosToBlendClamps.Add("XMin", false);
        // UsingCamPosToBlendClamps.Add("YMin", false);
        // UsingCamPosToBlendClamps.Add("ZMin", false);
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
        worldUiCam.focalLength = scriptableCamSettings.targetFocalLength;
        overlayUiCam.focalLength = scriptableCamSettings.targetFocalLength;
        targetFocalLength = scriptableCamSettings.targetFocalLength;
        cameraRotation = scriptableCamSettings.cameraRotation;
        cameraOffset = scriptableCamSettings.cameraOffset;

        distanceFromPlayer = scriptableCamSettings.distanceFromPlayer;
        positionLerp = scriptableCamSettings.positionLerp;
        rotationLerp = scriptableCamSettings.rotationLerp;
        focalLerp = scriptableCamSettings.focalLerp;
        cameraTransitionCurve = scriptableCamSettings.transitionCurve;


        transform.parent.position = player.position + cameraOffset;
        transform.localPosition = -transform.forward * distanceFromPlayer;
        transform.rotation = Quaternion.Euler(scriptableCamSettings.cameraRotation);
        
        transitionStartPos = transform.parent.position;
        transitionStartZDist = transform.localPosition;
        transitionStartRot = transform.localRotation;
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
        
        if (TargetCamSettings == preset) return;
        
        previousCamSettings = TargetCamSettings;
        TargetCamSettings = preset;

        
        if (counter < transitionTime)
        {
            cameraTransitionCurve = alternateTransitionCurve;
        }
        else
        {
            cameraTransitionCurve = TargetCamSettings.transitionCurve;
        }

        counter = 0;
        transitionTime = TransitionTime == 0 ? 0.001f : TransitionTime;

        transitionStartPos = transform.parent.position;
        transitionStartZDist = transform.localPosition;
        transitionStartRot = transform.localRotation;
        

        applyXYClamping = Mathf.Abs(TargetCamSettings.posMaxClamp.x + TargetCamSettings.posMaxClamp.y) >= 1 ||
                          Mathf.Abs(TargetCamSettings.posMinClamp.x + TargetCamSettings.posMinClamp.y) >= 1;
        applyZClamping = Mathf.Abs(TargetCamSettings.posMaxClamp.z) >= 1 ||
                         Mathf.Abs(TargetCamSettings.posMinClamp.z) >= 1;
        
        //Debug.Log("Cam Settings: " + preset.name);
        // UsingCamPosToBlendClamps["XMax"] = previousCamSettings.posMaxClamp.x != TargetCamSettings.posMaxClamp.x;
        // UsingCamPosToBlendClamps["YMax"] = previousCamSettings.posMaxClamp.y != TargetCamSettings.posMaxClamp.y;
        // UsingCamPosToBlendClamps["ZMax"] = previousCamSettings.posMaxClamp.z != TargetCamSettings.posMaxClamp.z;
        // UsingCamPosToBlendClamps["XMin"] = previousCamSettings.posMinClamp.x != TargetCamSettings.posMinClamp.x;
        // UsingCamPosToBlendClamps["YMin"] = previousCamSettings.posMinClamp.y != TargetCamSettings.posMinClamp.y;
        // UsingCamPosToBlendClamps["ZMin"] = previousCamSettings.posMinClamp.z != TargetCamSettings.posMinClamp.z;
    }

    private void ApplyScriptableCamSettings(float TransitionTime)
    {
        transitionStartPos = transform.parent.position;
        transitionStartZDist = transform.localPosition;
        transitionStartRot = transform.localRotation;
        
        cameraTransitionCurve = codexCamSettings.transitionCurve;
        transitionTime = TransitionTime == 0 ? 0.001f : TransitionTime;
        counter = 0;
    }

    private void UpdateCamWithCodex(bool state)
    {
        if (state)
        {
            ApplyScriptableCamSettings(codexEnterTime);
            localCodexShow = true;
        }
        else
        {
            ApplyScriptableCamSettings(codexExitTime);
            localCodexShow = false;
        }
    }

    public virtual void InstantCamUpdate(CameraPreset preset)
    {
        previousCamSettings = TargetCamSettings;
        TargetCamSettings = preset;
        
        transitionTime = 0.001f;
        counter = 0.1f;
        
        cameraOffset = TargetCamSettings.cameraOffset;
        distanceFromPlayer = TargetCamSettings.distanceFromPlayer;
        cameraRotation = TargetCamSettings.cameraRotation;
        targetFocalLength = TargetCamSettings.targetFocalLength;        
        
        
        transform.parent.position = player.position + TargetCamSettings.cameraOffset;
        transform.localPosition = -transform.forward * TargetCamSettings.distanceFromPlayer;
        transform.parent.position = ClampCamPos(transform.parent.position);
        transform.localRotation = Quaternion.Euler(TargetCamSettings.cameraRotation);
        cam.focalLength = TargetCamSettings.targetFocalLength;
        worldUiCam.focalLength = TargetCamSettings.targetFocalLength;
        overlayUiCam.focalLength = TargetCamSettings.targetFocalLength;

        transitionStartPos = transform.parent.position;
        transitionStartZDist = transform.localPosition;
        transitionStartRot = transform.localRotation;

//        Debug.Log("Instant Cam Settings: " + TargetCamSettings.name);
    }

    private void Start()
    {
        movement = CharacterMovementController.Instance;
        CharacterInputManager.Instance.OnCodexUse.AddListener(UpdateCamWithCodex);
    }


    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        posMaxClamp = TargetCamSettings.posMaxClamp;
        posMinClamp = TargetCamSettings.posMinClamp;

        if (counter < transitionTime)
        {

        }
        else
        {
             transform.parent.position = Vector3.Lerp(transform.parent.position, player.position + cameraOffset,
                 movement.isRunning ? positionLerp * 2.5f : positionLerp);
             transform.localPosition =
                 Vector3.Lerp(transform.localPosition, -transform.forward * distanceFromPlayer, positionLerp);
             transform.parent.position = ClampCamPos(transform.parent.position);
            // transform.localRotation =
            //     Quaternion.Lerp(transform.localRotation, Quaternion.Euler(cameraRotation), rotationLerp);
            //
            // cam.focalLength = Mathf.Lerp(cam.focalLength, targetFocalLength, focalLerp);
            // overlayUiCam.focalLength = Mathf.Lerp(overlayUiCam.focalLength, targetFocalLength, focalLerp);
        }



    }

    private Vector3 ClampCamPos(Vector3 position)
    {
        if (!applyXYClamping || GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge)
            return position;

        if (position.x > posMaxClamp.x)
        {
            position = new Vector3(posMaxClamp.x, position.y, position.z);
        }

        if (position.x < posMinClamp.x)
        {
            position =
                new Vector3(posMinClamp.x, position.y, position.z);
        }

        if (position.z > posMaxClamp.y)
        {
            position =
                new Vector3(position.x, position.y, posMaxClamp.y);
        }

        if (position.z < posMinClamp.y)
        {
            position =
                new Vector3(position.x, position.y, posMinClamp.y);
        }

        if (!applyZClamping)
            return position;

        if (position.y > posMaxClamp.z)
        {
            position =
                new Vector3(position.x, posMaxClamp.z, position.z);
        }

        if (position.y < posMinClamp.z)
        {
            position =
                new Vector3(position.x, posMinClamp.z, position.z);
        }

        return position;
    }

    public virtual void Update()
    {
        if (counter < transitionTime && transitionTime != 0)
        {
            counter += Time.deltaTime;
            
            Vector3 aimedCamPos = player.position + cameraOffset;
            aimedCamPos = ClampCamPos(aimedCamPos);
            transform.parent.position = Vector3.Lerp(transitionStartPos, aimedCamPos, cameraTransitionCurve.Evaluate(counter / transitionTime));
            transform.localPosition =
                Vector3.Lerp(transitionStartZDist, -transform.forward * distanceFromPlayer, cameraTransitionCurve.Evaluate(counter / transitionTime));
            transform.localRotation =
                Quaternion.Lerp(transitionStartRot, Quaternion.Euler(cameraRotation), cameraTransitionCurve.Evaluate(counter / transitionTime));
        }


        if (localCodexShow)
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
    }
}