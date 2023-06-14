using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillingMine : MonoBehaviour
{
    [SerializeField] private Transform BlowEffect;
    public float Radius = 5;
    public float Power = 100;
    public float Timeout = 2;
    public LayerMask _damageLayer;
    void Start()
    {
        InitEffect();
        StartCoroutine(Blow());
    }

    public void InitEffect() {
        BlowEffect.localScale = Vector3.one * Radius;
    }

    private IEnumerator Blow() {
        yield return new WaitForSeconds(Timeout);
        StartCoroutine(ApplyDamage());
    }
    private IEnumerator ApplyDamage() {
        BlowEffect.gameObject.SetActive(true);
        BlowEffect.transform.parent = null;

        yield return new WaitForSeconds(0.1f);

        var colliders = Physics.OverlapSphere(transform.position, Radius, _damageLayer);

        foreach(Collider c in colliders) {
            if (c.gameObject.TryGetComponent<Damagable>(out var damagable)) {
                damagable.Damage(Power);
            }
        }

        Destroy(gameObject);
    }
    
}
