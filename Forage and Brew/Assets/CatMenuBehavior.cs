using UnityEngine;

public class CatMenuBehavior : MonoBehaviour
{
    public Transform codexSpine;
    public Transform pawsParent;

    public float parentChangeTime;
    private bool checker = false;

    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (checker)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer >= parentChangeTime)
        {
            codexSpine.SetParent(pawsParent);
        }
    }
}
