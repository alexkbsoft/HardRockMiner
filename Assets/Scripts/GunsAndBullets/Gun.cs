using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject BulletPref;
    public float FirePeriod = 1;
    public float AttackAngle = 30;
    public float AttackDistance = 20;

    private bool _canFire = true;
    void Start()
    {

    }

    void Update()
    {
        if (!_canFire)
        {
            return;
        }

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject go in enemies)
        {
            if (CanAttackEnemy(go)) {
                Fire(go);
                _canFire = false;
                StartCoroutine(ResetFire());

                break;
            }
        }
    }

    private IEnumerator ResetFire() {
        yield return new WaitForSeconds(FirePeriod);
        _canFire = true;
    }

    private bool CanAttackEnemy(GameObject enemy)
    {
        float angleCos = Mathf.Cos(Mathf.Deg2Rad * AttackAngle);
        Vector3 direction = (enemy.transform.position - transform.position);

        return Vector3.Dot(direction.normalized, transform.forward) > angleCos
            && direction.magnitude < AttackDistance;
    }

    private void Fire(GameObject go) {
        Debug.Log("Fire: ");
        var bullet = LeanPool.Spawn(BulletPref, transform.position, transform.rotation);
        bullet.GetComponent<SimpleProjectile>().TargetTags.Add("Enemy");
        LeanPool.Despawn(bullet, 1.5f);
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
