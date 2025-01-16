using UnityEngine;

public class TutorialZoneTrigger : MonoBehaviour
{
    public int triggerID;
    private void OnTriggerEnter(Collider other)
    {
        TutorialManager.instance.NotifyFromZoneTrigger(triggerID);
        
    }
}
