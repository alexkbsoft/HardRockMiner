using System.Collections;
using System.Collections.Generic;
using Pathfinding;
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

    private Vector3 _currentDestination;
    private Animator _anim;
    private Transform _target;
    private bool _pursue = false;


    private bool _isAttacking = false;
    private bool _isDamaged = false;
    private bool _isDead = false;
    private HitPointsBar _hpBar;
    private Canvas _hpBarCanvas;
    private Damagable _damagable;
    private bool _fading = false;
    private Coroutine _checkTargetsCoroutine;
    private AIPath _aiPathFinder;
    private AIDestinationSetter _destinationSetter;
    private bool _closeEnough = false;

    void Start()
    {

        _anim = GetComponent<Animator>();
        _hpBar = GetComponentInChildren<HitPointsBar>();
        _damagable = GetComponent<Damagable>();
        _aiPathFinder = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();

        _target = GameObject.Find("Player").transform;


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

        CheckAnimations();


        if (_closeEnough)
        {
            var attackRotation = _target.transform.position - transform.position;
            attackRotation.y = 0;
            transform.rotation = Quaternion.LookRotation(attackRotation);
        }
    }



    private void CheckAnimations()
    {
        _anim.SetBool("Run", !_closeEnough);
        _closeEnough = TargetDist() <= AttackDistance;

        // if (!_isAttacking)
        // {
            _anim.SetBool("Attack", _closeEnough);

        // }
        if (_closeEnough && _aiPathFinder.updatePosition)
        {
            _aiPathFinder.updatePosition = false;
            _aiPathFinder.updateRotation = false;
        }

        if (!_closeEnough && !_aiPathFinder.updatePosition)
        {
            _aiPathFinder.updatePosition = true;
            _aiPathFinder.updateRotation = true;
        }
    }

    private float TargetDist()
    {
        var pos = _target.transform.position;
        var enemyPos = transform.position;

        return Vector3.Distance(pos, enemyPos);
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
        _destinationSetter.target = null;
        
        _anim.SetTrigger("Dead");

        

        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<CharacterController>());
        Destroy(GetComponent<AIPath>());
        

        Destroy(gameObject, 3.5f);
        StartCoroutine(StartFading());
    }

    private IEnumerator StartFading()
    {
        yield return new WaitForSeconds(2.0f);
        _fading = true;
    }


    #region Animation events

    public void AttackEnd()
    {
        // _isAttacking = false;
        // _destinationSetter.target = _target;
    }

    public void AttackStart()
    {
        // _isAttacking = true;
        // _destinationSetter.target = null;
    }

    #endregion

    void OnDestroy()
    {
        _damagable.OnDamaged?.RemoveAllListeners();
    }

}
