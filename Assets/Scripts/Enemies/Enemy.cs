using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float RunSpeed = 10.0f;
    public float AnimationMult = 1.5f;
    public float PatrolPeriod = 4.0f;
    public Transform[] Waypoints;
    public float AttackDistance = 8.0f;
    public float WatchDistance = 20;

    public float AttackEffectDistance = 4;

    private NavMeshAgent _navMeshAgent;

    private Vector3 _currentDestination;
    private Animator _anim;
    private GameObject _target;
    private bool _pursue = false;

    private Vector3 _prevPos;

    private bool _isAttacking = false;
    private bool _isDamaged = false;
    private bool _isDead = false;
    private HitPointsBar _hpBar;
    private Canvas _hpBarCanvas;
    private Damagable _damagable;
    private bool _fading = false;
    private Coroutine _checkTargetsCoroutine;

    void Start()
    {
        // StartCoroutine(SelectPoint());
        _checkTargetsCoroutine = StartCoroutine(CheckTarget());

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _hpBar = GetComponentInChildren<HitPointsBar>();
        _damagable = GetComponent<Damagable>();

        _target = GameObject.Find("Player");

        _prevPos = _navMeshAgent.transform.position;

        _damagable.OnDamaged.AddListener(Damage);

        _hpBar.SetHitPoints((int)_damagable.MaxLive, (int)_damagable.MaxLive);
        _hpBarCanvas = _hpBar.GetComponentInParent<Canvas>();
        _hpBarCanvas.enabled = false;
    }

    void Update()
    {
        if (_isDead)
        {
            if (_fading)
            {
                transform.position += Vector3.down * Time.deltaTime;
            }
            
            return;
        }

        if (_pursue)
        {
            _currentDestination = _target.transform.position;
        }

        CheckAnimations();


        if (_isAttacking)
        {
            _navMeshAgent.velocity = Vector3.zero;
            var attackRotation = _target.transform.position - transform.position;
            attackRotation.y = 0;
            _navMeshAgent.transform.rotation = Quaternion.LookRotation(attackRotation);
        }
        else
        {
            _navMeshAgent.SetDestination(_currentDestination);
        }
    }

    private void CheckAnimations()
    {
        _anim.SetBool("Run", _navMeshAgent.velocity.magnitude > 0.1);
        _anim.SetBool("Attack", TargetDist() <= AttackDistance);
    }

    private float TargetDist()
    {
        var pos = _target.transform.position;
        var enemyPos = transform.position;

        return Vector3.Distance(pos, enemyPos);
    }

    private IEnumerator CheckTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            var pos = _target.transform.position;
            pos.x = 0;
            var enemyPos = transform.position;
            enemyPos.x = 0;
            if (TargetDist() < WatchDistance)
            {
                _pursue = true;
                _navMeshAgent.stoppingDistance = AttackDistance;
                _navMeshAgent.speed = RunSpeed;
                // _anim.SetFloat("Speed", AnimationMult);
            }
            else
            {
                _pursue = false;
                _navMeshAgent.stoppingDistance = 0;
                // _anim.SetFloat("Speed", 1f);
            }
        }
    }

    public void Damage(float currentHP)
    {
        _hpBar.SetHitPoints((int)currentHP, (int)_damagable.MaxLive);
        _hpBarCanvas.enabled = true;

    }

    public void Die()
    {
        if (_isDead == true)
        {
            return;
        }

        _isDead = true;
        _hpBarCanvas.enabled = false;

        this.tag = "Untagged";
        _navMeshAgent.isStopped = true;
        _anim.SetTrigger("Dead");

        StopCoroutine(_checkTargetsCoroutine);

        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<CharacterController>());
        Destroy(GetComponent<NavMeshAgent>());

        Destroy(gameObject, 3.5f);
        StartCoroutine(StartFading());
    }

    private IEnumerator StartFading()
    {
        yield return new WaitForSeconds(2.0f);
        _fading = true;
    }

    // private IEnumerator SelectPoint()
    // {
    // while (true)
    // {
    // yield return new WaitForSeconds(PatrolPeriod);

    // var pt1 = twoPointZone[0].position;
    // var pt2 = twoPointZone[1].position;
    // _currentDestination = pt1 + Vector3.right * Random.Range(0, Vector3.Distance(pt1, pt2));
    // }
    // }

    #region Animation events

    public void HitEvent()
    {
        // var toRacoon = _racoon.transform.position - transform.position;

        // if (
        //     Vector3.Dot(toRacoon, transform.forward) > 0.5f &&
        //     RacoonDist() <= AttackEffectDistance
        //     )
        // {
        //     _racoon.GetComponent<PlayerController>().Damage(10, transform.position);
        // }
    }
    public void AttackEnd()
    {
        _isAttacking = false;
    }

    public void AttackStart()
    {
        _isAttacking = true;
    }

    #endregion

    void OnDestroy()
    {
        _damagable.OnDamaged?.RemoveAllListeners();
    }

}
