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
    private bool _isCurrentRainOutfit;
    private int _selectedOutfitIndex;
    private bool _isSelectedRainOutfit;
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
    [SerializeField] private TMP_Text outfitCategoryNameText;
    [SerializeField] private ScrollRect outfitScrollRect;
    [SerializeField] private float outfitScrollPadding = 10f;
    [SerializeField] private float outfitScrollDuration = 0.2f;
    [SerializeField] private AnimationCurve outfitScrollCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private Transform outfitGridLayoutTransform;
    [SerializeField] private OutfitButtonBehaviour outfitButtonPrefab;
    private readonly List<OutfitButtonBehaviour> _outfitButtons = new();
    [SerializeField] private CanvasGroup mainButtonCanvasGroup;
    [SerializeField] private float mainButtonUnavailableAlpha = 0.5f;
    [SerializeField] private float mainButtonAvailableAlpha = 1f;
    [SerializeField] private GameObject selectLayout;
    [SerializeField] private GameObject purchaseLayout;
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
        
        RegenerateOutfitUI();
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
        _isSelectedRainOutfit = _isCurrentRainOutfit = GameDontDestroyOnLoadManager.Instance.CurrentOutfitSo.IsRainOutfit;
        _selectedOutfitIndex = _currentOutfitIndex = (_isSelectedRainOutfit ? 
                outfitListSo.RainOutfits : outfitListSo.CasualOutfits)
            .IndexOf(GameDontDestroyOnLoadManager.Instance.CurrentOutfitSo);
        
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
        RegenerateOutfitUI();
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
            StartCoroutine(ChangeOutfitCoroutine(_isSelectedRainOutfit ? outfitListSo.RainOutfits[_selectedOutfitIndex] : 
                outfitListSo.CasualOutfits[_selectedOutfitIndex]));
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

    public void PreviousOutfitCategory()
    {
        if (!_isUsingMirror) return;
        
        _isCurrentRainOutfit = !_isCurrentRainOutfit;
        
        if (_currentOutfitIndex >= (_isCurrentRainOutfit ? outfitListSo.RainOutfits : outfitListSo.CasualOutfits).Count)
        {
            _currentOutfitIndex = 0; // Loop back to the first outfit
        }
        
        StartCoroutine(ChangeOutfitCoroutine(_isCurrentRainOutfit ? outfitListSo.RainOutfits[_currentOutfitIndex] : 
            outfitListSo.CasualOutfits[_currentOutfitIndex]));
        
        // TODO: UI behaviour to show outfit change success
        leftArrowRectTransform.DOAnchorPosX(_leftArrowStartX - arrowMoveDistance, arrowMoveDuration)
            .SetEase(arrowMoveCurve).SetLoops(2, LoopType.Yoyo);
        
        // Update UI for outfit selection
        UpdateOutfitCategoryUI();
        UpdateLockUI();
    }

    public void NextOutfitCategory()
    {
        if (!_isUsingMirror) return;
        
        _isCurrentRainOutfit = !_isCurrentRainOutfit;
        
        if (_currentOutfitIndex >= (_isCurrentRainOutfit ? outfitListSo.RainOutfits : outfitListSo.CasualOutfits).Count)
        {
            _currentOutfitIndex = 0; // Loop back to the first outfit
        }
        
        StartCoroutine(ChangeOutfitCoroutine(_isCurrentRainOutfit ? outfitListSo.RainOutfits[_currentOutfitIndex] : 
            outfitListSo.CasualOutfits[_currentOutfitIndex]));
        
        // TODO: UI behaviour to show outfit change success
        rightArrowRectTransform.DOAnchorPosX(_rightArrowStartX + arrowMoveDistance, arrowMoveDuration)
            .SetEase(arrowMoveCurve).SetLoops(2, LoopType.Yoyo);
        
        // Update UI for outfit selection
        UpdateOutfitCategoryUI();
        UpdateLockUI();
    }

    public void LeftOutfit()
    {
        if (!_isUsingMirror) return;

        _currentOutfitIndex += _currentOutfitIndex % 2 == 1 ? -1 : 1;
        
        if (_currentOutfitIndex >= (_isCurrentRainOutfit ? outfitListSo.RainOutfits : outfitListSo.CasualOutfits).Count)
        {
            _currentOutfitIndex--; // Go back to the last valid outfit
        }
        else
        {
            StartCoroutine(ChangeOutfitCoroutine(_isCurrentRainOutfit ? outfitListSo.RainOutfits[_currentOutfitIndex] : 
                outfitListSo.CasualOutfits[_currentOutfitIndex]));
        }
        
        // TODO: UI behaviour to show outfit change success
        
        // Update UI for outfit selection
        UpdateOutfitUI();
        UpdateLockUI();
    }
    
    public void RightOutfit()
    {
        if (!_isUsingMirror) return;
        
        _currentOutfitIndex += _currentOutfitIndex % 2 == 1 ? -1 : 1;
        
        if (_currentOutfitIndex >= (_isCurrentRainOutfit ? outfitListSo.RainOutfits : outfitListSo.CasualOutfits).Count)
        {
            _currentOutfitIndex--; // Go back to the last valid outfit
        }
        else
        {
            StartCoroutine(ChangeOutfitCoroutine(_isCurrentRainOutfit ? outfitListSo.RainOutfits[_currentOutfitIndex] : 
                outfitListSo.CasualOutfits[_currentOutfitIndex]));
        }
        
        // TODO: UI behaviour to show outfit change success
        
        // Update UI for outfit selection
        UpdateOutfitUI();
        UpdateLockUI();
    }
    
    public void UpOutfit()
    {
        if (!_isUsingMirror) return;

        _currentOutfitIndex -= 2;
        
        if (_currentOutfitIndex < 0)
        {
            int newCurrentOutfitIndex = (_isCurrentRainOutfit ? outfitListSo.RainOutfits : outfitListSo.CasualOutfits)
                                        .Count - 1; // Loop back to the last outfit
            _currentOutfitIndex = newCurrentOutfitIndex % 2 == _currentOutfitIndex % 2 ?
                newCurrentOutfitIndex : newCurrentOutfitIndex - 1;
        }
        
        StartCoroutine(ChangeOutfitCoroutine(_isCurrentRainOutfit ? outfitListSo.RainOutfits[_currentOutfitIndex] : 
            outfitListSo.CasualOutfits[_currentOutfitIndex]));
        
        // TODO: UI behaviour to show outfit change success
        
        // Update UI for outfit selection
        UpdateOutfitUI();
        UpdateLockUI();
    }
    
    public void DownOutfit()
    {
        if (!_isUsingMirror) return;

        _currentOutfitIndex += 2;
        
        if (_currentOutfitIndex >= (_isCurrentRainOutfit ? outfitListSo.RainOutfits : outfitListSo.CasualOutfits).Count)
        {
            _currentOutfitIndex = _currentOutfitIndex % 2 == 0 ? 0 : 1; // Loop back to the first or second outfit
        }
        
        StartCoroutine(ChangeOutfitCoroutine(_isCurrentRainOutfit ? outfitListSo.RainOutfits[_currentOutfitIndex] : 
            outfitListSo.CasualOutfits[_currentOutfitIndex]));
        
        // TODO: UI behaviour to show outfit change success
        
        // Update UI for outfit selection
        UpdateOutfitUI();
        UpdateLockUI();
    }

    public void SelectOutfit()
    {
        if (!_isUsingMirror) return;
        
        if (!GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(_isCurrentRainOutfit ?
                outfitListSo.RainOutfits[_currentOutfitIndex] : outfitListSo.CasualOutfits[_currentOutfitIndex])) return;
        
        _selectedOutfitIndex = _currentOutfitIndex;
        _isSelectedRainOutfit = _isCurrentRainOutfit;
        
        // TODO: UI behaviour to show outfit selection success
        
        UpdateOutfitUI();
    }

    public void PurchaseOutfit()
    {
        if (!_isUsingMirror) return;
        
        CharacterOutfitSo outfit = _isCurrentRainOutfit ? outfitListSo.RainOutfits[_currentOutfitIndex] : 
            outfitListSo.CasualOutfits[_currentOutfitIndex];
        
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
        _outfitButtons[_currentOutfitIndex].UnlockOutfit();
        
        StartCoroutine(UnlockOutfitCoroutine());
    }
    
    private IEnumerator UnlockOutfitCoroutine()
    {
        // Wait for the next frame
        yield return new WaitForEndOfFrame();
        
        // Unlock the outfit
        GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Add(_isCurrentRainOutfit ?
            outfitListSo.RainOutfits[_currentOutfitIndex] : outfitListSo.CasualOutfits[_currentOutfitIndex]);
        
        // Update the current outfit UI
        UpdateOutfitUI();
    }

    private IEnumerator ChangeOutfitCoroutine(CharacterOutfitSo outfit)
    {
        CharacterVfxManager.Instance.PlayPuffVfx();
        
        yield return new WaitForSeconds(outfitChangeDelay);
        
        CharacterAnimManager.instance.SetOutfit(outfit);
    }

    private void RegenerateOutfitUI()
    {
        // Clear existing buttons
        foreach (Transform outfitButton in outfitGridLayoutTransform)
        {
            Destroy(outfitButton.gameObject);
        }
        
        _outfitButtons.Clear();

        List<CharacterOutfitSo> outfitList = _isCurrentRainOutfit ? outfitListSo.RainOutfits : outfitListSo.CasualOutfits;
        
        // Create new buttons
        foreach (CharacterOutfitSo outfit in outfitList)
        {
            OutfitButtonBehaviour outfitButton = Instantiate(outfitButtonPrefab, outfitGridLayoutTransform);
            outfitButton.Initialize(this, outfit);
            _outfitButtons.Add(outfitButton);
        }

        // Ensure current outfit index is within bounds
        if (_currentOutfitIndex >= outfitList.Count)
        {
            _currentOutfitIndex = outfitList.Count - 1;
        }
        
        UpdateOutfitUI();
    }

    private void UpdateOutfitCategoryUI()
    {
        // Update category name
        outfitCategoryNameText.text = _isCurrentRainOutfit ? "Rain Outfits" : "Casual Outfits";

        // Regenerate outfit buttons
        RegenerateOutfitUI();
    }

    private void UpdateOutfitUI()
    {
        List<CharacterOutfitSo> outfitList = _isCurrentRainOutfit ? outfitListSo.RainOutfits : outfitListSo.CasualOutfits;
        
        CharacterOutfitSo currentOutfit = outfitList[_currentOutfitIndex];
        
        // Update outfit buttons
        for (int i = 0; i < _outfitButtons.Count; i++)
        {
            OutfitButtonBehaviour outfitButton = _outfitButtons[i];

            if (i == _currentOutfitIndex)
            {
                outfitButton.EnableSelector();
                StartCoroutine(UpdateOutfitUIScrollCoroutine(outfitButton));
            }
            else
            {
                outfitButton.DisableSelector();
            }

            if (i == _selectedOutfitIndex && 
                ((_isSelectedRainOutfit && _isCurrentRainOutfit) || (!_isSelectedRainOutfit && !_isCurrentRainOutfit)))
            {
                outfitButton.EnableCheckmark();
            }
            else
            {
                outfitButton.DisableCheckmark();
            }
        }
        
        // Update main button and layouts
        if (GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(currentOutfit))
        {
            mainButtonCanvasGroup.alpha = _currentOutfitIndex != _selectedOutfitIndex ?
                mainButtonAvailableAlpha : mainButtonUnavailableAlpha;
            purchaseLayout.SetActive(false);
            selectLayout.SetActive(true);
        }
        else
        {
            _canBuyCurrentOutfit = !(MoneyManager.Instance.MoneyAmount < currentOutfit.OutfitMoneyCost ||
                                    currentOutfit.IngredientCosts.Any(ingredientCost =>
                                        GameDontDestroyOnLoadManager.Instance.CollectedIngredients.Count(ingredient =>
                                            ingredientCost.Ingredient == ingredient) +
                                        GameDontDestroyOnLoadManager.Instance.OutCollectedIngredients.Count(collectedIngredient =>
                                            ingredientCost.Ingredient == collectedIngredient.IngredientValuesSo)
                                        < ingredientCost.Amount));
            
            mainButtonCanvasGroup.alpha = _canBuyCurrentOutfit ? mainButtonAvailableAlpha : mainButtonUnavailableAlpha;
            purchaseLayout.SetActive(true);
            selectLayout.SetActive(false);
            
            moneyCostText.text = currentOutfit.OutfitMoneyCost.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(moneyCostLayoutRectTransform);
            
            if (currentOutfit.IngredientCosts.Count == 0)
            {
                ingredientCostsLayout.SetActive(false);
            }
            else
            {
                ingredientCostsLayout.SetActive(true);
                
                for (int i = 0; i < ingredientCostLayouts.Count; i++)
                {
                    ingredientCostLayouts[i].SetActive(false);
                }

                for (int i = 0; i < currentOutfit.IngredientCosts.Count; i++)
                {
                    IngredientCost ingredientCost = currentOutfit.IngredientCosts[i];
                    ingredientCostLayouts[i].SetActive(true);
                    ingredientCostTexts[i].text = ingredientCost.Amount.ToString();
                    ingredientCostImages[i].sprite = ingredientCost.Ingredient.iconLow;
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(ingredientCostsLayoutRectTransform);
            }
        }
    }
    
    private IEnumerator UpdateOutfitUIScrollCoroutine(OutfitButtonBehaviour outfitButton)
    {
        yield return new WaitForEndOfFrame();
        
        Vector2 endPosition = outfitScrollRect.GetPositionEnsureVisibilityVertical(outfitButton.GetComponent<RectTransform>(), outfitScrollPadding);

        outfitScrollRect.content.DOKill();
        outfitScrollRect.content.DOAnchorPosY(endPosition.y, outfitScrollDuration).SetEase(outfitScrollCurve);
    }

    private void UpdateLockUI()
    {
        // Update outfit buttons
        for (int i = 0; i < _outfitButtons.Count; i++)
        {
            OutfitButtonBehaviour outfitButton = _outfitButtons[i];
            
            if (!GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(outfitButton.OutfitSo))
            {
                outfitButton.EnableLock();
            }
            else
            {
                outfitButton.DisableLock();
            }
        }
        
        if (GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(_isCurrentRainOutfit ?
                outfitListSo.RainOutfits[_currentOutfitIndex] : outfitListSo.CasualOutfits[_currentOutfitIndex]))
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
