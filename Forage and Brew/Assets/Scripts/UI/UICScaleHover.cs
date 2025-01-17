using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public class UIScaleHover : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hover")]
    [SerializeField] private Vector3 multiplier;
    
    [SerializeField] private Vector2 durationInOut;
    
    [SerializeField] private AnimationCurve inCurve;
    [SerializeField] private AnimationCurve outCurve;

    [SerializeField] private Color hoverColor;
    [SerializeField] private Color unhoverColor;
    
    [Header("Click")]
    [SerializeField] private Vector3 multiplierClick;
    
    [SerializeField] private Vector2 durationClickInOut;
    
    [SerializeField] private AnimationCurve inCurveClick;
    [SerializeField] private AnimationCurve outCurveClick;
    
    private Vector3 _baseSize;
    
    void Start()
    {
        _baseSize = transform.localScale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.localScale = _baseSize;
        transform.DOScale(new Vector3(_baseSize.x * multiplierClick.x, _baseSize.y * multiplierClick.y, _baseSize.z * multiplierClick.z), durationClickInOut.x).SetEase(inCurveClick).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(new Vector3(_baseSize.x, _baseSize.y, _baseSize.z), durationClickInOut.y).SetEase(outCurveClick).SetUpdate(true);
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(new Vector3(_baseSize.x * multiplier.x, _baseSize.y * multiplier.y, _baseSize.z * multiplier.z), durationInOut.x).SetEase(inCurve).SetUpdate(true);

        GetComponent<Image>().DOColor(hoverColor, 0.2f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(_baseSize, durationInOut.y).SetEase(outCurve).SetUpdate(true);
        
        GetComponent<Image>().DOColor(unhoverColor, 0.2f);
    }
}
