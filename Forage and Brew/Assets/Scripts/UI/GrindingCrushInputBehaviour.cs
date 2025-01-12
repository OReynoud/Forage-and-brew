using UnityEngine;

[SelectionBase]
public class GrindingCrushInputBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject previewInputGameObject;
    [SerializeField] private GameObject correctInputGameObject;


    public void OnEnable()
    {
        SetPreviewInput();
    }


    public void SetPreviewInput()
    {
        previewInputGameObject.SetActive(true);
        correctInputGameObject.SetActive(false);
    }
    
    public void SetCorrectInput()
    {
        previewInputGameObject.SetActive(false);
        correctInputGameObject.SetActive(true);
    }
}
