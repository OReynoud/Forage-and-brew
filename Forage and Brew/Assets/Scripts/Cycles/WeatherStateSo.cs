using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_WeatherState", menuName = "Weather/WeatherStateSo")]
public class WeatherStateSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public List<WeatherStateEndProbabilityBySuccessiveDayNumber> EndProbabilities { get; private set; }
    
    // Debugging
    private readonly List<int> _successiveDayNumbers = new();
    private float _totalEndProbability;

    private void OnValidate()
    {
        if (EndProbabilities == null || EndProbabilities.Count == 0)
        {
            Debug.LogError("EndProbabilities is null or empty.");
        }
        else
        {
            _successiveDayNumbers.Clear();
            
            foreach (WeatherStateEndProbabilityBySuccessiveDayNumber endProbability in EndProbabilities)
            {
                if (endProbability.WeatherStateEndProbabilities == null || endProbability.WeatherStateEndProbabilities.Count == 0)
                {
                    Debug.LogError("WeatherStateEndProbabilities is null or empty.");
                }
                else
                {
                    _totalEndProbability = 0f;
                    
                    foreach (WeatherStateEndProbability weatherStateEndProbability in endProbability.WeatherStateEndProbabilities)
                    {
                        if (weatherStateEndProbability.WeatherStateSo == null)
                        {
                            Debug.LogError("WeatherStateSo is null.");
                        }
                        
                        _totalEndProbability += weatherStateEndProbability.EndProbability;
                    }
                    
                    if (!Mathf.Approximately(_totalEndProbability, 1000f))
                    {
                        Debug.LogError("Total end probability for " + endProbability.SuccessiveDayNumber +
                                       " successive day(s) is not 1000.");
                    }
                }
                
                _successiveDayNumbers.Add(endProbability.SuccessiveDayNumber);
            }
            
            _successiveDayNumbers.Sort();
            
            for (int i = 0; i < _successiveDayNumbers.Count - 1; i++)
            {
                if (_successiveDayNumbers[i] == _successiveDayNumbers[i + 1])
                {
                    Debug.LogError("Successive day number " + _successiveDayNumbers[i] + " is duplicated.");
                }
            }
        }
    }
}
