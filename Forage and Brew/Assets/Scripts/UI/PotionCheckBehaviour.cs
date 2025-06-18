using UnityEngine;

public class PotionCheckBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject checkMarkGameObject;
    
    
    public void EnableCheckMark()
    {
        checkMarkGameObject.SetActive(true);
    }
    
    public void DisableCheckMark()
    {
        checkMarkGameObject.SetActive(false);
    }
}
