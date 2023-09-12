using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrill : MonoBehaviour
{
    [SerializeField] MinerState _minerState;
    [SerializeField] private LineRenderer _liserLine;
    [SerializeField] private ParticleSystemRenderer _startRenderer;
    [SerializeField] private ParticleSystemRenderer _sparkleRenderer;
    [SerializeField] private ParticleSystemRenderer _ringRenderer;
    [SerializeField] private ParticleSystemRenderer _sideRenderer;
    [SerializeField] private ParticleSystemRenderer _forwardRenderer;
    [SerializeField] private ParticleSystemRenderer _particleRenderer;
    [SerializeField] private ParticleSystemRenderer _spikesRenderer;
    [SerializeField] private ParticleSystemRenderer _lingerRenderer;
    [SerializeField] private ParticleSystem _startParticle;
    
    
    public bool ForceDrilling = false;
    public LayerMask TargetLayers;
    public float DrillDistance = 3;
    public float DrillPower = 5;
    public float DrillNoise = 5;
    public GameObject LaserEffect;
    public GameObject LaserStart;
    public GameObject LaserEnd;
    private Coroutine _checkDistance;
    private bool _isDrilling;
    public Animator DrillingAnimator;
    private Transform _drillRay;
    private EventBus _eventBus;

    void Start()
    {
        // _checkDistance = StartCoroutine(CheckDistanceToTarget());
        // _drillingAnimator = GameObject.FindWithTag("Drill").GetComponent<Animator>();
        _drillRay = transform.Find("DrillRay");

        SetLength(DrillDistance);

        LaserEffect.SetActive(ForceDrilling);
        LaserStart.SetActive(ForceDrilling);

        _eventBus = EventBus.Instance;

    }

    public void EnableDrill(bool drilling)
    {
        _isDrilling = drilling;

        LaserEffect.SetActive(_isDrilling);
        LaserStart.SetActive(_isDrilling);
    }

    private void SetLength(float distance)
    {
        _drillRay.localScale = new Vector3(0, 0, distance);
    }

    // private IEnumerator CheckDistanceToTarget()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(0.05f);

    //         _isDrilling = Physics.Raycast(transform.position, transform.forward, DrillDistance, TargetLayers) || ForceDrilling;

    //         LaserEffect.SetActive(_isDrilling);
    //         LaserStart.SetActive(_isDrilling);
    //         LaserEnd.SetActive(false);

    //         _drillingAnimator.SetBool("Drill", _isDrilling);
    //     }
    // }


    void Update()
    {
        bool drilling = _isDrilling || ForceDrilling;
        if (!drilling)
        {
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, DrillDistance, TargetLayers);
        var drillLength = DrillDistance;
        var showHitEnd = false;

        if (hits.Length > 0)
        {
            var (closesObj, hit, distance) = Targeting.GetClosest(hits, transform.position);
            if (closesObj.TryGetComponent<Damagable>(out var damagable))
            {
                damagable.Damage(DrillPower * Time.deltaTime);
                
                var noiseDelta = DrillNoise * Time.deltaTime;
                _eventBus.AlarmChanged?.Invoke(noiseDelta);
            }

            if (hit != null)
            {
                drillLength = distance;
                showHitEnd = true;

                RaycastHit resultHit = hit.Value;
                LaserEnd.transform.position = resultHit.point + resultHit.normal * 0.1f;
                LaserEnd.transform.rotation = Quaternion.LookRotation(hits[0].normal);
            }
        }

        SetLength(drillLength);
        LaserEnd.SetActive(showHitEnd);
    }

    public void SetMaterial(Material mat, Material startMat, Material sparkleMat, Material ring, Material smallfire)
    {
        _liserLine.material = mat;
        _startRenderer.sharedMaterial = startMat;
        _sparkleRenderer.sharedMaterial = sparkleMat;
        _ringRenderer.sharedMaterial = ring;
        _sideRenderer.sharedMaterial = smallfire;
        _forwardRenderer.sharedMaterial = smallfire;
        _particleRenderer.sharedMaterial = smallfire;
        _spikesRenderer.sharedMaterial = smallfire;
        _lingerRenderer.sharedMaterial = smallfire;

    }

    public void SetSparksColor(Color sparksColor)
    {
        var mainModule = _startParticle.main;
        mainModule.startColor = sparksColor;
    }

    public void SetWidth(float width)
    {
        _liserLine.startWidth = width;
        _liserLine.endWidth = width;
    }
}
