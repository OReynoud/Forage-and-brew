using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCrateManager : MonoBehaviour
{
    public static PotionCrateManager Instance { get; private set; }

    [field: SerializeField] public List<PotionCrateBehaviour> PotionCrates { get; private set; } = new();
    private readonly List<PotionCrateBehaviour> _triggeredPotionCrates = new();
    private PotionCrateBehaviour _currentPotionCrate;

    private Transform _playerTransform;


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        foreach (PotionCrateBehaviour potionBasket in PotionCrates)
        {
            potionBasket.PotionCrateManager = this;
        }
    }

    private void Start()
    {
        _playerTransform = CharacterMovementController.Instance.transform;

        StartCoroutine(StartingCoroutine());
    }

    private IEnumerator StartingCoroutine()
    {
        yield return new WaitUntil(() => OrderManager.Instance.IsInitialized);

        ReactivateRightPotionCrates();
    }

    private void Update()
    {
        ActivateRightPopup();
    }

    private void ActivateRightPopup()
    {
        if (_triggeredPotionCrates.Count == 0) return;

        float minDistance = float.MaxValue;
        PotionCrateBehaviour closestPotionCrate = null;

        foreach (PotionCrateBehaviour triggeredPotionCrate in _triggeredPotionCrates)
        {
            float distance = Vector3.Distance(triggeredPotionCrate.transform.position, _playerTransform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPotionCrate = triggeredPotionCrate;
            }
        }

        if (_currentPotionCrate != closestPotionCrate)
        {
            _currentPotionCrate?.DisablePopup();
            _currentPotionCrate = closestPotionCrate;
            _currentPotionCrate?.EnablePopup();
        }
    }


    public void ReactivateRightPotionCrates()
    {
        for (int i = 0; i < PotionCrates.Count; i++)
        {
            if (OrderManager.Instance.CurrentOrders[i] == null)
            {
                PotionCrates[i].DisableCrate();
                continue;
            }
        
            if (OrderManager.Instance.CurrentOrders[i].OrderContent == null)
            {
                PotionCrates[i].DisableCrate();
                continue;
            }
        
            PotionCrates[i].EnableCrate(
                OrderManager.Instance.CurrentOrders[i].OrderContent,
                OrderManager.Instance.CurrentOrders[i].RelatedLetter.Client,
                OrderManager.Instance.CurrentOrders[i].OrderDisplay);
        }
    }


    public void ManageTriggerEnter(PotionCrateBehaviour potionCrate)
    {
        _triggeredPotionCrates.Add(potionCrate);
    }

    public void ManageTriggerExit(PotionCrateBehaviour potionCrate)
    {
        if (_currentPotionCrate == potionCrate)
        {
            _currentPotionCrate.DisablePopup();
            _currentPotionCrate = null;
        }

        _triggeredPotionCrates.Remove(potionCrate);
    }
}