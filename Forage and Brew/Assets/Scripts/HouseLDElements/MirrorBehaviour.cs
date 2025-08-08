using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MirrorBehaviour : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CharacterOutfitListSo outfitListSo;
    [SerializeField] private Transform locationToWalk;
    
    [Header("Camera Settings")]
    [SerializeField] private CameraPreset mirrorCamera;
    [SerializeField] private float mirrorCameraTransitionDuration = 0.5f;
    [SerializeField] private CameraPreset usualCamera;
    [SerializeField] private float usualCameraTransitionDuration = 0.5f;
    
    [Header("Change Outfit")]
    [SerializeField] private float outfitChangeDelay = 0.1f;
    private int _currentOutfitIndex;
    private int _selectedOutfitIndex;
    private bool _canBuyCurrentOutfit;
    
    [Header("UI")]
    [SerializeField] private GameObject localInputCanvas;
    [SerializeField] private GameObject mirrorInterfaceCanvas;
    [SerializeField] private GameObject checkmarkGameObject;
    [SerializeField] private RectTransform leftArrowRectTransform;
    private float _leftArrowStartX;
    [SerializeField] private RectTransform rightArrowRectTransform;
    private float _rightArrowStartX;
    [SerializeField] private float arrowMoveDistance = 10f;
    [SerializeField] private float arrowMoveDuration = 0.1f;
    [SerializeField] private AnimationCurve arrowMoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private TMP_Text outfitNameText;
    [SerializeField] private GameObject selectLayout;
    [SerializeField] private CanvasGroup purchaseLayoutCanvasGroup;
    [SerializeField] private float purchaseLayoutUnavailableAlpha = 0.5f;
    [SerializeField] private float purchaseLayoutAvailableAlpha = 1f;
    [SerializeField] private GameObject purchaseButtonGameObject;
    [SerializeField] private RectTransform moneyCostLayoutRectTransform;
    [SerializeField] private TMP_Text moneyCostText;
    [SerializeField] private GameObject ingredientCostsLayout;
    [SerializeField] private RectTransform ingredientCostsLayoutRectTransform;
    [SerializeField] private List<GameObject> ingredientCostLayouts;
    [SerializeField] private List<TMP_Text> ingredientCostTexts;
    [SerializeField] private List<Image> ingredientCostImages;
    [SerializeField] private LockBehaviour lockBehaviour;
    
    // Global variables
    private bool _isUsingMirror;
    
    // Animator hashes
    private static readonly int DoNo = Animator.StringToHash("DoNo");


    private void Start()
    {
        localInputCanvas.SetActive(false);
        lockBehaviour.Disable();
        mirrorInterfaceCanvas.SetActive(false);
        
        _leftArrowStartX = leftArrowRectTransform.anchoredPosition.x;
        _rightArrowStartX = rightArrowRectTransform.anchoredPosition.x;
    }


    private void EnableInteract()
    {
        localInputCanvas.SetActive(true);
    }
    
    private void DisableInteract()
    {
        localInputCanvas.SetActive(false);
    }


    #region Enter Mirror
    
    public void EnterMirror()
    {
        if (_isUsingMirror) return;
        
        // Mirror variables
        _isUsingMirror = true;
        _selectedOutfitIndex = _currentOutfitIndex =
            outfitListSo.Outfits.IndexOf(GameDontDestroyOnLoadManager.Instance.CurrentOutfitSo);
        
        // Inputs
        CharacterInputManager.Instance.DisableInputs();
        
        // Movement
        CharacterMovementController.Instance.TriggerWalkTransition(locationToWalk, mirrorCameraTransitionDuration);
        
        // UI
        localInputCanvas.SetActive(false);
        InfoDisplayManager.instance.ShowOnlyMoney(true);
        
        // Camera
        StartCoroutine(ChangeToMirrorCameraCoroutine());
    }

    private IEnumerator ChangeToMirrorCameraCoroutine()
    {
        // Camera transition to mirror
        ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = true;
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(mirrorCamera, mirrorCameraTransitionDuration);
        
        // Wait for the camera transition to complete
        yield return new WaitForSeconds(mirrorCameraTransitionDuration);
        
        OpenMirrorInterface();
    }
    
    private void OpenMirrorInterface()
    {
        // UI
        mirrorInterfaceCanvas.SetActive(true);
        leftArrowRectTransform.anchoredPosition = new Vector2(_leftArrowStartX, leftArrowRectTransform.anchoredPosition.y);
        rightArrowRectTransform.anchoredPosition = new Vector2(_rightArrowStartX, rightArrowRectTransform.anchoredPosition.y);
        UpdateOutfitUI();
        UpdateLockUI();
        
        // Inputs
        CharacterInputManager.Instance.EnableMirrorInputs();
    }

    #endregion


    #region Exit Mirror

    public void ExitMirror()
    {
        if (!_isUsingMirror) return;
        
        // Set selected outfit
        if (_selectedOutfitIndex != _currentOutfitIndex)
        {
            StartCoroutine(ChangeOutfit(outfitListSo.Outfits[_selectedOutfitIndex]));
        }
        
        // Mirror variables
        _isUsingMirror = false;
        
        // Inputs
        CharacterInputManager.Instance.DisableMirrorInputs();
        
        // UI
        mirrorInterfaceCanvas.SetActive(false);
        lockBehaviour.Disable();
        
        // Camera
        StartCoroutine(ChangeToUsualCameraCoroutine());
    }

    private IEnumerator ChangeToUsualCameraCoroutine()
    {
        // Camera transition to usual
        SimpleCameraBehavior.instance.ApplyScriptableCamSettings(usualCamera, usualCameraTransitionDuration);
        
        // Wait for the camera transition to complete
        yield return new WaitForSeconds(usualCameraTransitionDuration);
        
        EnableUsualBehaviour();
    }

    private void EnableUsualBehaviour()
    {
        // UI
        localInputCanvas.SetActive(true);
        InfoDisplayManager.instance.ShowOnlyMoney(false);
        
        // Camera
        ((HouseCameraBehavior)HouseCameraBehavior.instance).overrideCameraLerp = false;
        
        // Inputs
        CharacterInputManager.Instance.EnableInputs();
    }

    #endregion
    

    #region Mirror Interface

    public void PreviousOutfit()
    {
        if (!_isUsingMirror) return;
        
        _currentOutfitIndex--;
        
        if (_currentOutfitIndex < 0)
        {
            _currentOutfitIndex = outfitListSo.Outfits.Count - 1; // Loop back to the last outfit
        }
        
        StartCoroutine(ChangeOutfit(outfitListSo.Outfits[_currentOutfitIndex]));
        
        // TODO: UI behaviour to show outfit change success
        leftArrowRectTransform.DOAnchorPosX(_leftArrowStartX - arrowMoveDistance, arrowMoveDuration)
            .SetEase(arrowMoveCurve).SetLoops(2, LoopType.Yoyo);
        
        // Update UI for outfit selection
        UpdateOutfitUI();
        UpdateLockUI();
    }

    public void NextOutfit()
    {
        if (!_isUsingMirror) return;
        
        _currentOutfitIndex++;
        
        if (_currentOutfitIndex >= outfitListSo.Outfits.Count)
        {
            _currentOutfitIndex = 0; // Loop back to the first outfit
        }
        
        StartCoroutine(ChangeOutfit(outfitListSo.Outfits[_currentOutfitIndex]));
        
        // TODO: UI behaviour to show outfit change success
        rightArrowRectTransform.DOAnchorPosX(_rightArrowStartX + arrowMoveDistance, arrowMoveDuration)
            .SetEase(arrowMoveCurve).SetLoops(2, LoopType.Yoyo);
        
        // Update UI for outfit selection
        UpdateOutfitUI();
        UpdateLockUI();
    }

    public void SelectOutfit()
    {
        if (!_isUsingMirror) return;
        
        if (!GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(outfitListSo.Outfits[_currentOutfitIndex])) return;
        
        _selectedOutfitIndex = _currentOutfitIndex;
        
        // TODO: UI behaviour to show outfit selection success
        
        UpdateOutfitUI();
    }

    public void PurchaseOutfit()
    {
        if (!_isUsingMirror) return;
        
        CharacterOutfitSo outfit = outfitListSo.Outfits[_currentOutfitIndex];
        
        if (GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(outfit)) return;
        
        if (MoneyManager.Instance.MoneyAmount < outfit.OutfitMoneyCost ||
            outfit.IngredientCosts.Any(ingredientCost =>
                GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Count(ingredient =>
                    ingredientCost.Ingredient == ingredient) +
                GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Count(collectedIngredient =>
                    ingredientCost.Ingredient == collectedIngredient.IngredientValuesSo)
                < ingredientCost.Amount))
        {
            CharacterAnimManager.instance.animator.SetTrigger(DoNo);
            return;
        }

        // Pay for the outfit
        MoneyManager.Instance.SubtractMoney(outfit.OutfitMoneyCost);
        foreach (IngredientCost ingredientCost in outfit.IngredientCosts)
        {
            for (int i = 0; i < ingredientCost.Amount; i++)
            {
                // Remove the ingredient from collected ingredients
                if (GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Remove(ingredientCost.Ingredient)) continue;
                
                // If not found in collected ingredients, remove from out collected ingredients
                CollectedIngredientBehaviour collectedIngredient = GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients
                    .First(collectedIngredient => collectedIngredient.IngredientValuesSo == ingredientCost.Ingredient);
                GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Remove(collectedIngredient);
                Destroy(collectedIngredient.gameObject);
            }
        }
        
        // TODO: UI behaviour to show outfit purchase success
        
        lockBehaviour.Unlock();
        
        StartCoroutine(UnlockOutfitCoroutine());
    }
    
    private IEnumerator UnlockOutfitCoroutine()
    {
        // Wait for the next frame
        yield return new WaitForEndOfFrame();
        
        // Unlock the outfit
        GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Add(outfitListSo.Outfits[_currentOutfitIndex]);
        
        // Update the current outfit UI
        UpdateOutfitUI();
    }

    private IEnumerator ChangeOutfit(CharacterOutfitSo outfit)
    {
        CharacterVfxManager.Instance.PlayPuffVfx();
        
        yield return new WaitForSeconds(outfitChangeDelay);
        
        CharacterAnimManager.instance.SetOutfit(outfit);
    }

    private void UpdateOutfitUI()
    {
        CharacterOutfitSo outfit = outfitListSo.Outfits[_currentOutfitIndex];
        
        outfitNameText.text = outfit.OutfitName;
        outfitNameText.color = outfit.OutfitColor;
        
        checkmarkGameObject.SetActive(_currentOutfitIndex == _selectedOutfitIndex);
        
        if (GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(outfit))
        {
            selectLayout.SetActive(_currentOutfitIndex != _selectedOutfitIndex);
            
            purchaseLayoutCanvasGroup.gameObject.SetActive(false);
        }
        else
        {
            selectLayout.SetActive(false);
            
            _canBuyCurrentOutfit = !(MoneyManager.Instance.MoneyAmount < outfit.OutfitMoneyCost ||
                                    outfit.IngredientCosts.Any(ingredientCost =>
                                        GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Count(ingredient =>
                                            ingredientCost.Ingredient == ingredient) +
                                        GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Count(collectedIngredient =>
                                            ingredientCost.Ingredient == collectedIngredient.IngredientValuesSo)
                                        < ingredientCost.Amount));
            
            purchaseLayoutCanvasGroup.gameObject.SetActive(true);
            purchaseLayoutCanvasGroup.alpha = _canBuyCurrentOutfit ? purchaseLayoutAvailableAlpha : purchaseLayoutUnavailableAlpha;
            purchaseButtonGameObject.SetActive(_canBuyCurrentOutfit);
            
            moneyCostText.text = outfit.OutfitMoneyCost.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(moneyCostLayoutRectTransform);
            
            if (outfit.IngredientCosts.Count == 0)
            {
                ingredientCostsLayout.SetActive(false);
            }
            else
            {
                ingredientCostsLayout.SetActive(true);
                
                for (int i = 0; i < ingredientCostsLayout.transform.childCount; i++)
                {
                    ingredientCostLayouts[i].SetActive(false);
                }

                for (int i = 0; i < outfit.IngredientCosts.Count; i++)
                {
                    IngredientCost ingredientCost = outfit.IngredientCosts[i];
                    ingredientCostLayouts[i].SetActive(true);
                    ingredientCostTexts[i].text = ingredientCost.Amount.ToString();
                    ingredientCostImages[i].sprite = ingredientCost.Ingredient.iconLow;
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(ingredientCostsLayoutRectTransform);
            }
        }
    }

    private void UpdateLockUI()
    {
        if (GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(outfitListSo.Outfits[_currentOutfitIndex]))
        {
            lockBehaviour.Disable();
        }
        else
        {
            lockBehaviour.Enable();
        }
    }
    
    #endregion
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) && 
            characterInteractController.collectedStack.Count == 0)
        {
            CharacterInteractController.Instance.CurrentNearMirror = this;
            EnableInteract();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            characterInteractController.CurrentNearMirror == this)
        {
            CharacterInteractController.Instance.CurrentNearMirror = null;
            DisableInteract();
        }
    }
}
