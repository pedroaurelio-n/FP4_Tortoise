using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UI_Message : MonoBehaviour
{
    [SerializeField] private CanvasGroup messageGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float delayBeforeFadeOut;
    [SerializeField] private float fadeOutDuration;

    private Vector3 defaultScale;

    private bool isShowingMessage;

    private void Start()
    {
        defaultScale = messageGroup.gameObject.transform.localScale;

        messageGroup.alpha = 0f;
        messageGroup.gameObject.transform.DOScale(Vector3.zero, 0f);
    }

    private void UpdateMessage(string message)
    {
        if (!isShowingMessage)
        {
            isShowingMessage = true;
            messageText.text = message;
            StartCoroutine(ShowMessage());
        }
    }

    private IEnumerator ShowMessage()
    {
        Sequence fadeIn = DOTween.Sequence();
        fadeIn.Append(messageGroup.gameObject.transform.DOScale(defaultScale, fadeInDuration));
        fadeIn.Insert(0, messageGroup.DOFade(1f, fadeIn.Duration()));
        
        fadeIn.Play();

        yield return new WaitForSeconds(fadeInDuration + delayBeforeFadeOut);

        Sequence fadeOut = DOTween.Sequence();
        fadeOut.Append(messageGroup.gameObject.transform.DOScale(Vector3.zero, fadeOutDuration));
        fadeOut.Insert(0, messageGroup.DOFade(0f, fadeOut.Duration()));

        fadeOut.Play();

        yield return new WaitForSeconds(fadeOutDuration);

        isShowingMessage = false;
    }
}
