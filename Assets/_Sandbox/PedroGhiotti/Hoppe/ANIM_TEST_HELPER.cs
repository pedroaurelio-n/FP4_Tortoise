using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANIM_TEST_HELPER : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Animator anim_old;
    [SerializeField] private Animator anim_updated;
    [Header("Config")]
    [SerializeField] private float loopDelay = 1;
    [SerializeField] private float timeScale = 1;
    [SerializeField] private bool doAttacksCombo = false;

    void Start()
    {
        StartCoroutine(Loop_Testprivate());
    }
    private void Update()
    {
        Time.timeScale = timeScale;
        anim_old.SetBool("Attack_Combo", doAttacksCombo);
        anim_updated.SetBool("Attack_Combo", doAttacksCombo);
    }
    private IEnumerator Loop_Testprivate()
    {
        anim_old.SetTrigger("Attack");
        anim_updated.SetTrigger("Attack");
        yield return new WaitForSeconds(loopDelay);
        StartCoroutine(Loop_Testprivate());
    }
}
