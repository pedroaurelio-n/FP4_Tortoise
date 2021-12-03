using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CompanionAlertMessage : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private CanvasGroup messageGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float delayBeforeFadeOut;
    [SerializeField] private float fadeOutDuration;

    private Vector3 defaultScale;

    private bool isShowingMessage;
    private Coroutine showCoroutine;

    private void Start()
    {
        defaultScale = messageGroup.gameObject.transform.localScale;

        messageGroup.alpha = 0f;
        messageGroup.gameObject.transform.DOScale(Vector3.zero, 0f);
    }

    private void Update()
    {
        var lookDirection = Quaternion.LookRotation(transform.position - _mainCamera.transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
    }

    private void UpdateMessage(string message)
    {
        if (!isShowingMessage)
        {
            isShowingMessage = true;
            messageText.text = message;
            showCoroutine = StartCoroutine(ShowMessage());
        }

        else
        {
            StopCoroutine(showCoroutine);
            isShowingMessage = false;
            messageGroup.gameObject.transform.DOScale(Vector3.zero, 0f);
            messageGroup.DOFade(0f, 0f);

            isShowingMessage = true;
            messageText.text = message;
            showCoroutine = StartCoroutine(ShowMessage());
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

    private void OnEnable()
    {   
        TriggerAction.onFailMessageSent += UpdateMessage;
        MessageTrigger.onMessageSent += UpdateMessage;
    }

    private void OnDisable()
    {
        TriggerAction.onFailMessageSent -= UpdateMessage;
        MessageTrigger.onMessageSent -= UpdateMessage;
    }
}
