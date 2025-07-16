using System;
using System.Collections;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ObjectToSitBehaviour : MonoBehaviour, ICinematicInteraction
{
    public Animator animator;
    public GameObject localCanvas;
    private bool _usingCouch;
    public float afkTime;
    private float _afkTimer;

    public bool overrideCam;

    [ShowIf("overrideCam")] public CameraPreset sitCam;
    [ShowIf("overrideCam")] public float sitCamTransitionTime;
    [ShowIf("overrideCam")] public CameraPreset standCam;
    [ShowIf("overrideCam")] public float standCamTransitionTime;
    [Space]
    public bool specificSitLocation;
    [ShowIf("specificSitLocation")] public Transform locationToWalk;
    [HideIf("specificSitLocation")] public float distanceToSit;


    public static readonly UnityEvent OnCouchExitEvent = new();
    
    private static readonly int DoSitAfk = Animator.StringToHash("DoSitAFK");
    private static readonly int DoStand = Animator.StringToHash("DoStand");
    private static readonly int DoSit = Animator.StringToHash("DoSit");

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (specificSitLocation) return;
        
        Handles.DrawWireDisc(transform.position,Vector3.up, distanceToSit);
    }
#endif



    private void Update()
    {
        if (!_usingCouch)return;
        _afkTimer += Time.deltaTime;
        if (_afkTimer > afkTime)
        {
            animator.SetTrigger(DoSitAfk);
            _afkTimer = -afkTime;
        }

    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = this;
        localCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if ((ICinematicInteraction)this == CharacterInteractController.Instance.CurrentNearCinematicInteraction)
        {
            CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        }
        localCanvas.SetActive(false);
    }

    
    public void StartInteraction()
    {
        _usingCouch = !_usingCouch;
        CharacterInputManager.Instance.DisableInputs();
        if (_usingCouch)
        {
            if (!specificSitLocation)
            {
                Vector3 posToLook = new Vector3(transform.position.x,
                    CharacterAnimManager.instance.transform.position.y, transform.position.z);
                CharacterAnimManager.instance.transform.LookAt(posToLook);
                CharacterMovementController.Instance.TriggerWalkTransition(posToLook - CharacterAnimManager.instance.transform.forward * distanceToSit);
            }
            else
            {
                CharacterMovementController.Instance.TriggerWalkTransition(locationToWalk.position);
            }
            localCanvas.SetActive(false);
            StartCoroutine(ToSitCam());
        }
        else
        {
            animator.SetTrigger(DoStand);
        }
    }

    public void CancelCouch()
    {
        if (!_usingCouch)
            return;
        _usingCouch = false;
        animator.SetTrigger(DoStand);
    }

    private IEnumerator ToSitCam()
    {
        if (overrideCam)
        {
            
            HouseCameraBehavior.overrideCameraLerp = true;
            SimpleCameraBehavior.instance.ApplyScriptableCamSettings(sitCam, sitCamTransitionTime);
        }
        yield return new WaitForSeconds(sitCamTransitionTime);
        SitOnCouch();
        
        OnCouchExitEvent.AddListener(ReceiveExitCouchEvent);
    }
    
    private void SitOnCouch()
    {
        CharacterAnimManager.instance.transform.LookAt(new Vector3(transform.position.x,CharacterAnimManager.instance.transform.position.y,transform.position.z));
        CharacterAnimManager.instance.animator.SetTrigger(DoSit);
        CharacterAnimManager.instance.UseCouch();
    }

    private void ReceiveExitCouchEvent()
    {
        StartCoroutine(ToStandCam());
    }

    private IEnumerator ToStandCam()
    {
        if (overrideCam)
        {
            SimpleCameraBehavior.instance.ApplyScriptableCamSettings(standCam, standCamTransitionTime);
        }
        yield return new WaitForSeconds(standCamTransitionTime);
        Debug.Log("Stand");
        localCanvas.SetActive(true);
        HouseCameraBehavior.overrideCameraLerp = false;
        CharacterInputManager.Instance.EnableInputs();
        
        OnCouchExitEvent.RemoveListener(ReceiveExitCouchEvent);
    }
}
