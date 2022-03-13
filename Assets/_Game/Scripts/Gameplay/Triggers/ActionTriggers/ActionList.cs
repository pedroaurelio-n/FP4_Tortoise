using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionList : MonoBehaviour
{
    public List<TriggerAction> actionList;
    public bool isListSequential;
    public bool willDeleteActions;
    public float DelayBetweenActions;
}
