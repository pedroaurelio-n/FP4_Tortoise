using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTrigger : MonoBehaviour
{
    [SerializeField] private ActionList reference;
    [SerializeField] private bool isListSequential;
    [SerializeField] private float delayBetweenActions;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMain player))
        {
            StartCoroutine(TryToActivateAction(reference));
        }
    }

    private IEnumerator TryToActivateAction(ActionList reference)
    {
        if (!isListSequential)
        {
            if (reference.actionList[0] == null)
                yield break;

            reference.actionList[0].TryToActivateAction();
            reference.actionList.RemoveAt(0);

            yield return null;
        }

        else
        {
            while (reference.actionList.Count > 0)
            {
                reference.actionList[0].TryToActivateAction();

                while (reference.actionList[0].isActionOnProgress)
                {
                    yield return null;
                }

                reference.actionList.RemoveAt(0);

                yield return new WaitForSeconds(delayBetweenActions);
            }

            yield return null;
        }

        yield return null;
    }
}