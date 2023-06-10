using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    public float MaxLive = 100;
    public float CurrentLife;
    public UnityEvent OnDestroyed;
    public UnityEvent<float> OnDamaged;



    void Awake()
    {
        CurrentLife = MaxLive;
    }

    public void Damage(float damage)
    {
        if (CurrentLife == 0)
        {
            return;
        }

        CurrentLife -= damage;
        OnDamaged?.Invoke(CurrentLife);

        if (CurrentLife <= 0)
        {
            CurrentLife = 0;
            OnDestroyed?.Invoke();
        }
    }
}
