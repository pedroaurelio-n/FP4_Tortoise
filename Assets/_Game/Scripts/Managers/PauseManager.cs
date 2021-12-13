using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private CinemachineFreeLook freeLookCamera;

    private PlayerControls playerControls;
    private bool isPaused;

    private void CheckPauseInput()
    {
        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        GameManager.canInput = false;
        GameManager.Instance.LockMouse(false);
        freeLookCamera.enabled = false;

        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        GameManager.Instance.LockMouse(true);

        freeLookCamera.enabled = true;

        pauseMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        isPaused = false;
        GameManager.canInput = true;
    }

    public void ReturnToMainMenu()
    {
        FadeManager.StartFadeIn(delegate { FadeManager.DelayAfterFadeIn(2f, delegate { SceneManager.LoadScene(0); }); });
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerActions.Pause.performed += ctx => CheckPauseInput();
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
