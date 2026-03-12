using UnityEngine;

public class CafeEntranceTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 public AmbienceManager ambienceManager;

 void OnTriggerEnter(Collider other)
 {
     if (other.CompareTag("Player"))
     {
         ambienceManager.EnterCafe();
     }
 }

 void OnTriggerExit(Collider other)
 {
     if (other.CompareTag("Player"))
     {
         ambienceManager.ExitCafe();
     }
     
 }
}
