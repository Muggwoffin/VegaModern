using UnityEngine;
using UnityEngine.Audio;

//Simulates sound being muffled behind walls
//Casts a ray from the item playing audio to the player to check if there are any obstacles between them
//Controls a low-pass filter cut off so we only hear a low bass hum when an obstacle (like a wall) is detected between player and sound source
public class AudioMuffle : MonoBehaviour
{
    public AudioMixer mixer;

    public Transform player;
    public LayerMask wallLayer;
   
    [Header("Filter Settings")] public float clearFrequency = 5000f;
    public float muffledFrequency = 500f;
    public float transitionSpeed = 5f;

    public string mixerParameter = "RadioMuffle"; //Name of the exposed mixer parameter
    
    private float targetFreq;
    

    void Start()
    {
        //If there are no walls then everything starts clear
        targetFreq = clearFrequency;
    }

    void Update()
    {
        // Work out direction and the distance between the player and the sound source
        Vector3 direction = player.position - transform.position;
        RaycastHit hit;

        //Draw a ray to see if a wall blocks the path
        if (Physics.Raycast(transform.position, direction, out hit, direction.magnitude, wallLayer))
        {
            //We've hit something! Alright muffle the frequency
            targetFreq = muffledFrequency;
            Debug.DrawRay(transform.position, direction, Color.red);
        }
        else
        {
            //Otherwise the frequency is clear
            targetFreq = clearFrequency;
            Debug.DrawRay(transform.position, direction, Color.green);
        }
        //Read the current cutoff value
        float currentFreq;
        mixer.GetFloat(mixerParameter, out currentFreq);
        
        //SMoothly move from current to target frequency over time, determined by our transition speed
        float nextFreq = Mathf.Lerp(currentFreq, targetFreq, Time.deltaTime * transitionSpeed);
        mixer.SetFloat(mixerParameter, nextFreq);
    }
}
