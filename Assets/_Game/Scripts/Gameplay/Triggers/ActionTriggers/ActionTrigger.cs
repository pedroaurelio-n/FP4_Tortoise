using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionTrigger : MonoBehaviour
{
    public ActionList reference;
    [SerializeField] private bool isListSequential;
    [SerializeField] private float delayBetweenActions;

    protected IEnumerator CheckAction(ActionList reference)
    {
        if (reference.actionList[0] == null || reference.actionList[0].isActionOnProgress)
                yield break;


        if (!isListSequential)
        {
            if (reference.actionList[0].TryToActivateAction())
            {
                reference.actionList.RemoveAt(0);
            }

            yield return null;
        }

        else
        {
            while (reference.actionList.Count > 0)
            {
                if (reference.actionList[0].TryToActivateAction())
                {
                    while (reference.actionList[0].isActionOnProgress)
                    {
                        yield return null;
                    }

                    reference.actionList.RemoveAt(0);
                }

                else
                {
                    yield break;
                }

                yield return new WaitForSeconds(delayBetweenActions);
            }

            yield return null;
        }

        yield return null;
    }
}
