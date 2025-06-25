using UnityEngine;

public class HouseCameraBehavior : SimpleCameraBehavior
{
    public HouseCameraSettingsBehavior mainCameraPreset;
    public HouseCameraSettingsBehavior[] allHouseCameraSettings;

    public float[] cameraSettingsWeights;

    private float totalWeight;
    
    private int weightDivider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private HouseCameraSettingsBehavior temp;
    // Update is called once per frame
    public override void FixedUpdate()
    {

        transform.parent.position = Vector3.Lerp(transform.parent.position, player.position + cameraOffset, positionLerp);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraRotation), rotationLerp);
        
        transform.localPosition =
            Vector3.Lerp(transform.localPosition, -transform.forward * distanceFromPlayer, positionLerp);
    }

    public override void Update()
    {
        temp = ClosestCameraSettings();
        if (temp != null)
        {
            if (temp != mainCameraPreset)
            {
                mainCameraPreset = ClosestCameraSettings();
            }
        }

        totalWeight = 0;
        weightDivider = 0;
        cameraOffset = Vector3.zero;
        cameraRotation = Vector3.zero;
        distanceFromPlayer = 0;
        if (Vector3.Distance(mainCameraPreset.transform.position,player.position) < mainCameraPreset.settings.triggerDistance)
        {
            for (int i = 0; i < allHouseCameraSettings.Length; i++)
            {
                CalculateWeight(i);

            }

            for (int i = 0; i < allHouseCameraSettings.Length; i++)
            {
                if (totalWeight > 0.1f)
                {
                    ApplyWeightedSettings(i);
                    continue;
                }
                break;
            }
        }
        else
        {
            ApplySettings();
        }
    }

    HouseCameraSettingsBehavior ClosestCameraSettings()
    {
        HouseCameraSettingsBehavior closest = null;
        for (int i = 0; i < allHouseCameraSettings.Length; i++)
        {
            if (Vector3.Distance(allHouseCameraSettings[i].transform.position, player.position) > allHouseCameraSettings[i].settings.triggerDistance)
            {
                continue;
            }
            if (closest == null)
            {
                closest = allHouseCameraSettings[i];
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
        weightDivider++;
    }

    void ApplyWeightedSettings(int i)
    {
        cameraOffset += allHouseCameraSettings[i].settings.cameraPreset.cameraOffset * cameraSettingsWeights[i] / totalWeight;
        distanceFromPlayer += allHouseCameraSettings[i].settings.cameraPreset.distanceFromPlayer * cameraSettingsWeights[i] / totalWeight;
        cameraRotation += allHouseCameraSettings[i].settings.cameraPreset.cameraRotation * cameraSettingsWeights[i] / totalWeight;
    }

    void ApplySettings()
    {
        cameraOffset = mainCameraPreset.settings.cameraPreset.cameraOffset;
        distanceFromPlayer = mainCameraPreset.settings.cameraPreset.distanceFromPlayer;
        cameraRotation = mainCameraPreset.settings.cameraPreset.cameraRotation;
    }
}
