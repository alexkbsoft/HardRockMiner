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

    [SerializeField] private Elbow _elbow;

    private GameObject _currentTarget;
    private float _timeToFire;

    void Start()
    {
        _elbow.TargetSelected.AddListener(OnTargetChanged);
    }

    void Update()
    {
        _timeToFire -= Time.deltaTime;

        if (_timeToFire <= 0) {
            _timeToFire = 1 / FireRate;
            AnimatedGun.Fire(WillHit());
        }
    }

    private bool WillHit()
    {
        return _currentTarget != null && Physics.SphereCast(
            transform.position - transform.forward,
            2,
            transform.forward,
            out var _,
            100,
            1 << LayerMask.NameToLayer("Enemy"));
    }

    private void OnTargetChanged(GameObject newTarget)
    {
        _currentTarget = newTarget;
    }

    public void FireBullet()
    {
        if (!WillHit())
        {
            return;
        }

        var bullet = LeanPool.Spawn(BulletPref, transform.position, transform.rotation);
        bullet.transform.LookAt(_currentTarget.transform.position + Constants.TargetOffset);

        var proj = bullet.GetComponent<SimpleProjectile>();
        proj.TargetTags.Clear();
        proj.TargetTags.Add("Enemy");

        LeanPool.Despawn(bullet, 1.5f);

        var muzzleFlash = LeanPool.Spawn(
            MuzzleFlashPrefab,
            transform.position + transform.forward * 0.5f,
            transform.rotation * Quaternion.Euler(0, 180, 0));

        LeanPool.Despawn(muzzleFlash, 0.5f);
    }

    void OnDestroy()
    {
        _elbow.TargetSelected.RemoveListener(OnTargetChanged);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Color c = new Color(0.8f, 0, 0, 0.3f);

        UnityEditor.Handles.color = c;

        var rotatedForward = Quaternion.Euler(0, -AttackAngle * 0.5f, 0) * transform.forward;

        UnityEditor.Handles.DrawSolidArc(
            transform.position,
            Vector3.up,
            rotatedForward,
            AttackAngle,
            AttackDistance
        );
    }
#endif
}
