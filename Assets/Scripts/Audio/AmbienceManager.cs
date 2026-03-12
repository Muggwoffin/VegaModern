using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AmbienceManager : MonoBehaviour
{
    public AudioMixer mixer;
    public float fadeDuration = 2.0f;
    public enum AudioZone {Street, Cafe, Kitchen}
    public AudioZone currentZone =  AudioZone.Street;
    
    void Start()
    {
        mixer.SetFloat("StreetVol", 0f);
        mixer.SetFloat("CafeVol", -80f);
        mixer.SetFloat("KitchenVol", -80f);
    }



    public void EnterCafe()
    {
        Debug.Log(currentZone);
        if  (currentZone == AudioZone.Cafe) return;
        StopAllCoroutines();
        StartCoroutine(FadeMixer("StreetVol", -80f, fadeDuration));
        StartCoroutine(FadeMixer("CafeVol", 0f, fadeDuration));
        StartCoroutine(FadeMixer("KitchenVol", -80f, fadeDuration));
        Debug.Log("Entered Cafe");
    }
    
    public void ExitCafe()
    {
        Debug.Log(currentZone);
        if  (currentZone != AudioZone.Cafe) return;
        StopAllCoroutines();
        StartCoroutine(FadeMixer("CafeVol", -80f, fadeDuration));
        StartCoroutine(FadeMixer("StreetVol", 0f, fadeDuration));
        StartCoroutine(FadeMixer("KitchenVol", -80f, fadeDuration));
        Debug.Log("Exited Cafe");
    }

    public void EnterKitchen()
    {
        Debug.Log(currentZone);
        if  (currentZone == AudioZone.Kitchen) return;
        StopAllCoroutines();
        StartCoroutine(FadeMixer("KitchenVol", 0f, fadeDuration));
        StartCoroutine(FadeMixer("CafeVol", -10f, fadeDuration));
    }
    
    public void ExitKitchen()
    {
        Debug.Log(currentZone);
        if  (currentZone != AudioZone.Kitchen) return;
        StopAllCoroutines();
        StartCoroutine(FadeMixer("KitchenVol", -80f, fadeDuration));
        StartCoroutine(FadeMixer("CafeVol", 0f, fadeDuration));
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
