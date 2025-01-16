using UnityEngine;

public class JoystickAnimationManagerBehaviour : MonoBehaviour
{
    [SerializeField] private JoystickAnimationValuesSo joystickAnimationValuesSo;
    [SerializeField] private RectTransform animatedJoystickRectTransform;
    
    public float AnimationDuration { get; set; }
    
    private bool _isPlayingAnimation;
    private bool _isUpdatingDelay;
    private bool _isCircular;
    private bool _isClockwise;
    private bool _isInASemiCircle;
    private float _animationTime;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;


    private void Start()
    {
        _startPosition = animatedJoystickRectTransform.anchoredPosition;
        AnimationDuration = joystickAnimationValuesSo.AnimationDuration;
    }
    
    private void Update()
    {
        if (!_isPlayingAnimation) return;

        UpdateCardinalAnimation();
        UpdateCircularAnimation();
        UpdateDelay();
    }

    private void OnEnable()
    {
        if (joystickAnimationValuesSo.IsDrivenExternally) return;
        
        switch (joystickAnimationValuesSo.JoystickDirection)
        {
            case JoystickDirection.Left:
                PlayLeftAnimation();
                break;
            case JoystickDirection.Right:
                PlayRightAnimation();
                break;
            case JoystickDirection.Down:
                PlayDownAnimation();
                break;
            case JoystickDirection.Up:
                PlayUpAnimation();
                break;
            case JoystickDirection.Clockwise:
                PlayClockwiseAnimation();
                break;
            case JoystickDirection.CounterClockwise:
                PlayCounterClockwiseAnimation();
                break;
            case JoystickDirection.ArcOfACircleDownCounterClockwise:
                PlayArcOfACircleDownCounterClockwiseAnimation();
                break;
        }
    }
    
    private void OnDisable()
    {
        StopAnimation();
    }


    private void UpdateCardinalAnimation()
    {
        if (_isUpdatingDelay) return;
        if (_isCircular) return;
        
        _animationTime += Time.deltaTime;
        animatedJoystickRectTransform.anchoredPosition = Vector2.Lerp(_startPosition, _targetPosition,
            joystickAnimationValuesSo.AnimationCurve.Evaluate(_animationTime / AnimationDuration));
        
        if (joystickAnimationValuesSo.IsDrivenExternally) return;
        
        if (_animationTime >= AnimationDuration)
        {
            if (_isInASemiCircle)
            {
                _isCircular = true;
            }
            else
            {
                _isUpdatingDelay = true;
            }
            
            _animationTime = 0f;
        }
    }
    
    private void UpdateCircularAnimation()
    {
        if (_isUpdatingDelay) return;
        if (!_isCircular) return;
        
        _animationTime += Time.deltaTime;
        animatedJoystickRectTransform.anchoredPosition = _isClockwise ? new Vector2(
            Mathf.Sin(_animationTime * Mathf.Deg2Rad * 180f + Mathf.Deg2Rad * 180f) *
            joystickAnimationValuesSo.AnimationDistance,
            Mathf.Cos(_animationTime * Mathf.Deg2Rad * 180f + Mathf.Deg2Rad * 180f) *
            joystickAnimationValuesSo.AnimationDistance) : new Vector2(
            Mathf.Cos(_animationTime * Mathf.Deg2Rad * 180f + Mathf.Deg2Rad * 90f) *
            joystickAnimationValuesSo.AnimationDistance,
            Mathf.Sin(_animationTime * Mathf.Deg2Rad * 180f + Mathf.Deg2Rad * 90f) *
            joystickAnimationValuesSo.AnimationDistance);
        
        if (joystickAnimationValuesSo.IsDrivenExternally) return;
        
        if (_animationTime >= AnimationDuration)
        {
            _isUpdatingDelay = true;
            _animationTime = 0f;
        }
    }
    
    private void UpdateDelay()
    {
        if (!_isUpdatingDelay) return;
        
        _animationTime += Time.deltaTime;
        
        if (_animationTime >= joystickAnimationValuesSo.AnimationDelay)
        {
            if (_isInASemiCircle)
            {
                _isCircular = false;
            }
            
            _isUpdatingDelay = false;
            _animationTime = 0f;
        }
    }


    public void PlayLeftAnimation()
    {
        _isPlayingAnimation = true;
        _isUpdatingDelay = false;
        _isCircular = false;
        _isInASemiCircle = false;
        _animationTime = 0f;
        animatedJoystickRectTransform.anchoredPosition = _startPosition;
        _targetPosition = new Vector2(_startPosition.x - joystickAnimationValuesSo.AnimationDistance, _startPosition.y);
    }

    public void PlayRightAnimation()
    {
        _isPlayingAnimation = true;
        _isUpdatingDelay = false;
        _isCircular = false;
        _isInASemiCircle = false;
        _animationTime = 0f;
        animatedJoystickRectTransform.anchoredPosition = _startPosition;
        _targetPosition = new Vector2(_startPosition.x + joystickAnimationValuesSo.AnimationDistance, _startPosition.y);
    }
    
    public void PlayDownAnimation()
    {
        _isPlayingAnimation = true;
        _isUpdatingDelay = false;
        _isCircular = false;
        _isInASemiCircle = false;
        _animationTime = 0f;
        animatedJoystickRectTransform.anchoredPosition = _startPosition;
        _targetPosition = new Vector2(_startPosition.x, _startPosition.y - joystickAnimationValuesSo.AnimationDistance);
    }
    
    public void PlayUpAnimation()
    {
        _isPlayingAnimation = true;
        _isUpdatingDelay = false;
        _isCircular = false;
        _isInASemiCircle = false;
        _animationTime = 0f;
        animatedJoystickRectTransform.anchoredPosition = _startPosition;
        _targetPosition = new Vector2(_startPosition.x, _startPosition.y + joystickAnimationValuesSo.AnimationDistance);
    }
    
    public void PlayClockwiseAnimation()
    {
        _isPlayingAnimation = true;
        _isUpdatingDelay = false;
        _isCircular = true;
        _isClockwise = true;
        _isInASemiCircle = false;
        _animationTime = 0f;
        animatedJoystickRectTransform.anchoredPosition = _startPosition - new Vector2(0f, joystickAnimationValuesSo.AnimationDistance * 0.5f);
    }
    
    public void PlayCounterClockwiseAnimation()
    {
        _isPlayingAnimation = true;
        _isUpdatingDelay = false;
        _isCircular = true;
        _isClockwise = false;
        _isInASemiCircle = false;
        _animationTime = 0f;
        animatedJoystickRectTransform.anchoredPosition = _startPosition - new Vector2(0f, joystickAnimationValuesSo.AnimationDistance * 0.5f);
    }
    
    public void PlayArcOfACircleDownCounterClockwiseAnimation()
    {
        _isPlayingAnimation = true;
        _isUpdatingDelay = false;
        _isCircular = false;
        _isClockwise = false;
        _isInASemiCircle = true;
        _animationTime = 0f;
        animatedJoystickRectTransform.anchoredPosition = _startPosition - new Vector2(0f, joystickAnimationValuesSo.AnimationDistance);
    }
    
    public void StopAnimation()
    {
        _isPlayingAnimation = false;
    }

    public void ResetClockwiseAnimation()
    {
        animatedJoystickRectTransform.anchoredPosition = _startPosition - new Vector2(0f, joystickAnimationValuesSo.AnimationDistance * 0.5f);
    }
}
