using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class Elbow : MonoBehaviour
{
    public float MaxAngle = 60;
    public UnityEvent<GameObject> TargetSelected;
    public float Cooldown = 0.2f;

    [SerializeField] private GameObject ConstrainTarget;
    [SerializeField] private GameObject RotateTarget;
    [SerializeField] private GameObject ConstrainHint;
    [SerializeField] private TwoBoneIKConstraint _constraint;
    [SerializeField] private MultiAimConstraint _gunAim;

    private Transform _target;
    private float _distanceToHand;
    private Vector3 _hintOffset;
    private EventBus _eventBus;

    private bool _isCooldown = false;
    private Transform _mechSpine;
    private Coroutine _currentResetter;

    private List<GameObject> _targetsList;


    void Start()
    {
        _constraint = GameObject.Find("GunAim").GetComponent<TwoBoneIKConstraint>();

        _distanceToHand = Vector3.Distance(transform.position, ConstrainTarget.transform.position);
        _hintOffset = ConstrainHint.transform.position - transform.position;

        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.TargetsChanged.AddListener(OnTargetsChanged);

        _mechSpine = GameObject.FindGameObjectWithTag("MechSpine").transform;

        StartCoroutine(SelectTarget());
    }

    void Update()
    {
        float weight = 0;

        if (_target != null)
        {
            var (canRotateHand, dir) = CanRotateHand(_target.position);

            if (canRotateHand)
            {
                weight = 1;
                // RotateTarget.transform.position = Vector3.Slerp(RotateTarget.transform.position, _target.position + Vector3.up * 0.7f, 0.05f);
                // ConstrainTarget.transform.position = Vector3.Slerp(ConstrainTarget.transform.position, transform.position + dir * _distanceToHand, 0.08f);
                RotateTarget.transform.position = _target.position + Vector3.up * 0.7f;
                ConstrainTarget.transform.position = transform.position + dir * _distanceToHand;
                ConstrainHint.transform.position = transform.position + _hintOffset;
            }
            else
            {
                TargetLost();
            }
            // else
            // {
            //     _target = null;
            //     TargetSelected?.Invoke(null);
            //     _isCooldown = true;
            //     StartCoroutine(ResetCooldown());

            //     weight = 0;
            // }
        }

        _constraint.weight = Mathf.Lerp(_constraint.weight, weight, 0.05f);
        _gunAim.weight = Mathf.Lerp(_constraint.weight, weight, 0.05f);
    }

    private void TargetLost()
    {
        _target = null;
        TargetSelected?.Invoke(null);
        _isCooldown = true;

        if (_currentResetter != null)
        {
            StopCoroutine(_currentResetter);
        }
        _currentResetter = StartCoroutine(ResetCooldown());
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        _isCooldown = false;
    }

    private (bool, Vector3) CanRotateHand(Vector3 targetPos)
    {
        var dir = (targetPos - transform.position).normalized;
        var angleCos = Mathf.Cos(MaxAngle * Mathf.Deg2Rad / 2);

        return (Vector3.Dot(dir, transform.forward) > angleCos, dir);
    }

    private void OnTargetsChanged(List<GameObject> targetsList)
    {
        _targetsList = targetsList;
    }

    private IEnumerator SelectTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (!_isCooldown && _targetsList != null)
            {
                if (_targetsList.Count > 0)
                {
                    Transform found = _target;

                    foreach (GameObject newTarget in _targetsList)
                    {
                        var (canRotateHand, _) = CanRotateHand(newTarget.transform.position);

                        if (canRotateHand)
                        {
                            var forward = transform.forward;
                            var pos = transform.position;

                            if (found == null)
                            {
                                found = newTarget.transform;
                                continue;
                            }

                            if (Targeting.Dot(_mechSpine.forward, pos, found) < Targeting.Dot(_mechSpine.forward, pos, newTarget.transform))
                            {
                                found = newTarget.transform;
                            }
                        }
                    }

                    if (_target != found)
                    {
                        TargetLost();
                    }

                    if (_target == null && found != null)
                    {
                        _target = found.transform;
                        TargetSelected?.Invoke(found.gameObject);
                    }

                }
                else
                {
                    TargetSelected?.Invoke(null);
                }
            }
        }

    }

    void OnDestroy()
    {
        _eventBus.TargetsChanged.RemoveListener(OnTargetsChanged);
    }



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Color c = new Color(0.8f, 0, 0, 0.3f);

        UnityEditor.Handles.color = c;

        var rotatedForward = Quaternion.Euler(0, -MaxAngle * 0.5f, 0) * transform.forward;

        UnityEditor.Handles.DrawSolidArc(
            transform.position,
            Vector3.up,
            rotatedForward,
            MaxAngle,
            5
        );
    }
#endif
}
