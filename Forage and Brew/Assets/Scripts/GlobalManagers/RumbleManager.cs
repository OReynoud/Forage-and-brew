using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    // Singleton
    public static RumbleManager Instance { get; private set; }
    
    private List<float> _rumbleDurationsLeft;
    private float _rumbleTimeLeft;
    private List<float> _rumbleIntervalsLeft;
    private float _rumbleIntervalTimeLeft;
    private bool _isRumbleIncremental;
    private float _rumbleTargetMagnitude;
    private float _rumbleTargetDuration;
    private AnimationCurve _rumbleCurve;

    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    private void Update()
    {
        UpdateCurrentRumble();
        UpdateRumbleInterval();
    }


    public void PlayRumble(float duration, float magnitude)
    {
        if (Gamepad.current == null) return;
        
        Gamepad.current.SetMotorSpeeds(magnitude * 0.5f, magnitude);
        _rumbleIntervalTimeLeft = 0f;
        _rumbleDurationsLeft = new List<float>();
        _rumbleTimeLeft = duration;
    }

    public void PlayIncrementalRumble(float duration, float magnitude, AnimationCurve curve)
    {
        if (Gamepad.current == null) return;

        _isRumbleIncremental = true;
        _rumbleTargetMagnitude = magnitude;
        _rumbleTargetDuration = duration;
        _rumbleCurve = curve;
        _rumbleIntervalTimeLeft = 0f;
        _rumbleDurationsLeft = new List<float>();
        _rumbleTimeLeft = duration;
    }

    public void PlayMultipleRumbles(List<float> durations, float magnitude, List<float> intervals)
    {
        if (Gamepad.current == null) return;

        Gamepad.current.SetMotorSpeeds(magnitude * 0.5f, magnitude);
        _rumbleDurationsLeft = durations.GetRange(1, durations.Count - 1);
        _rumbleTimeLeft = durations[0];
        _rumbleIntervalsLeft = new List<float>(intervals);
    }

    private void UpdateCurrentRumble()
    {
        if (Gamepad.current == null) return;
        
        if (_rumbleTimeLeft <= 0f) return;
        
        if (_isRumbleIncremental)
        {
            float currentTime = _rumbleTargetDuration - _rumbleTimeLeft;
            float normalizedTime = Mathf.Clamp01(currentTime / _rumbleTargetDuration);
            float curveValue = _rumbleCurve.Evaluate(normalizedTime);
            float currentMagnitude = curveValue * _rumbleTargetMagnitude;
            Gamepad.current.SetMotorSpeeds(currentMagnitude * 0.5f, currentMagnitude);
        }
        
        _rumbleTimeLeft -= Time.deltaTime;
        
        if (_rumbleTimeLeft <= 0f)
        {
            if (_rumbleDurationsLeft.Count > 0)
            {
                _rumbleIntervalTimeLeft = _rumbleIntervalsLeft[0];
                _rumbleIntervalsLeft.RemoveAt(0);
                
                InputSystem.PauseHaptics();
            }
            else
            {
                _isRumbleIncremental = false;
                InputSystem.ResetHaptics();
            }
        }
    }

    private void UpdateRumbleInterval()
    {
        if (Gamepad.current == null) return;
        
        if (_rumbleIntervalTimeLeft <= 0f) return;
        
        _rumbleIntervalTimeLeft -= Time.deltaTime;
        
        if (_rumbleIntervalTimeLeft <= 0f)
        {
            _rumbleTimeLeft = _rumbleDurationsLeft[0];
            _rumbleDurationsLeft.RemoveAt(0);
            
            InputSystem.ResumeHaptics();
        }
    }
}
