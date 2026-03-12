using UnityEngine;
using UnityEngine.Audio;

public class AudioMuffle : MonoBehaviour
{
    public AudioMixer mixer;

    public Transform player;
    public LayerMask wallLayer;
    [Header("Filter Settings")] public float clearFrequency = 5000f;
    public float muffledFrequency = 500f;
    public float transitionSpeed = 5f;

    public string mixerParameter = "RadioMuffle";
    
    private float targetFreq;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetFreq = clearFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - transform.position;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, direction.magnitude, wallLayer))
        {
            targetFreq = muffledFrequency;
            Debug.DrawRay(transform.position, direction, Color.red);
        }
        else
        {
            targetFreq = clearFrequency;
            Debug.DrawRay(transform.position, direction, Color.green);
        }
        float currentFreq;
        mixer.GetFloat(mixerParameter, out currentFreq);
        float nextFreq = Mathf.Lerp(currentFreq, targetFreq, Time.deltaTime * transitionSpeed);
        mixer.SetFloat(mixerParameter, nextFreq);
    }
}
