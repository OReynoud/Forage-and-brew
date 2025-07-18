using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialDissolveBehavior : MonoBehaviour
{
    public string dissolveID;


    public Image DissolveImage;
    public bool autoAdd;
    [ReadOnly]public bool doDissolve;
    private float dissolveTimer;
    public AnimationCurve animCurveDissolve;
    private bool isDissolved;
    public bool pairedDissolve;
    [ShowIf("pairedDissolve")] public TutorialDissolveBehavior pairedBehavior;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CodexContentManager.instance.tutorialDissolves.Add(dissolveID, this);
        if (autoAdd)
        {
            CodexContentManager.instance.tutorialDissolvesToCheck.Insert(1, dissolveID);
        }
    }

    public void StartDissolve()
    {
        Material mat = Instantiate(DissolveImage.material);
        DissolveImage.material.SetFloat(Ex.CutoffHeight, 0);
        DissolveImage.material = mat;


        doDissolve = true;


        AutoFlip.instance.ControledBook.discoveryAudio.Play();
    }

    private void Update()
    {
        if (!doDissolve) return;

        dissolveTimer += Time.deltaTime;

        DissolveImage.material.SetFloat(Ex.CutoffHeight, animCurveDissolve.Evaluate(dissolveTimer));

        if (dissolveTimer > animCurveDissolve.keys[^1].time)
        {
            doDissolve = false;
            AutoFlip.instance.isDissolving = false;
            isDissolved = true;
            
            //CodexContentManager.instance.tutorialDissolves.Remove(dissolveID);
            Destroy(gameObject,4.0f);
        }
    }

}