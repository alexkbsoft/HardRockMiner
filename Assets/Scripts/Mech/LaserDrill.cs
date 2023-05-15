using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrill : MonoBehaviour
{
    public float DrillDistance = 3;
    public float DrillPower = 5;
    public GameObject LaserEffect;
    private Coroutine _checkDistance;
    private bool _isDrilling;

    void Start()
    {
        _checkDistance = StartCoroutine(CheckDistanceToTarget());
    }

    private IEnumerator CheckDistanceToTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            _isDrilling = Physics.Raycast(transform.position, transform.forward, DrillDistance, 1 << LayerMask.NameToLayer("Walls"));

            LaserEffect.SetActive(_isDrilling);
        }
    }


    void Update()
    {
        if (!_isDrilling) {
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, DrillDistance, 1 << LayerMask.NameToLayer("Walls"));

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<Damagable>(out var damagable)) {
                    damagable.Damage(DrillPower * Time.deltaTime);
                }
            }
        }
    }
}
