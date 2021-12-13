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
    [SerializeField] private float waitDuration;
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

        yield return new WaitForSecondsRealtime(waitDuration);

        gameOverScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(waitDuration);

        FadeManager.StartFadeIn(delegate { FadeManager.DelayAfterFadeIn(1f, delegate { SceneManager.LoadScene(0); }); });
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
