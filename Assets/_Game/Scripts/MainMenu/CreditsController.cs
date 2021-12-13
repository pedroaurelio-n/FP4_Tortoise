using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class CreditsController : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.LockMouse(false);
    }

    public void ReturnToMainMenu()
    {
        FadeManager.StartFadeIn(delegate { FadeManager.DelayAfterFadeIn(1f, delegate { SceneManager.LoadScene(0); }); });
    }
}
