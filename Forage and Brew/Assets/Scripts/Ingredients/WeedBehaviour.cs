using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WeedBehaviour : MonoBehaviour
{
    [Header("Young Sprout")]
    [SerializeField] private GameObject youngSproutGameObject;
    
    // TODO: Replace by animator
    [SerializeField] private float youngSproutGrowthDuration = 1f;
    [SerializeField] private AnimationCurve youngSproutGrowthCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float youngSproutStartScale = 0.1f;
    
    [Header("UI")]
    [SerializeField] private GameObject weedingInputLeftGameObject;
    [SerializeField] private GameObject weedingReleaseLeftGameObject;
    [SerializeField] private RectTransform weedingGaugeLeftLayoutRectTransform;
    [SerializeField] private List<RectTransform> weedingGaugeLeftRectTransforms;
    [SerializeField] private List<Slider> weedingGaugeLeftSliders;
    [SerializeField] private GameObject weedingInputRightGameObject;
    [SerializeField] private GameObject weedingReleaseRightGameObject;
    [SerializeField] private RectTransform weedingGaugeRightLayoutRectTransform;
    [SerializeField] private List<RectTransform> weedingGaugeRightRectTransforms;
    [SerializeField] private List<Slider> weedingGaugeRightSliders;
    private float _largeGaugeHeight;
    private float _thinGaugeHeight;
    [SerializeField] private float gaugeSizeChangeDuration = 0.5f;
    [SerializeField] private AnimationCurve gaugeSizeChangeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    private void Start()
    {
        _largeGaugeHeight = weedingGaugeLeftRectTransforms[0].sizeDelta.y;
        _thinGaugeHeight = weedingGaugeLeftRectTransforms[1].sizeDelta.y;
    }


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
            for (int i = 0; i < weedingGaugeRightSliders.Count; i++)
            {
                weedingGaugeRightRectTransforms[i].gameObject.SetActive(true);
                weedingGaugeRightSliders[i].value = 0f;
            }
        }
        else
        {
            weedingInputLeftGameObject.SetActive(true);
            for (int i = 0; i < weedingGaugeLeftSliders.Count; i++)
            {
                weedingGaugeLeftRectTransforms[i].gameObject.SetActive(true);
                weedingGaugeLeftSliders[i].value = 0f;
            }
        }
    }

    public void DisableUI()
    {
        weedingInputLeftGameObject.SetActive(false);
        weedingReleaseLeftGameObject.SetActive(false);
        foreach (RectTransform weedingGaugeLeftRectTransform in weedingGaugeLeftRectTransforms)
        {
            weedingGaugeLeftRectTransform.gameObject.SetActive(false);
        }
        
        weedingInputRightGameObject.SetActive(false);
        weedingReleaseRightGameObject.SetActive(false);
        foreach (RectTransform weedingGaugeRightRectTransform in weedingGaugeRightRectTransforms)
        {
            weedingGaugeRightRectTransform.gameObject.SetActive(false);
        }
    }
    

    public void CollectWeed()
    {
        DisableWeed();
        
        youngSproutGameObject.SetActive(true);
        
        // TODO: Replace by real animation
        youngSproutGameObject.transform.localScale = new Vector3(youngSproutStartScale, youngSproutStartScale, youngSproutStartScale);
        youngSproutGameObject.transform.DOScale(Vector3.one, youngSproutGrowthDuration).SetEase(youngSproutGrowthCurve);
    }
    
    
    public void ChangeWeedingInputIndex(int newSliderIndex, bool isUiRight)
    {
        List<int> gaugeIndices = new();

        for (int i = 0; i < weedingGaugeLeftRectTransforms.Count; i++)
        {
            (isUiRight ? weedingGaugeRightRectTransforms : weedingGaugeLeftRectTransforms)[i].DOKill(true);
            gaugeIndices.Add(i);
        }
        
        gaugeIndices.Remove(newSliderIndex);
        
        if (isUiRight)
        {
            weedingGaugeRightRectTransforms[newSliderIndex].DOSizeDelta(
                new Vector2(weedingGaugeRightRectTransforms[newSliderIndex].sizeDelta.x, _largeGaugeHeight), 
                gaugeSizeChangeDuration).SetEase(gaugeSizeChangeCurve).OnUpdate(() =>
                LayoutRebuilder.MarkLayoutForRebuild(weedingGaugeRightLayoutRectTransform));

            foreach (int index in gaugeIndices)
            {
                weedingGaugeRightRectTransforms[index].DOSizeDelta(
                    new Vector2(weedingGaugeRightRectTransforms[index].sizeDelta.x, _thinGaugeHeight),
                    gaugeSizeChangeDuration).SetEase(gaugeSizeChangeCurve);
            }
        }
        else
        {
            weedingGaugeLeftRectTransforms[newSliderIndex].DOSizeDelta(
                new Vector2(weedingGaugeLeftRectTransforms[newSliderIndex].sizeDelta.x, _largeGaugeHeight), 
                gaugeSizeChangeDuration).SetEase(gaugeSizeChangeCurve).OnUpdate(() =>
                LayoutRebuilder.MarkLayoutForRebuild(weedingGaugeLeftLayoutRectTransform));

            foreach (int index in gaugeIndices)
            {
                weedingGaugeLeftRectTransforms[index].DOSizeDelta(
                    new Vector2(weedingGaugeLeftRectTransforms[index].sizeDelta.x, _thinGaugeHeight),
                    gaugeSizeChangeDuration).SetEase(gaugeSizeChangeCurve);
            }
        }
    }
    
    public void ResetWeedingInputIndex(bool isUiRight)
    {
        if (isUiRight)
        {
            weedingGaugeRightRectTransforms[0].DOKill();
            weedingGaugeRightRectTransforms[0].sizeDelta = new Vector2(weedingGaugeRightRectTransforms[0].sizeDelta.x,
                _largeGaugeHeight);
            
            for (int i = 1; i < weedingGaugeRightRectTransforms.Count; i++)
            {
                weedingGaugeRightRectTransforms[i].DOKill();
                weedingGaugeRightRectTransforms[i].sizeDelta = new Vector2(weedingGaugeRightRectTransforms[i].sizeDelta.x,
                    _thinGaugeHeight);
            }
        }
        else
        {
            weedingGaugeLeftRectTransforms[0].DOKill();
            weedingGaugeLeftRectTransforms[0].sizeDelta = new Vector2(weedingGaugeLeftRectTransforms[0].sizeDelta.x,
                _largeGaugeHeight);
            
            for (int i = 1; i < weedingGaugeLeftRectTransforms.Count; i++)
            {
                weedingGaugeLeftRectTransforms[i].DOKill();
                weedingGaugeLeftRectTransforms[i].sizeDelta = new Vector2(weedingGaugeLeftRectTransforms[i].sizeDelta.x,
                    _thinGaugeHeight);
            }
        }
    }
    
    public void SetWeedingValue(float value, int sliderIndex, bool isUiRight)
    {
        if (isUiRight)
        {
            weedingGaugeRightSliders[sliderIndex].value = value;
        }
        else
        {
            weedingGaugeLeftSliders[sliderIndex].value = value;
        }
    }
    
    public void PressWeeding(bool isUiRight)
    {
        if (isUiRight)
        {
            weedingReleaseRightGameObject.SetActive(false);
        }
        else
        {
            weedingReleaseLeftGameObject.SetActive(false);
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
