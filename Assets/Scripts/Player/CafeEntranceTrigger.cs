using UnityEngine;

public class CafeEntranceTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 public AmbienceManager ambienceManager;

 private float cooldown = 1.0f;
 private float lastEnterTime = -999f;
 private float lastExitTime = -999f;


 void OnTriggerEnter(Collider other)
 {
     Debug.Log("CAFE TRIGGER HIT BY: " + other.gameObject.name);
     if (other.CompareTag("Player") && Time.time > lastEnterTime + cooldown)
     {
         lastEnterTime = Time.time;
         ambienceManager.EnterCafe();
     }
 }

 void OnTriggerExit(Collider other)
 {
     if (other.CompareTag("Player") && Time.time > lastExitTime + cooldown)
     {
         lastExitTime = Time.time;
         ambienceManager.ExitCafe();
     }
     
 }
}
