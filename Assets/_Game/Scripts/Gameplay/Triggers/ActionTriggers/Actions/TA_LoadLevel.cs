using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TA_LoadLevel : TriggerAction
{
    [SerializeField] private int levelBuildIndex;

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
        FadeManager.StartFadeIn(delegate { 
            FadeManager.DelayAfterFadeIn(1f, delegate { 
                isActionOnProgress = false;
                SceneManager.LoadScene(levelBuildIndex); 
            }); 
        });
    }
}
