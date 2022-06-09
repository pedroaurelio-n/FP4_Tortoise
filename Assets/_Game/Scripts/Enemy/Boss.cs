using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    INACTIVE,
    IDLE,
    STUN,

}

public class Boss : Enemy
{
    [SerializeField] private BossState bossState = BossState.IDLE;
    [SerializeField] private ShootBullets shoot;
    [SerializeField] private PushForce pushForce;
    [SerializeField] private Animator animator;
    [SerializeField] private int bulletCountSequence = 4;
    [SerializeField] private float intervalBetweenSequences = 5f;
    [SerializeField] private float stunDuration;
    [SerializeField] private Transform target;
    [SerializeField] private float turnSpeed;

    [SerializeField] private List<Renderer> DEBUGMESHS;
    [SerializeField] private float colorTriggerDuration;

    private Coroutine _shootCoroutine;
    private Coroutine _colorCoroutine;
    private bool isShooting;

    private void Awake()
    {
        InitializeValues();
    }

    private void Start()
    {
        pushForce.gameObject.SetActive(false);
        ChangeState(0);        
    }

    private void Update()
    {
        switch (bossState)
        {
            case BossState.INACTIVE:
                break;

            case BossState.IDLE:
                LookAtTarget();
                break;

            case BossState.STUN:
                break;
        }
    }

    public void Shoot(int bulletIndex)
    {
        shoot.ChangeBulletIndex(bulletIndex);
        shoot.StartShooting();
    }

    private IEnumerator Shoot()
    {
        isShooting = true;
        var shootInterval = new WaitForSeconds(shoot.GetShootInterval());
        var betweenInterval = new WaitForSeconds(intervalBetweenSequences);
        yield return new WaitForSeconds(intervalBetweenSequences * 0.3f);

        while (isShooting)
        {
            for (int i = 0; i < bulletCountSequence; i++)
            {
                if (i == bulletCountSequence - 1)
                {
                    animator.SetTrigger("Attack_Heavy");
                }
                else
                {
                    animator.SetTrigger("Attack_Light");
                }

                yield return new WaitForSeconds(1f);
            }

            yield return betweenInterval;
        }
    }

    private void LookAtTarget()
    {
        var targetRelativePosition = Quaternion.LookRotation(target.position - transform.position);
        var newRotation = Quaternion.RotateTowards(transform.rotation, targetRelativePosition, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(transform.rotation.x, newRotation.eulerAngles.y, transform.rotation.z);
    }

    public void ChangeState(int index)
    {

        switch (index)
        {
            case 0:
                if (_colorCoroutine != null)
                {
                    StopCoroutine(_colorCoroutine);
                    _colorCoroutine = null;
                }
                if (_shootCoroutine != null)
                {
                    StopCoroutine(_shootCoroutine);
                    _shootCoroutine = null;
                }
                // ChangeColorInstant(Color.red);

                // animator.SetTrigger("Stun_Enter");
                break;

            case 1:
                if (_colorCoroutine != null)
                {
                    StopCoroutine(_colorCoroutine);
                    _colorCoroutine = null;
                }

                // ChangeColorInstant(Color.green);
                if (_shootCoroutine == null) _shootCoroutine = StartCoroutine(Shoot());

                animator.SetTrigger("Stun_Out");
                break;

            case 2:
                if (_shootCoroutine != null)
                {
                    StopCoroutine(_shootCoroutine);
                    _shootCoroutine = null;
                }

                StartCoroutine(StunCoroutine());
                break;
        }
        
        bossState = (BossState)index;
    }

    public override void TakeDamage(Vector3 hitNormal, bool byBullet)
    {
        Debug.Log($"Test");
        if (!byBullet)
        {
            if (bossState == BossState.IDLE)
            {
                // _colorCoroutine = StartCoroutine(ChangeColorCoroutine(Color.green, Color.cyan));
            }

            else if (bossState == BossState.STUN)
            {
                if (isAlive)
                {
                    animator.SetTrigger("Stun_Damage");
                    _currentHealth--;

                    if (_currentHealth <= 0)
                        Die();

                    else
                    {
                        DamageFeedback(hitNormal);
                    }
                }
            }
        }

        else
        {
            ChangeState(2);
        }
    }

    private void ChangeColorInstant(Color color)
    {
        for (int i = 0; i < DEBUGMESHS.Count; i++)
        {
            DEBUGMESHS[i].material.color = color;
        }
    }

    private IEnumerator ChangeColorCoroutine(Color color1, Color color2)
    {
        var duration = new WaitForSeconds(colorTriggerDuration);

        var elapsedTime = 0f;
        var changedColor = color2;
        var naturalColor = color1;

        // ChangeColorInstant(changedColor);

        Color color;

        while (elapsedTime < colorTriggerDuration)
        {
            color = Color.Lerp(changedColor, naturalColor, (elapsedTime/colorTriggerDuration));

            // ChangeColorInstant(color);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ChangeColorInstant(naturalColor);

        yield return null;
    }

    private IEnumerator StunCoroutine()
    {
        animator.SetTrigger("Stun_Enter");
        yield return new WaitForSeconds(stunDuration);
        pushForce.gameObject.SetActive(true);
        ChangeState(1);
        animator.SetTrigger("Stun_Exit");
        yield return new WaitForSeconds(stunDuration * 0.5f);
        pushForce.gameObject.SetActive(false);
    }

    protected override void Die()
    {
        base.Die();
    }

    protected override void DamageFeedback(Vector3 hitNormal)
    {
        // _colorCoroutine = StartCoroutine(ChangeColorCoroutine(Color.red, Color.white));
    }

    protected override void Move()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            pushForce.SetPlayer(player);
            ChangeState(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerMain>(out PlayerMain player))
            ChangeState(0);
    }
}
