using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private Image fadeImageReference;
    [SerializeField] private float fadeDurationValue;
    [SerializeField] private float fadeDelayValue;

    [SerializeField] private bool startWithFadeOut;
    [SerializeField] private bool startHasDelay;

    private static Image fadeImage;
    private static float fadeDuration;

    private void Awake()
    {        
        fadeImage = fadeImageReference;
        fadeDuration = fadeDurationValue;

        fadeImage.raycastTarget = true;

        if (startWithFadeOut)
        {
            if (startHasDelay)
            {
                fadeImage.DOFade(1f, 0f);
                GameManager.canInput = false;
                DelayAfterFadeIn(fadeDelayValue, delegate{ StartFadeOut(null); });
            }
            else
            {
                fadeImage.DOFade(1f, 0f);
                StartFadeOut(null);
            }
        }
        else
        {
            fadeImage.DOFade(0f, 0f);
        }
    }

    public static void StartFadeIn(TweenCallback action)
    {
        fadeImage.DOFade(1f, fadeDuration).SetUpdate(true).OnComplete(action);
        GameManager.canInput = false;
    }

    public static void StartFadeOut(TweenCallback action)
    {
        fadeImage.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(action);
        GameManager.canInput = true;
        fadeImage.raycastTarget = false;
    }

    public static void DelayAfterFadeIn(float duration, TweenCallback action)
    {
        fadeImage.DOFade(1f, fadeDuration).SetUpdate(true).OnComplete(action);
    }
}
