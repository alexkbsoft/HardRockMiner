using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Elbow : MonoBehaviour
{
    [SerializeField] private GameObject ConstrainTarget;
    [SerializeField] private TwoBoneIKConstraint _constraint;

    private Transform _target;

    
    void Start()
    {
        _constraint = GameObject.Find("GunAim").GetComponent<TwoBoneIKConstraint>();
    }

    void Update()
    {

        _constraint.weight = _target == null ? 0 : 1;
    }
}
