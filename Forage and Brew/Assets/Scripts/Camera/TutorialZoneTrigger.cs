using UnityEngine;

public class TutorialZoneTrigger : MonoBehaviour
{
    public string triggerID;
    private void OnTriggerEnter(Collider other)
    {
        TutorialManager.instance.NotifyFromZoneTrigger(triggerID);
        
    }
}
