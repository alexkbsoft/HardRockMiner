using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatedGun : MonoBehaviour
{
    public UnityEvent Fiered;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Fire(bool fire) {
        _animator.SetBool("Shoot", fire);
    }

    public void OnShoot() {
        Fiered?.Invoke();
    }
}
