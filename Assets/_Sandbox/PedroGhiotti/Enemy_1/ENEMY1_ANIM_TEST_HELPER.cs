using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENEMY1_ANIM_TEST_HELPER : MonoBehaviour
{
    [SerializeField] private Animator anim_old;
    [SerializeField] private Animator anim_updated;
    [SerializeField] private bool activated;
    [SerializeField] private bool walking;
    [SerializeField] private bool takeDamage;
    [SerializeField] private bool attack;
    void Start()
    {
        
    }
    void Update()
    {
        anim_old.SetBool("Activated", activated);
        anim_updated.SetBool("Activated", activated);

        anim_old.SetBool("Run", walking);
        anim_updated.SetBool("Run", walking);

        if(takeDamage == true)
        {
            anim_updated.SetTrigger("damage");
            takeDamage = false;
        }
        if(attack == true)
        {
            anim_updated.SetTrigger("Attack");
            anim_old.SetTrigger("Attack");
            attack = false;
        }
    }
}
