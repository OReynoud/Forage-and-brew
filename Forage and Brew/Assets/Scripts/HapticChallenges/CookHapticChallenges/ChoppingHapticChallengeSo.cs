using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_ChoppingHapticChallenge", menuName = "Haptic Challenges/ChoppingHapticChallengeSo")]
public class ChoppingHapticChallengeSo : ScriptableObject
{
    [field: SerializeField] [field: Tooltip("The sequence of inputs to perform.")] [field: Range(1, 3)]
    public List<int> ChoppingInputIndices { get; private set; }
    
    
    private void OnValidate()
    {
        for (int i = 0; i < ChoppingInputIndices.Count; i++)
        {
            ChoppingInputIndices[i] = Mathf.Clamp(ChoppingInputIndices[i], 1, 3);
        }
    }
}
