using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LockBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject lockCanvas;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Image lockImage;
    [SerializeField] private float lockSpriteChangeDelay = 0.5f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Constant(0f, 1f, 1f);
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    public void Unlock()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(lockCanvas.transform.DOMoveY(lockCanvas.transform.position.y + 1f, moveDuration)
            .SetEase(moveCurve));
        sequence.Join(lockImage.DOFade(0f, fadeDuration).SetEase(fadeCurve));
        sequence.OnComplete(() =>
        {
            lockCanvas.SetActive(false);
        });
        Sequence callbackSequence = DOTween.Sequence();
        callbackSequence.AppendCallback(() =>
        {
            lockImage.sprite = unlockedSprite;
        }).SetDelay(lockSpriteChangeDelay);
    }
    
    public void Disable()
    {
        lockCanvas.SetActive(false);
    }
}
