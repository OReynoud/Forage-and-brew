using System;
using NaughtyAttributes;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [Expandable] public CameraPreset camSettings;

    [HideIf("instantTransition")] public float transitionTime;

    public bool instantTransition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (instantTransition)
        {
            CameraController.instance.ApplyScriptableCamSettings(camSettings, 0);
        }
        else
        {
            CameraController.instance.ApplyScriptableCamSettings(camSettings, transitionTime);
        }
        
    }
}
