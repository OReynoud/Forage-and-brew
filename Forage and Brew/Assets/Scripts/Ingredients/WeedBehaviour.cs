using UnityEngine;
using UnityEngine.UI;

public class WeedBehaviour : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject weedingInputLeftGameObject;
    [SerializeField] private GameObject weedingReleaseLeftGameObject;
    [SerializeField] private GameObject weedingGaugeLeftGameObject;
    [SerializeField] private Slider weedingGaugeLeftSlider;
    [SerializeField] private GameObject weedingInputRightGameObject;
    [SerializeField] private GameObject weedingReleaseRightGameObject;
    [SerializeField] private GameObject weedingGaugeRightGameObject;
    [SerializeField] private Slider weedingGaugeRightSlider;
    
    
    public void EnableWeed()
    {
        gameObject.SetActive(true);
    }
    
    public void DisableWeed()
    {
        gameObject.SetActive(false);
    }


    public void EnableCollect(bool isUiRight)
    {
        if (isUiRight)
        {
            weedingInputRightGameObject.SetActive(true);
            weedingGaugeRightGameObject.SetActive(true);
            weedingGaugeRightSlider.value = 0f;
        }
        else
        {
            weedingInputLeftGameObject.SetActive(true);
            weedingGaugeLeftGameObject.SetActive(true);
            weedingGaugeLeftSlider.value = 0f;
        }
    }

    public void DisableUI()
    {
        weedingInputLeftGameObject.SetActive(false);
        weedingReleaseLeftGameObject.SetActive(false);
        weedingGaugeLeftGameObject.SetActive(false);
        weedingInputRightGameObject.SetActive(false);
        weedingReleaseRightGameObject.SetActive(false);
        weedingGaugeRightGameObject.SetActive(false);
    }
    
    
    public void SetWeedingValue(float value, bool isUiRight)
    {
        if (isUiRight)
        {
            weedingGaugeRightSlider.value = value;
        }
        else
        {
            weedingGaugeLeftSlider.value = value;
        }
    }
    
    public void ReleaseWeeding(bool isUiRight)
    {
        if (isUiRight)
        {
            weedingReleaseRightGameObject.SetActive(true);
        }
        else
        {
            weedingReleaseLeftGameObject.SetActive(true);
        }
    }
}
