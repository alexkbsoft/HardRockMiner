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
    
    public float DrillDistance = 3;
    public float DrillPower = 5;
    public float DrillNoise = 5;
    public float DrillWidth = 0.7f;
    
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
        laserDrill.SetWidth(DrillWidth);
        
        laserDrill.DrillDistance = DrillDistance;
        laserDrill.DrillPower = DrillPower;
        laserDrill.DrillNoise = DrillNoise;
    }

    public void OnEnableDrill(bool drilling)
    {
        _animator.SetBool("Drill", drilling);
    }

}
