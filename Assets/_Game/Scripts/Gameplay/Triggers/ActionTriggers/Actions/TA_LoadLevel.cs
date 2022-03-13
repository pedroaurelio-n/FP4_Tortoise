using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TA_LoadLevel : TriggerAction
{
    [SerializeField] private int levelBuildIndex;

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
