using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HouseCameraBehavior : SimpleCameraBehavior
{
    public HouseCameraSettingsBehavior mainCameraPreset;
    public List<HouseCameraSettingsBehavior> allHouseCameraSettings = new();
    public static UnityEvent<bool, HouseCameraSettingsBehavior> cameraTriggerBehavior = new(); 

    public float[] cameraSettingsWeights;

    private float totalWeight;

    public bool overrideCameraLerp;
    // {
    //     get => true;
    //     set
    //     {
    //         Debug.Log(value);
    //     }
    // }

    public Vector3 aimedPos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        base.Awake();
        cameraTriggerBehavior.AddListener(UpdateUsingCamerasList);
        applyXYClamping = true;
    }

    private HouseCameraSettingsBehavior temp;
    // Update is called once per frame
    public override void FixedUpdate()
    {
        //Debug.Log(mainCameraPreset);
        if (localCodexShow || overrideCameraLerp)
        {
            base.FixedUpdate();
            return;
        }

        if (allHouseCameraSettings.Count <= 1)
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, player.position + cameraOffset, positionLerp);
            transform.parent.position = ClampCamPos(transform.parent.position);
        }
        else
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, aimedPos, positionLerp * 3);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraRotation), rotationLerp);
        
        transform.localPosition =
            Vector3.Lerp(transform.localPosition, -transform.forward * distanceFromPlayer, positionLerp);
    }

    public override void Update()
    {

        if (localCodexShow || overrideCameraLerp)
        {
            base.Update();
            return;
        }
        if (allHouseCameraSettings.Count > 0)
        {
            mainCameraPreset = ClosestCameraSettings();
            previousCamSettings = mainCameraPreset.settings.cameraPreset;
            TargetCamSettings = mainCameraPreset.settings.cameraPreset;
        }

        if (!mainCameraPreset)
            return;
        
        totalWeight = 0;
        cameraOffset = Vector3.zero;
        cameraRotation = Vector3.zero;
        distanceFromPlayer = 0;
        posMinClamp = Vector3.zero;
        posMaxClamp = Vector3.zero;
        if (Vector3.Distance(mainCameraPreset.transform.position,player.position) < mainCameraPreset.settings.triggerDistance)
        {
            aimedPos = Vector3.zero;
            for (int i = 0; i < allHouseCameraSettings.Count; i++)
            {
                CalculateWeight(i);

            }

            for (int i = 0; i < allHouseCameraSettings.Count; i++)
            {
                        
                aimedPos += ClampCamPos(
                    player.position + allHouseCameraSettings[i].settings.cameraPreset.cameraOffset,
                    allHouseCameraSettings[i].settings.cameraPreset.posMaxClamp,
                    allHouseCameraSettings[i].settings.cameraPreset.posMinClamp) * cameraSettingsWeights[i] / totalWeight;
                
                if (totalWeight > 0.1f)
                {
                    ApplyWeightedSettings(i);
                    continue;
                }
                ApplySettings();
                
                break;
            }

            //aimedPos /= allHouseCameraSettings.Count;
        }
        else
        {
            ApplySettings();
        }
    }

    HouseCameraSettingsBehavior ClosestCameraSettings()
    {
        HouseCameraSettingsBehavior closest = allHouseCameraSettings[0];
        for (int i = 1; i < allHouseCameraSettings.Count; i++)
        {
            if (Vector3.Distance(allHouseCameraSettings[i].transform.position, player.position) > allHouseCameraSettings[i].settings.triggerDistance)
            {
                continue;
            }
            if (Vector3.Distance(player.position,closest.transform.position) > Vector3.Distance(player.position,allHouseCameraSettings[i].transform.position) )
            {
                closest = allHouseCameraSettings[i];
            }
        }

        return closest;
    }


    void CalculateWeight(int i)
    {
        if (Vector3.Distance(allHouseCameraSettings[i].transform.position, player.position) > allHouseCameraSettings[i].settings.triggerDistance)
        {
            cameraSettingsWeights[i] = 0;
            return;
        }

        cameraSettingsWeights[i] =
            1 - Vector3.Distance(allHouseCameraSettings[i].transform.position, player.position) / allHouseCameraSettings[i].settings.triggerDistance;
        totalWeight += cameraSettingsWeights[i];

    }

    void ApplyWeightedSettings(int i)
    {
        cameraOffset += allHouseCameraSettings[i].settings.cameraPreset.cameraOffset * cameraSettingsWeights[i] / totalWeight;
        distanceFromPlayer += allHouseCameraSettings[i].settings.cameraPreset.distanceFromPlayer * cameraSettingsWeights[i] / totalWeight;
        cameraRotation += allHouseCameraSettings[i].settings.cameraPreset.cameraRotation * cameraSettingsWeights[i] / totalWeight;
        posMaxClamp += allHouseCameraSettings[i].settings.cameraPreset.posMaxClamp * cameraSettingsWeights[i] / totalWeight;
        posMinClamp += allHouseCameraSettings[i].settings.cameraPreset.posMinClamp * cameraSettingsWeights[i] / totalWeight;



    }

    void ApplySettings()
    {
        cameraOffset = mainCameraPreset.settings.cameraPreset.cameraOffset;
        distanceFromPlayer = mainCameraPreset.settings.cameraPreset.distanceFromPlayer;
        cameraRotation = mainCameraPreset.settings.cameraPreset.cameraRotation;
        posMaxClamp = mainCameraPreset.settings.cameraPreset.posMaxClamp;
        posMinClamp = mainCameraPreset.settings.cameraPreset.posMinClamp;
    }

    void UpdateUsingCamerasList(bool doInsert, HouseCameraSettingsBehavior settingsBehavior)
    {
        if (doInsert)
        {
            allHouseCameraSettings.Add(settingsBehavior);
        }
        else
        {
            if (allHouseCameraSettings.Count == 1)
            {
                mainCameraPreset = settingsBehavior;
            }
            allHouseCameraSettings.Remove(settingsBehavior);
        }
    }

    public override void InstantCamUpdate(CameraPreset preset)
    {
        base.InstantCamUpdate(preset);
        cameraOffset = preset.cameraOffset;
        distanceFromPlayer = preset.distanceFromPlayer;
        cameraRotation = preset.cameraRotation;
        posMaxClamp = preset.posMaxClamp;
        posMinClamp = preset.posMinClamp;
        transform.parent.position = player.position + cameraOffset;
        transform.rotation = Quaternion.Euler(cameraRotation);
        transform.localPosition = -transform.forward * distanceFromPlayer;
        
        mainCameraPreset = null;
    }

    protected Vector3 ClampCamPos(Vector3 position, Vector3 MaxClamp, Vector3 MinClamp)
    {
        if (!applyXYClamping || GameDontDestroyOnLoadManager.Instance.IsInHapticChallenge)
            return position;

        if (position.x > MaxClamp.x)
        {
            position = new Vector3(MaxClamp.x, position.y, position.z);
        }

        if (position.x < MinClamp.x)
        {
            position =
                new Vector3(MinClamp.x, position.y, position.z);
        }

        if (position.z > MaxClamp.y)
        {
            position =
                new Vector3(position.x, position.y, MaxClamp.y);
        }

        if (position.z < MinClamp.y)
        {
            position =
                new Vector3(position.x, position.y, MinClamp.y);
        }
        
        return position;
    }
}
