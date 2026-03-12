using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AmbienceManager : MonoBehaviour
{
    public AudioMixer mixer;
    public float fadeDuration = 2.0f;
    void Start()
    {
        mixer.SetFloat("StreetVol", 0f);
        mixer.SetFloat("CafeVol", -80f);
    }

    public void EnterCafe()
    {
        StartCoroutine(FadeMixer("StreetVol", -80f, fadeDuration));
        StartCoroutine(FadeMixer("CafeVol", 0f, fadeDuration));
    }
    
    public void ExitCafe()
    {
        StartCoroutine(FadeMixer("CafeVol", -80f, fadeDuration));
        StartCoroutine(FadeMixer("StreetVol", 0f, fadeDuration));
    }
    
    

}
