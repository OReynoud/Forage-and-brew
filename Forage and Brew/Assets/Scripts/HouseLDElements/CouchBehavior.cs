using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CouchBehavior : MonoBehaviour, ICinematicInteraction
{
    public Animator animator;
    public GameObject localCanvas;
    private bool usingCouch;
    public float afkTime;
    private float afkTimer;
    public CameraPreset sitCam;
    public float sitCamTransitionTime;
    public CameraPreset standCam;
    public float standCamTransitionTime;

    public Transform locationToWalk;

    public static UnityEvent onCouchExitEvent = new ();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onCouchExitEvent.AddListener(ReceiveExitCouchEvent);
    }

    // Update is called once per frame
    void Update()
    {
        if (!usingCouch)return;
        afkTimer += Time.deltaTime;
        if (afkTimer > afkTime)
        {
            animator.SetTrigger("DoSitAFK");
            afkTimer = -afkTime;
        }

    }    
    private void OnTriggerEnter(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = this;
        localCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        localCanvas.SetActive(true);
    }

    public void StartInteraction()
    {
        usingCouch = !usingCouch;
        CharacterInputManager.Instance.DisableInputs();
        if (usingCouch)
        {
            CharacterMovementController.Instance.TriggerWalkTransition(locationToWalk.position);
            localCanvas.SetActive(false);
            StartCoroutine(ToSitCam());
        }
        else
        {
            animator.SetTrigger("DoStand");
        }
    }

    IEnumerator ToSitCam()
    {
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(sitCam, sitCamTransitionTime);
        yield return new WaitForSeconds(sitCamTransitionTime);
        SitOnCouch();
    }
    private void SitOnCouch()
    {
        CharacterAnimManager.instance.transform.LookAt(new Vector3(transform.position.x,CharacterAnimManager.instance.transform.position.y,transform.position.z));
        CharacterAnimManager.instance.animator.SetTrigger("DoSit");
        CharacterAnimManager.instance.UseCouch();
    }

    void ReceiveExitCouchEvent()
    {
        StartCoroutine(ToStandCam());
    }

    IEnumerator ToStandCam()
    {
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(standCam, standCamTransitionTime);
        yield return new WaitForSeconds(standCamTransitionTime);
        
        localCanvas.SetActive(true);
        CharacterInputManager.Instance.EnableInputs();
    }
}
