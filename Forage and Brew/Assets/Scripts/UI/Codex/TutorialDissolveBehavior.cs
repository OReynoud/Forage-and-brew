using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDissolveBehavior : MonoBehaviour
{
    public string dissolveID;


    public Image DissolveImage;
    public bool autoAdd;
    [ReadOnly]public bool doDissolve;
    private float dissolveTimer;
    public AnimationCurve animCurveDissolve;
    public bool pairedDissolve;
    public int pageToCheck;
    [ShowIf("pairedDissolve")] public TutorialDissolveBehavior pairedBehavior;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Material mat = Instantiate(DissolveImage.material);
        
        if (GameDontDestroyOnLoadManager.Instance.codexIsUnlocked)
        {
            mat.SetFloat(Ex.CutoffHeight, 1);
            DissolveImage.material = mat;
            
            return;
        }

        mat.SetFloat(Ex.CutoffHeight, 0);
        DissolveImage.material = mat;
        
        CodexContentManager.instance.tutorialDissolves.Add(dissolveID, this);
        
        if (autoAdd)
        {
            CodexContentManager.instance.tutorialDissolvesToCheck.Insert(1, dissolveID);
        }
    }

    public void StartDissolve()
    {
        Material mat = Instantiate(DissolveImage.material);
        mat.SetFloat(Ex.CutoffHeight, 0);
        DissolveImage.material = mat;
        
        doDissolve = true;
        
        AutoFlip.instance.ControledBook.discoveryAudio.Play();
    }

    private void Update()
    {
        if (!doDissolve) return;

        dissolveTimer += Time.deltaTime;

        Material mat = Instantiate(DissolveImage.material);
        mat.SetFloat(Ex.CutoffHeight, animCurveDissolve.Evaluate(dissolveTimer));
        DissolveImage.material = mat;

        if (dissolveTimer > animCurveDissolve.keys[^1].time)
        {
            doDissolve = false;
            AutoFlip.instance.isDissolving = false;
            
            //CodexContentManager.instance.tutorialDissolves.Remove(dissolveID);
            Destroy(gameObject,4.0f);
        }
    }

}