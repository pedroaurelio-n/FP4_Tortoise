using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionTrigger : MonoBehaviour
{
    public delegate void StarRemove(int value);
    public static event StarRemove onStarRemove;

    public ActionList reference;

    protected IEnumerator CheckAction(ActionList reference)
    {
        if (reference.actionList.Count == 0 || reference.actionList[0] == null || reference.actionList[0].isActionOnProgress)
                yield break;


        if (!reference.isListSequential)
        {
            if (reference.actionList[0].TryToActivateAction())
            {
                if (reference.willDeleteActions)
                {
                    if (onStarRemove != null && reference.actionList[0].willReduceStars)
                        onStarRemove(-reference.actionList[0].minimumStarsRequired);

                    reference.actionList.RemoveAt(0);
                }
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

                    if (reference.willDeleteActions)
                    {
                        if (onStarRemove != null && reference.actionList[0].willReduceStars)
                            onStarRemove(-reference.actionList[0].minimumStarsRequired);
                            
                        reference.actionList.RemoveAt(0);
                    }
                }

                else
                {
                    yield break;
                }

                yield return new WaitForSeconds(reference.DelayBetweenActions);
            }

            yield return null;
        }

        yield return null;
    }
}
