using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoppingInputBehaviour : MonoBehaviour
{
    [SerializeField] private Image previewInputImage;
    [SerializeField] private Image currentInputImage;
    [SerializeField] private Image correctInputImage;
    [SerializeField] private Image wrongInputImage;
    [SerializeField] private List<Sprite> choppingInputSprites;


    public void OnEnable()
    {
        SetPreviewInput();
    }


    public void SetPreviewInput()
    {
        previewInputImage.gameObject.SetActive(true);
        currentInputImage.gameObject.SetActive(false);
        correctInputImage.gameObject.SetActive(false);
        wrongInputImage.gameObject.SetActive(false);
    }
    
    public void SetCurrentInput()
    {
        previewInputImage.gameObject.SetActive(false);
        currentInputImage.gameObject.SetActive(true);
        correctInputImage.gameObject.SetActive(false);
        wrongInputImage.gameObject.SetActive(false);
    }
    
    public void SetRightInput()
    {
        previewInputImage.gameObject.SetActive(false);
        currentInputImage.gameObject.SetActive(false);
        correctInputImage.gameObject.SetActive(true);
        wrongInputImage.gameObject.SetActive(false);
    }
    
    public void SetWrongInput()
    {
        previewInputImage.gameObject.SetActive(false);
        currentInputImage.gameObject.SetActive(false);
        correctInputImage.gameObject.SetActive(false);
        wrongInputImage.gameObject.SetActive(true);
    }
    
    
    public void SetInputSprite(int index)
    {
        previewInputImage.sprite = choppingInputSprites[index - 1];
        currentInputImage.sprite = choppingInputSprites[index - 1];
        correctInputImage.sprite = choppingInputSprites[index - 1];
        wrongInputImage.sprite = choppingInputSprites[index - 1];
    }
}
