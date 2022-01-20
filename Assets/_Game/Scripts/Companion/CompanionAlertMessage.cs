using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CompanionAlertMessage : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CanvasGroup messageGroup;
    [SerializeField] private TMP_Text messageText;

    [Header("Message Configs")]
    [SerializeField] private float rotationSpeed;
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

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        var lookDirection = Quaternion.LookRotation(transform.position - mainCamera.transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
    }

    private void UpdateMessage(string message)
    {
        if (isShowingMessage)
        {
            StopCoroutine(showCoroutine);
            isShowingMessage = false;
            messageGroup.gameObject.transform.DOScale(Vector3.zero, 0f);
            messageGroup.DOFade(0f, 0f);
        }

        messageText.text = message;
        showCoroutine = StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        isShowingMessage = true;

        StartCoroutine(MessageFadeIn());

        yield return new WaitForSeconds(fadeInDuration + delayBeforeFadeOut);
        StartCoroutine(MessageFadeOut(null));

        yield return new WaitForSeconds(fadeOutDuration);

        isShowingMessage = false;
    }

    public void HideMessage(float duration)
    {
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);
        
        if (gameObject.activeInHierarchy)
            StartCoroutine(MessageFadeOut(duration));
    }

    private IEnumerator MessageFadeIn()
    {
        yield return null;
        Sequence fadeIn = DOTween.Sequence();
        fadeIn.Append(messageGroup.gameObject.transform.DOScale(defaultScale, fadeInDuration));
        fadeIn.Insert(0, messageGroup.DOFade(1f, fadeIn.Duration()));
        
        fadeIn.Play();
    }

    private IEnumerator MessageFadeOut(float? optDuration)
    {
        var duration = optDuration == null? fadeOutDuration : optDuration;
        yield return null;
        Sequence fadeOut = DOTween.Sequence();
        fadeOut.Append(messageGroup.gameObject.transform.DOScale(Vector3.zero, fadeOutDuration));
        fadeOut.Insert(0, messageGroup.DOFade(0f, fadeOut.Duration()));

        fadeOut.Play();
    }

    private void OnEnable()
    {   
        TriggerAction.onFailMessageSent += UpdateMessage;
        MessageTrigger.onMessageSent += UpdateMessage;
        TA_SendUIMessage.onMessageSent += UpdateMessage;
    }

    private void OnDisable()
    {
        TriggerAction.onFailMessageSent -= UpdateMessage;
        MessageTrigger.onMessageSent -= UpdateMessage;
        TA_SendUIMessage.onMessageSent -= UpdateMessage;
    }
}
