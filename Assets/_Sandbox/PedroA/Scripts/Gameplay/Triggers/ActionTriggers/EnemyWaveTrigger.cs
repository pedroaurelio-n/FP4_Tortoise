using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveTrigger : ActionTrigger
{
    public List<Enemy> EnemyList;
    
    public void RemoveEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);

        if (EnemyList.Count == 0)
        {
            StartCoroutine(base.CheckAction(reference));
        }
    }
}
