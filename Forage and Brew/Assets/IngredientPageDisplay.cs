using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IngredientPageDisplay : MonoBehaviour
{
    public Image dissolveImage;
    public AnimationCurve dissolveCurve;

    public IngredientValuesSo associatedIngredient;

    public float dissolveTimer;

    public float animationTime;

    public bool doDissolve;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void StartDissolve()
    {
        animationTime = dissolveCurve.keys[^1].time;
        doDissolve = true;
        dissolveTimer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (!doDissolve) return;
        
        dissolveTimer += Time.deltaTime;
            
        dissolveImage.material.SetFloat(Ex.CutoffHeight, dissolveCurve.Evaluate(dissolveTimer));

        if (dissolveTimer > animationTime)
        {
            enabled = false;
        }
    }
}
