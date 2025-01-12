using UnityEngine;

public class ConfirmationCircleBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject previewCircleGameObject;
    [SerializeField] private GameObject currentCircleGameObject;
    [SerializeField] private GameObject rightCircleGameObject;


    public void OnEnable()
    {
        SetPreviewCircle();
    }


    public void SetPreviewCircle()
    {
        previewCircleGameObject.SetActive(true);
        currentCircleGameObject.SetActive(false);
        rightCircleGameObject.SetActive(false);
    }
    
    public void SetCurrentCircle()
    {
        previewCircleGameObject.SetActive(false);
        currentCircleGameObject.SetActive(true);
        rightCircleGameObject.SetActive(false);
    }
    
    public void SetRightCircle()
    {
        previewCircleGameObject.SetActive(false);
        currentCircleGameObject.SetActive(false);
        rightCircleGameObject.SetActive(true);
    }
}
