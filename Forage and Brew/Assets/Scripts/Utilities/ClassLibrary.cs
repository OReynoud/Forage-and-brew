using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]

public class Letter
{
    [field: SerializeField] public LetterContentSo LetterContent { get; set; }
    [field: SerializeField] public NarrativeBlockOfLetters RelatedNarrativeBlock { get; set; }
    [field: SerializeField] public FillerBlockOfLetters RelatedFillerBlock { get; set; }

    public Letter(LetterContentSo content, NarrativeBlockOfLetters nBlock)
    {
        LetterContent = content;
        RelatedNarrativeBlock = nBlock;
    }
    public Letter(LetterContentSo content, FillerBlockOfLetters fBlock)
    {
        LetterContent = content;
        RelatedFillerBlock = fBlock;
    }

    public Letter(LetterContentSo content)
    {
        LetterContent = content;
    }
}

[Serializable]
public class Order
{
    [field: SerializeField] public OrderContentSo OrderContent { get; set; }
    public OrderCodexDisplayBehaviour OrderDisplay { get; set; }
    [field: SerializeField] public NarrativeBlockOfLetters RelatedNarrativeBlock { get; private set; }
    [field: SerializeField] public LetterContentSo RelatedLetter { get; private set; }

    public Order(Letter LetterToOrder, OrderCodexDisplayBehaviour orderDisplay)
    {
        OrderDisplay = orderDisplay;
        OrderContent = LetterToOrder.LetterContent.OrderContent;
        RelatedLetter = LetterToOrder.LetterContent;
        RelatedNarrativeBlock = LetterToOrder.RelatedNarrativeBlock;
    }
}

[Serializable]
public class PotionDemand
{
    [field: SerializeField] public bool IsSpecific { get; private set; }
    [field: AllowNesting] [field: ShowIf("IsSpecific")] [field: SerializeField] public PotionValuesSo Potion { get; private set; }
    [field: AllowNesting] [field: HideIf("IsSpecific")] [field: SerializeField] public string Keywords { get; private set; }
    [field: AllowNesting] [field: HideIf("IsSpecific")] [field: SerializeField] public PotionTagSo ValidTag { get; private set; }

    public PotionDemand(PotionValuesSo newPotion)
    {
        Potion = newPotion;
        IsSpecific = true;
    }
    
    public PotionDemand(PotionTagSo newTag, string newKeywords)
    {
        ValidTag = newTag;
        IsSpecific = false;
        Keywords = newKeywords;
    }
}

[Serializable]
public class NarrativeBlockOfLetters
{
    [field: SerializeField] public NarrativeBlockOfLettersContentSo ContentSo { get; set; }
    
    [field: AllowNesting] [field: SerializeField] [field: ReadOnly] public int SelfProgressionIndex { get; set; }
    [field: SerializeField] [field: HideInInspector] public bool[] CompletedLetters { get; set; }
    [field: SerializeField] [field: HideInInspector] public bool[] InactiveLetters { get; set; }
    [field: AllowNesting] [field: SerializeField] [field: ReadOnly] public int NewLetterCountDown { get; set; }

    public NarrativeBlockOfLetters(NarrativeBlockOfLettersContentSo content)
    {
        ContentSo = content;
        
        CompletedLetters = new bool [ContentSo.Content.Count];
        InactiveLetters = new bool [ContentSo.Content.Count];
    }
}

[Serializable]
public class FillerBlockOfLetters
{
    [field: SerializeField] public FillerBlockLettersContentSo ContentSo { get; set; }
    [field: SerializeField] public bool HasUsedFirstLetter { get; set; }
    [field: SerializeField] public LetterContentSo LastUsedLetter { get; set; }

    public FillerBlockOfLetters(FillerBlockLettersContentSo content, bool hasUsedFirstLetter,
        LetterContentSo lastUsedLetter)
    {
        ContentSo = content;
        HasUsedFirstLetter = hasUsedFirstLetter;
        LastUsedLetter = lastUsedLetter;
    }
}

[Serializable]
public class ClientOrderPotions
{
    public ClientOrderPotions(OrderContentSo orderSo, ClientSo clientSo, List<FloorCookedPotion> potions)
    {
        OrderSo = orderSo;
        ClientSo = clientSo;
        Potions = potions;
    }

    [field: SerializeField] public OrderContentSo OrderSo { get; set; }
    [field: SerializeField] public ClientSo ClientSo { get; set; }
    [field: SerializeField] public List<FloorCookedPotion> Potions { get; set; } = new();
}

[Serializable]
public class WeatherSuccessiveDays
{
    [field: SerializeField] public WeatherStateSo WeatherStateSo { get; set; }
    [field: SerializeField] public int SuccessiveDays { get; set; }
    
    public WeatherSuccessiveDays(WeatherStateSo weatherStateSo, int successiveDays)
    {
        WeatherStateSo = weatherStateSo;
        SuccessiveDays = successiveDays;
    }
}

[Serializable]
public class TutorialBlock
{
    [field: SerializeField] [field: AllowNesting] [field: ReadOnly] public bool hasBeenTriggered { get; set; }

    [field: SerializeField] [field: AllowNesting] [field: ReadOnly] public TutoBlockSo data { get; set; }

    public TutorialBlock(TutoBlockSo Data)
    {
        data = Data;
        hasBeenTriggered = false;
    }

}

[Serializable]
public class HouseCameraSetting
{
    public CameraPreset cameraPreset;
    public float triggerDistance = 1;
}

[Serializable]
public class SpawnGroupCount
{
    [field: SerializeField] public Transform SpawnGroupTransform { get; set; }
    [field: SerializeField] public int SpawnCount { get; set; }
}

public class StackableItem : MonoBehaviour
{
    protected bool isBeingDroppedInTarget;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _startRotation;
    private Vector3 _endRotation;
    public float dropInTargetLerp;
    private Vector3 _dropTargetOffset;
    protected float lerp;
    public virtual void EnableGrab(){}
    public virtual void DisableGrab(){}
    public virtual void GrabMethod(bool grab){}

    public void Update()
    {
        if (!isBeingDroppedInTarget) return;

        if (lerp >= 1f)
        {
            StackableDropped();
            return;
        }
        
        lerp += Time.deltaTime * dropInTargetLerp;
        transform.position = Vector3.Lerp(_startPosition,_endPosition,lerp) + Vector3.up * ShoveHeightCurve.Evaluate(lerp);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(_startRotation),Quaternion.Euler(_endRotation), ShoveRotationCurve.Evaluate(lerp) );
    }

    public virtual void StackableDropped()
    {
        
    }

    public float StackHeight { get; protected set; }
    
    public void DropInTarget(Transform target, bool randomizeEndPoint, Vector3 offset = default)
    {
        _dropTargetOffset = offset;
        _startRotation = transform.eulerAngles;
        _endRotation = target.eulerAngles;
        if (randomizeEndPoint)
        {
            _startPosition = transform.position;
            _endPosition = target.position;
        }
        else
        {
            _startPosition = transform.position;
            _endPosition = target.position+ _dropTargetOffset + Vector3.up + new Vector3(Random.Range(-1f, 1f), Random.value, Random.Range(-1f, 1f));
        }
        lerp = 0f;
        isBeingDroppedInTarget = true;
    }
    public Transform GetTransform() => transform;

    public virtual StackableValuesSo GetStackableValuesSo()
    {
        return default;
    }
    [field: SerializeField] public AnimationCurve ShoveHeightCurve { get; set; }
    [field: SerializeField] public AnimationCurve ShoveRotationCurve { get; set; } = AnimationCurve.EaseInOut(0,0,1,1);
}
