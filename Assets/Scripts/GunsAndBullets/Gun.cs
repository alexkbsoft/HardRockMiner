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

    [SerializeField] private Elbow _elbow;
    private bool _canFire = true;
    private GameObject _currentTarget;
    
    void Start()
    {
        _elbow.TargetSelected.AddListener(OnTargetChanged);
    }

    void Update()
    {
        if (!_canFire)
        {
            return;
        }

        if(_currentTarget != null)
        {
            if (Targeting.CanAttackEnemy(transform, _currentTarget, AttackAngle, AttackDistance)) {
                Fire(_currentTarget);
                _canFire = false;
                StartCoroutine(ResetFire());
            }
        }
    }

    private void OnTargetChanged(GameObject newTarget)
    {
        _currentTarget = newTarget;
    }

    private IEnumerator ResetFire() {
        yield return new WaitForSeconds(FirePeriod);
        _canFire = true;
    }

    private void Fire(GameObject go) {
        Debug.Log("Fire: ");
        var bullet = LeanPool.Spawn(BulletPref, transform.position, transform.rotation);
        var proj = bullet.GetComponent<SimpleProjectile>();
        proj.TargetTags.Clear();
        proj.TargetTags.Add("Enemy");
        
        LeanPool.Despawn(bullet, 1.5f);
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

        var rotatedForward = Quaternion.Euler(0, - AttackAngle * 0.5f, 0) * transform.forward;

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
