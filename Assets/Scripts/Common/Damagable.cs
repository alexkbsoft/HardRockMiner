using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    public float MaxLive = 100;
    public UnityEvent OnDestroyed;

    private float CurrentLife;

    void Start()
    {
        CurrentLife = MaxLive;
    }

    public void Damage(float damage)
    {
        if (CurrentLife == 0) {
            return;
        }

        CurrentLife -= damage;

        if (CurrentLife <= 0)
        {
            CurrentLife = 0;
            OnDestroyed?.Invoke();
        }
    }
}
