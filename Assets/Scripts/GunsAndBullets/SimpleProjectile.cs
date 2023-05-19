using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;


public class SimpleProjectile : MonoBehaviour
{
    public float Speed = 50.0f;
    public float RaycastAdvance = 2f;
    public float Damage = 20.0f;
    public LayerMask layerMask;
    public List<string> TargetTags;

    [SerializeField] GameObject _flarePrefab;

    void Start()
    {

    }

    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        Vector3 step = transform.forward * Time.deltaTime * Speed;

        if (Physics.SphereCast(transform.position,
            0.35f,
            transform.forward,
            out var hitPoint,
            step.magnitude * RaycastAdvance,
            layerMask))
        {
            Damagable damagable = hitPoint.collider.gameObject.GetComponent<Damagable>();
            if (damagable && TargetTags.Contains(hitPoint.collider.tag))
            {
                damagable.Damage(Damage);
            }

            LeanPool.Despawn(gameObject);
            var flare = LeanPool.Spawn(_flarePrefab);
            flare.transform.position = hitPoint.point;
            flare.transform.rotation = Quaternion.LookRotation(hitPoint.normal);
            LeanPool.Despawn(flare, 1.5f);
        }
    }
}
