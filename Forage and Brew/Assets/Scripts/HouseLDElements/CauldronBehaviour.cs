using System.Collections.Generic;
using UnityEngine;

public class CauldronBehaviour : Singleton<CauldronBehaviour>
{
    [SerializeField] private GameObject interactInputCanvasGameObject;
    [SerializeField] private GameObject buttonAGameObject;
    [SerializeField] private GameObject buttonYGameObject;
    [field: SerializeField] public Transform SpoonTransform { get; private set; }

    public List<IngredientValuesSo> Ingredients { get; } = new();

    
    private void Start()
    {
        interactInputCanvasGameObject.SetActive(false);
    }


    public void EnableInteract(bool areHandsFull)
    {
        buttonAGameObject.SetActive(areHandsFull);
        buttonYGameObject.SetActive(!areHandsFull);
        
        interactInputCanvasGameObject.SetActive(true);
    }
    
    public void DisableInteract(bool isStillNear = false)
    {
        if (isStillNear)
        {
            buttonAGameObject.SetActive(!buttonAGameObject.activeSelf);
            buttonYGameObject.SetActive(!buttonYGameObject.activeSelf);
        }
        else
        {
            interactInputCanvasGameObject.SetActive(false);
        }
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out StirHapticChallengeManager stirHapticChallengeManager))
        {
            characterInteractController.CurrentNearCauldron = this;
            stirHapticChallengeManager.CurrentCauldron = this;
            EnableInteract(characterInteractController.AreHandsFull);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterInteractController characterInteractController) &&
            other.TryGetComponent(out StirHapticChallengeManager stirHapticChallengeManager) &&
            characterInteractController.CurrentNearCauldron == this)
        {
            characterInteractController.CurrentNearCauldron = null;
            stirHapticChallengeManager.CurrentCauldron = null;
            DisableInteract();
        }
    }
}
