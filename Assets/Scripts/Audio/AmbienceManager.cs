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

    //remove this debug toggle and eventually switch back to the new input system
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnterCafe();
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExitCafe();
            
        }
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

    IEnumerator FadeMixer(string param, float targetDb, float duration)
    {
        float currentDb;
        mixer.GetFloat(param, out currentDb);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newDb = Mathf.Lerp(currentDb, targetDb, elapsed / duration);
            mixer.SetFloat(param, newDb);
            yield return null;
        }
        mixer.SetFloat(param, targetDb);
    }

}
