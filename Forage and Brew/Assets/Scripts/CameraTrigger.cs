using System;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public CameraPreset camSettings;

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
        Debug.Log("Changing Cam Settings to " + camSettings.name);
        CameraController.instance.ApplyScriptableCamSettings(camSettings, transitionTime);
    }
}
