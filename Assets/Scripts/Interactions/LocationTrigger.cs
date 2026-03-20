using UnityEngine;

// A Switch statement controlled by an enumerated list that determines where the player is and pings the ambience manager to control the audio to reflec this
public class LocationTrigger : MonoBehaviour
{
    public enum Location {Street, Cafe, Kitchen}

    public Location destination;
    public AmbienceManager ambienceManager;

    private float lastTriggerTime;
    private float cooldown = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time > lastTriggerTime + cooldown)
            {
            lastTriggerTime = Time.time;

            switch (destination)
                {
                case Location.Street:
                    ambienceManager.ExitCafe();
                    break;
                case Location.Cafe:
                    ambienceManager.EnterCafe();
                    break;
                case Location.Kitchen:
                    ambienceManager.EnterKitchen();
                    break;
                }
            
            }
        Debug.Log("Transitioning to " + destination);
    }
}
