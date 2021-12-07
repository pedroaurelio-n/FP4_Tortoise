using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TA_LoadLevel : TriggerAction
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;

    public override bool TryToActivateAction()
    {
        if (CanActivateAction())
            ActivateAction();
        else
            Debug.Log("Couldn't perform action");

        return CanActivateAction();
    }

    protected override bool CanActivateAction()
    {
        return true;
    }

    protected override void ActivateAction()
    {
        isActionOnProgress = true;
        fadeImage.DOFade(1f, fadeDuration).OnComplete(delegate {fadeImage.DOFade(1f, fadeDuration).OnComplete(delegate {SceneManager.LoadScene(2); });});
    }
}
