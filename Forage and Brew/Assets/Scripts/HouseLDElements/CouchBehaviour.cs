using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CouchBehaviour : MonoBehaviour, ICinematicInteraction
{
    public Animator animator;
    public GameObject localCanvas;
    private bool _usingCouch;
    public float afkTime;
    private float _afkTimer;
    public CameraPreset sitCam;
    public float sitCamTransitionTime;
    public CameraPreset standCam;
    public float standCamTransitionTime;

    public Transform locationToWalk;

    public static readonly UnityEvent OnCouchExitEvent = new();
    
    private static readonly int DoSitAfk = Animator.StringToHash("DoSitAFK");
    private static readonly int DoStand = Animator.StringToHash("DoStand");
    private static readonly int DoSit = Animator.StringToHash("DoSit");


    private void Start()
    {
        OnCouchExitEvent.AddListener(ReceiveExitCouchEvent);
    }

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
        CharacterInteractController.Instance.CurrentNearCinematicInteraction = null;
        localCanvas.SetActive(true);
    }

    
    public void StartInteraction()
    {
        _usingCouch = !_usingCouch;
        CharacterInputManager.Instance.DisableInputs();
        if (_usingCouch)
        {
            CharacterMovementController.Instance.TriggerWalkTransition(locationToWalk.position);
            localCanvas.SetActive(false);
            StartCoroutine(ToSitCam());
        }
        else
        {
            animator.SetTrigger(DoStand);
        }
    }

    private IEnumerator ToSitCam()
    {
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(sitCam, sitCamTransitionTime);
        yield return new WaitForSeconds(sitCamTransitionTime);
        SitOnCouch();
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
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(standCam, standCamTransitionTime);
        yield return new WaitForSeconds(standCamTransitionTime);
        
        localCanvas.SetActive(true);
        CharacterInputManager.Instance.EnableInputs();
    }
}
