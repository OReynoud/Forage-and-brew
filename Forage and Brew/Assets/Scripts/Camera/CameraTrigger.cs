using System;
using NaughtyAttributes;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [Expandable] public CameraPreset camSettings;

    public float transitionTime;
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
        CameraController.instance.ApplyScriptableCamSettings(camSettings, transitionTime);
    }
}
