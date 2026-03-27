using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeadphoneNotice : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;

    [Header("Timing")] public float fadeInDuration = 1f;
    public float displayDuration = 3f;
    public float fadeOutDuration = 1.5f;

    private void Start()
    {
        canvas.alpha = 0;
        StartCoroutine(ShowNotice());
    }

    private IEnumerator ShowNotice()
    {
        //fades in
        yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));
        
        //Holds the UI in place
        yield return new WaitForSeconds(displayDuration);
        
        //fades out
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));
        
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        //Create a gradual fade
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvas.alpha = to;
    }
}
