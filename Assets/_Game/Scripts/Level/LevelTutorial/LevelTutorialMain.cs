using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelTutorialMain : MonoBehaviour
{
    [SerializeField] private List<GameObject> unpoweredEnemies;
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private GameObject elevatorCollider;
    [SerializeField] private List<GameObject> elevatorButtons;
    [SerializeField] private List<Light> lightComponents;
    [SerializeField] private List<GameObject> firstPartMessages;
    [SerializeField] private List<GameObject> secondPartMessages;

    public void ActivateTransition()
    {
        GameManager.canInput = false;
        FadeManager.StartFadeIn(delegate { StartCoroutine(ActivatePower()); });
    }

    private IEnumerator ActivatePower()
    {
        yield return null;

        foreach (GameObject enemyobject in unpoweredEnemies)
        {
            enemyobject.SetActive(false);
        }

        foreach (GameObject enemyobject in enemies)
        {
            enemyobject.SetActive(true);
        }

        elevatorCollider.SetActive(false);

        elevatorButtons[0].SetActive(false);
        elevatorButtons[1].SetActive(true);

        foreach (Light light in lightComponents)
        {
            light.enabled = true;
        }

        foreach (GameObject message in firstPartMessages)
        {
            message.SetActive(false);
        }

        foreach (GameObject message in secondPartMessages)
        {
            message.SetActive(true);
        }

        FadeManager.DelayAfterFadeIn(2f, delegate { FadeManager.StartFadeOut(null); });
    }
}
