using UnityEngine;

public class PotionCrateAnimatorBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject lidCanvasGameObject;
    
    
    public void DisableLidCanvas()
    {
        lidCanvasGameObject.SetActive(false);
    }
}
