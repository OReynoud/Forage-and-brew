using NaughtyAttributes;
using UnityEngine;
[CreateAssetMenu(fileName = "D_TutorialBlock", menuName = "Tutorial/TutoBlock")]
public class TutoBlockSo : ScriptableObject
{
    [field: BoxGroup] [field: SerializeField] [field: AllowNesting] [field: EnumFlags] public TutorialTriggerConditions triggerConditions { get; set; }
    
    [field: Foldout("If using time of the day")] [field: SerializeField] [field: AllowNesting] public TimeOfDay timeOftheDay { get; set; }
    
    [field: Foldout("If using zone trigger")] [field: SerializeField] [field: AllowNesting] public int triggerID { get; set; }

    [ResizableTextArea] public string title;
    [ResizableTextArea] public string content;
    public float stayTime = 3;
}