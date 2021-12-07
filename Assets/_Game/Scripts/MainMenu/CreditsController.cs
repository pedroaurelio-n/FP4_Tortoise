using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;

    public void ReturnToMainMenu()
    {
        fadeImage.DOFade(1f, fadeDuration).OnComplete(delegate {fadeImage.DOFade(1f, fadeDuration).OnComplete(delegate { GameManager.Instance.LockMouse(false); SceneManager.LoadScene(0); });});
    }
}
