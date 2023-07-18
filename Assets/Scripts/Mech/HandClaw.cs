using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandClaw : MonoBehaviour
{
    [SerializeField] private Material _laserMat;
    [SerializeField] private Material _glowMat;
    [SerializeField] private Material _sparkleMat;
    [SerializeField] private Material _laserRing;
    [SerializeField] private Material _laserSmallfire;
    [SerializeField] private Color _sparksStartColor;
    
    // [SerializeField] private LaserDrill laserDrill;
    private Animator _animator;
    private EventBus _eventBus;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _eventBus = GameObject.FindObjectOfType<EventBus>();
        _eventBus.DrillEnabled?.AddListener(OnEnableDrill);

        var laserDrill = GameObject.Find("HandLaserDrill").GetComponent<LaserDrill>();

        laserDrill.SetMaterial(_laserMat, 
            _glowMat, 
            _sparkleMat, 
            _laserRing, 
            _laserSmallfire);
        laserDrill.SetSparksColor(_sparksStartColor);
    }

    public void OnEnableDrill(bool drilling)
    {
        _animator.SetBool("Drill", drilling);
    }

}
