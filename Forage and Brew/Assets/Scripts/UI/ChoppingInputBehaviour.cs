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
        previewInputImage.enabled = true;
        currentInputImage.enabled = false;
        correctInputImage.enabled = false;
        wrongInputImage.enabled = false;
    }
    
    public void SetCurrentInput()
    {
        previewInputImage.enabled = false;
        currentInputImage.enabled = true;
        correctInputImage.enabled = false;
        wrongInputImage.enabled = false;
    }
    
    public void SetRightInput()
    {
        previewInputImage.enabled = false;
        currentInputImage.enabled = false;
        correctInputImage.enabled = true;
        wrongInputImage.enabled = false;
    }
    
    public void SetWrongInput()
    {
        previewInputImage.enabled = false;
        currentInputImage.enabled = false;
        correctInputImage.enabled = false;
        wrongInputImage.enabled = true;
    }
    
    
    public void SetInputSprite(int index)
    {
        previewInputImage.sprite = choppingInputSprites[index - 1];
        currentInputImage.sprite = choppingInputSprites[index - 1];
        correctInputImage.sprite = choppingInputSprites[index - 1];
        wrongInputImage.sprite = choppingInputSprites[index - 1];
    }
}
