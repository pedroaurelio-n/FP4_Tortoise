using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject grayImage;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;
    [SerializeField] private AudioSource musicManager;

    private void ActivateGameOver()
    {
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        grayImage.SetActive(true);
        musicManager.Stop();
        Time.timeScale = 0f;
        GameManager.canInput = false;
        GameManager.Instance.LockMouse(true);

        yield return new WaitForSecondsRealtime(fadeDuration);

        gameOverScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(fadeDuration);

        fadeImage.DOFade(1f, fadeDuration).SetUpdate(true);
        yield return new WaitForSecondsRealtime(fadeDuration);
        yield return new WaitForSecondsRealtime(fadeDuration);
        
        GameManager.Instance.LockMouse(false);
        SceneManager.LoadScene(0);
    }

    private void OnEnable()
    {
        PlayerHealth.onGameOver += ActivateGameOver;
    }

    private void OnDisable()
    {
        PlayerHealth.onGameOver -= ActivateGameOver;
    }
}
