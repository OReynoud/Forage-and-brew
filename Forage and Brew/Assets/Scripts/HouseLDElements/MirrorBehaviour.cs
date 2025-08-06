using System.Collections;
using System.Linq;
using UnityEngine;

public class MirrorBehaviour : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject localInputCanvas;
    [SerializeField] private GameObject mirrorInterfaceCanvas;
    [SerializeField] private CharacterOutfitListSo outfitListSo;
    [SerializeField] private Transform locationToWalk;
    
    [Header("Camera Settings")]
    [SerializeField] private CameraPreset mirrorCamera;
    [SerializeField] private float mirrorCameraTransitionDuration = 0.5f;
    [SerializeField] private CameraPreset usualCamera;
    [SerializeField] private float usualCameraTransitionDuration = 0.5f;
    
    [Header("Change Outfit")]
    [SerializeField] private float outfitChangeDelay = 0.1f;
    
    // Global variables
    private bool _isUsingMirror;
    
    // Outfit variables
    private int _currentOutfitIndex;
    private int _selectedOutfitIndex;
    
    // Animator hashes
    private static readonly int DoNo = Animator.StringToHash("DoNo");


    private void Start()
    {
        localInputCanvas.SetActive(false);
        mirrorInterfaceCanvas.SetActive(false);
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
    }

    public void SelectOutfit()
    {
        if (!_isUsingMirror) return;
        
        if (!GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Contains(outfitListSo.Outfits[_currentOutfitIndex])) return;
        
        _selectedOutfitIndex = _currentOutfitIndex;
        
        // TODO: UI behaviour to show outfit selection success
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
        
        StartCoroutine(UnlockOutfitCoroutine());
    }
    
    private IEnumerator UnlockOutfitCoroutine()
    {
        // Wait for the next frame
        yield return new WaitForEndOfFrame();
        
        // Unlock the outfit
        GameDontDestroyOnLoadManager.Instance.UnlockedOutfits.Add(outfitListSo.Outfits[_currentOutfitIndex]);
    }

    private IEnumerator ChangeOutfit(CharacterOutfitSo outfit)
    {
        CharacterVfxManager.Instance.PlayPuffVfx();
        
        yield return new WaitForSeconds(outfitChangeDelay);
        
        CharacterAnimManager.instance.SetOutfit(outfit);
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
