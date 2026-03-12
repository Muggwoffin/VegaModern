using UnityEngine;

public class KitchenEntranceTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AmbienceManager ambienceManager;

    private float cooldown = 1.0f;
    private float lastTriggerTime = -999f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time > lastTriggerTime + cooldown)
        {
            lastTriggerTime = Time.time;
            ambienceManager.EnterKitchen();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && Time.time > lastTriggerTime + cooldown)
        {
            lastTriggerTime = Time.time;
            ambienceManager.ExitKitchen();
        }
     
    }
}

