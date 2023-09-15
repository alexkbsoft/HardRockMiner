using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    private Damagable _playerDamagable;

    void Start()
    {
        _playerDamagable = GetComponent<Damagable>();
        StartCoroutine(SpendOxygen());
    }

    private IEnumerator SpendOxygen() {
        while(true) {
            Debug.Log("SPEND");
            yield return new WaitForSeconds(1);

            _playerDamagable.Damage(1);
        }
    }
}
