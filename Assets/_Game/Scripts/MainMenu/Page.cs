using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum EntryMode
{
    DO_NOTHING,
    SLIDE,
    ZOOM,
    FADE
}

public enum EntryDirection
{
    NONE,
    UP,
    RIGHT,
    DOWN,
    LEFT
}

[RequireComponent(typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class Page : MonoBehaviour
{
    public bool ExitOnNewPagePush = false;
    public GameObject onCancelSelectedObject;

    [SerializeField] private float animationDuration;
    [SerializeField] private AudioClip entryClip;
    [SerializeField] private AudioClip exitClip;
    [SerializeField] private EntryMode entryMode = EntryMode.DO_NOTHING;
    [SerializeField] private EntryDirection entryDirection = EntryDirection.NONE;
    [SerializeField] private EntryMode exitMode = EntryMode.DO_NOTHING;
    [SerializeField] private EntryDirection exitDirection = EntryDirection.NONE;

    [SerializeField] private UnityEvent PrePushAction;
    [SerializeField] private UnityEvent PostPushAction;
    [SerializeField] private UnityEvent PrePopAction;
    [SerializeField] private UnityEvent PostPopAction;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    [HideInInspector] public bool isAnimating;
    private Coroutine _audioCoroutine;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Enter(bool playAudio)
    {
        PrePushAction?.Invoke();

        switch (entryMode)
        {
            case EntryMode.ZOOM:
                ZoomIn(playAudio);
                break;

            case EntryMode.FADE:
                FadeIn(playAudio);
                break;

            default: throw new System.Exception("Not Implemented");
        }
    }

    public void Exit(bool playAudio)
    {
        PrePopAction?.Invoke();

        switch (exitMode)
        {
            case EntryMode.ZOOM:
                ZoomOut(playAudio);
                break;

            case EntryMode.FADE:
                FadeOut(playAudio);
                break;

            default: throw new System.Exception("Not Implemented");
        }
    }

    private void ZoomIn(bool playAudio)
    {
        if (isAnimating)
            return;

        isAnimating = true;
        _rectTransform.DOScale(Vector3.one, animationDuration).OnComplete(delegate { isAnimating = false; PostPushAction?.Invoke(); });

        PlayEntryClip(playAudio);
    }

    private void FadeIn(bool playAudio)
    {
        if (isAnimating)
            return;

        isAnimating = true;
        _canvasGroup.DOFade(1f, animationDuration).OnComplete(delegate { isAnimating = false; PostPushAction?.Invoke(); });
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        PlayEntryClip(playAudio);
    }

    private void ZoomOut(bool playAudio)
    {
        if (isAnimating)
            return;

        isAnimating = true;
        _rectTransform.DOScale(Vector3.zero, animationDuration).OnComplete(delegate { isAnimating = false; PostPopAction?.Invoke(); });

        PlayExitClip(playAudio);
    }

    private void FadeOut(bool playAudio)
    {
        if (isAnimating)
            return;

        isAnimating = true;
        _canvasGroup.DOFade(0f, animationDuration).OnComplete(delegate { isAnimating = false; PostPopAction?.Invoke(); });
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        PlayExitClip(playAudio);
    }

    private void PlayEntryClip(bool playAudio)
    {
        if (playAudio && entryClip != null)
        {
            AudioManager.Instance.PlayAudio(entryClip);
        }
    }

    private void PlayExitClip(bool playAudio)
    {
        if (playAudio && exitClip != null)
        {
            AudioManager.Instance.PlayAudio(exitClip);
        }
    }
}
