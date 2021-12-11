using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CompanionHover : MonoBehaviour
{
    [Header("Companion References")]
    [SerializeField] private CompanionMain companionMain;

    [Header("Hover Configs")]
    [SerializeField] private float upHoverOffset;
    [SerializeField] private float downHoverOffset;
    [SerializeField] private float hoverDuration;
    [SerializeField] private Ease hoverEase;

    private float _startYPosition;

    private void Start()
    {
        _startYPosition = companionMain.companionDesiredPlacement.localPosition.y;
        StartCoroutine(StartHover());
    }

    private IEnumerator StartHover()
    {
        while (true)
        {
            companionMain.companionDesiredPlacement.DOLocalMoveY(_startYPosition + downHoverOffset, hoverDuration).SetEase(hoverEase);
            yield return new WaitForSeconds(hoverDuration);

            companionMain.companionDesiredPlacement.DOLocalMoveY(_startYPosition + upHoverOffset, hoverDuration).SetEase(hoverEase);
            yield return new WaitForSeconds(hoverDuration);

            yield return null;
        }
    }
}
