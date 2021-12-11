using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CompanionGlideController : MonoBehaviour
{
    [Header("Companion References")]
    [SerializeField] private CompanionMain companionMain;

    [Header("Glide Configs")]
    [SerializeField] private Ease glideEase;
    [SerializeField] private float glideMovementDuration;

    private bool isOnGlide;
    private Coroutine startGliding;
    private Coroutine returnFromGliding;
    private Tween scaleToZero;

    public void HandleGliding()
    {
        if (companionMain.playerMovement.isGliding && !isOnGlide)
        {
            if (returnFromGliding != null)
                StopCoroutine(returnFromGliding);

            companionMain.companionAlertMessage.HideMessage(glideMovementDuration);
            startGliding = StartCoroutine(ActivateGlideMovement());
        }

        if (!companionMain.playerMovement.isGliding && isOnGlide)
        {
            if (startGliding != null)
            {
                StopCoroutine(startGliding);
            }

            returnFromGliding = StartCoroutine(DeactivateGlideMovement());
        }
    }

    private IEnumerator ActivateGlideMovement()
    {
        companionMain.companionSocketTrail.Clear();
        isOnGlide = true;
        scaleToZero = companionMain.companionEntityMesh.transform.DOScale(Vector3.zero, glideMovementDuration).SetEase(glideEase);
        companionMain.companionEntityMesh.transform.DOMove(companionMain.playerCompanionSocket.position, glideMovementDuration).SetEase(glideEase);

        yield return new WaitForSeconds(glideMovementDuration+0.02f);

        companionMain.companionEntityMesh.SetActive(false);
        companionMain.companionSocketTrail.enabled = true;
    }

    private IEnumerator DeactivateGlideMovement()
    {
        isOnGlide = false;
        scaleToZero.Kill();
        transform.position = companionMain.companionDesiredPlacement.position;
        companionMain.companionEntityMesh.transform.position = companionMain.playerCompanionSocket.position;
        companionMain.companionEntityMesh.SetActive(true);
        companionMain.companionEntityMesh.gameObject.GetComponent<TrailRenderer>().Clear();
        companionMain.companionSocketTrail.enabled = false;

        companionMain.companionEntityMesh.transform.DOScale(Vector3.one, glideMovementDuration).SetEase(glideEase);
        companionMain.companionEntityMesh.transform.DOMove(transform.position, glideMovementDuration).SetEase(glideEase);

        yield return new WaitForSeconds(glideMovementDuration+0.01f);

        companionMain.companionEntityMesh.SetActive(true);
        companionMain.companionSocketTrail.enabled = false;
    }
}
