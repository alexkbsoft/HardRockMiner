using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrill : MonoBehaviour
{
    public float DrillDistance = 3;
    public float DrillPower = 5;
    public GameObject LaserEffect;
    public GameObject LaserStart;
    public GameObject LaserEnd;
    private Coroutine _checkDistance;
    private bool _isDrilling;
    private Animator _drillingAnimator;

    void Start()
    {
        _checkDistance = StartCoroutine(CheckDistanceToTarget());
        _drillingAnimator = GameObject.FindWithTag("Drill").GetComponent<Animator>();
    }

    private IEnumerator CheckDistanceToTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);

            _isDrilling = Physics.Raycast(transform.position, transform.forward, DrillDistance, 1 << LayerMask.NameToLayer("Walls"));

            LaserEffect.SetActive(_isDrilling);
            LaserEnd.SetActive(_isDrilling);
            LaserStart.SetActive(_isDrilling);

            _drillingAnimator.SetBool("Drill", _isDrilling);
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

            LaserEnd.transform.position = hits[0].point + hits[0].normal * 0.1f;
            LaserEnd.transform.rotation = Quaternion.LookRotation( hits[0].normal);
        }
    }
}
