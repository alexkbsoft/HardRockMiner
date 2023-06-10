using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    public GameObject BulletPref;
    public float FirePeriod = 1;
    public float AttackAngle = 30;
    public float AttackDistance = 20;
    public AnimatedGun AnimatedGun;
    public GameObject MuzzleFlashPrefab;
    public float FireRate = 1.1f;
    public LayerMask _targetLayer;
    private bool _Fire = false;

    [SerializeField] private Elbow _elbow;

    private GameObject _currentTarget;
    private float _timeToFire;
    private EventBus _eventBus;


    void Start()
    {
        // _elbow.TargetSelected.AddListener(OnTargetChanged);

        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.FireEnabled?.AddListener(EnableFire);
    }

    void Update()
    {
        // _timeToFire -= Time.deltaTime;

        // if (_timeToFire <= 0)
        // {
        //     _timeToFire = 1 / FireRate;
        //     _Fire = true;

        //     // _currentTarget = WillHit();
        // }

        AnimatedGun.Fire(_Fire);
    }

    private void EnableFire(bool enableFire)
    {
        _Fire = enableFire;
    }

    private GameObject WillHit()
    {
        var hitTargets = Physics.SphereCastAll(
            transform.position - transform.forward * 1.5f,
            1f,
            transform.forward,
            AttackDistance,
            _targetLayer);

        GameObject closest = null;
        float dist = float.PositiveInfinity;

        foreach (RaycastHit target in hitTargets)
        {
            var newDist = Vector3.Distance(transform.position, target.point);

            if (newDist < dist)
            {
                dist = newDist;
                closest = target.collider.gameObject;
            }
        }

        return
            closest != null &&
            closest.tag == "Enemy" ? closest : null;
    }

    private void OnTargetChanged(GameObject newTarget)
    {
        // _currentTarget = newTarget;
    }

    public void FireBullet()
    {
        // if (!WillHit())
        // {
        //     return;
        // }

        // if (_currentTarget == null) {
        //     return;
        // }

        var forward = transform.forward;
        forward.y = 0;

        var bullet = LeanPool.Spawn(BulletPref,
            transform.position,
            Quaternion.LookRotation(forward));

        // bullet.transform.LookAt(_currentTarget.transform.position + Vector3.up);

        var proj = bullet.GetComponent<SimpleProjectile>();

        proj.TargetTags.Clear();
        proj.TargetTags.Add("Enemy");

        LeanPool.Despawn(bullet, 1f);

        var muzzleFlash = LeanPool.Spawn(
            MuzzleFlashPrefab,
            transform.position + transform.forward * 0.5f,
            transform.rotation * Quaternion.Euler(0, 180, 0));

        LeanPool.Despawn(muzzleFlash, 0.6f);
    }

    void OnDestroy()
    {
        // _elbow.TargetSelected.RemoveListener(OnTargetChanged);
        _eventBus.FireEnabled?.RemoveListener(EnableFire);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Color c = new Color(0.8f, 0, 0, 1f);

        UnityEditor.Handles.color = c;

        var rotatedForward = Quaternion.Euler(0, -AttackAngle * 0.5f, 0) * transform.forward;

        Debug.DrawLine(transform.position - transform.forward * 1.5f, transform.position + transform.forward * AttackDistance, Color.red);

        UnityEditor.Handles.DrawWireArc(
            transform.position - transform.forward * 1.5f,
            Vector3.up,
            rotatedForward,
            AttackAngle,
            AttackDistance
        );
    }
#endif
}
