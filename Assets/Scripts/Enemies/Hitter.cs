using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class Hitter : MonoBehaviour
{
    public float HitPower = 10;
    public float HitEffectDistance = 2;
    public float HitSphereRadius = 2;
    public Transform HitSource;
    public GameObject HitEffect;

    public void OnAnimationHitMoment()
    {
        Debug.Log("ON ANIMATION HIT");
        if (Physics.SphereCast(HitSource.position,
            HitSphereRadius,
            HitSource.forward,
            out var hitInfo,
            HitEffectDistance,
            1 << LayerMask.NameToLayer("Player")
            ))
        {
            var hitTarget = hitInfo.collider.gameObject;

            if (hitTarget.TryGetComponent<Damagable>(out var damagable))
            {
                damagable.Damage(HitPower);

                if (HitEffect != null) {
                    var effect = LeanPool.Spawn(HitEffect,
                        hitInfo.point,
                        Quaternion.LookRotation(hitInfo.normal));
                    LeanPool.Despawn(effect, 2.0f);
                }
            }

        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (HitSource != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(HitSource.position, HitSphereRadius);
            Gizmos.DrawLine(HitSource.position, HitSource.position + HitSource.forward * HitEffectDistance);
        }
    }
#endif
}
