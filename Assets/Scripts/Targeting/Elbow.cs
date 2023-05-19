using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class Elbow : MonoBehaviour
{
    public float MaxAngle = 60;
    public UnityEvent<GameObject> TargetSelected;

    [SerializeField] private GameObject ConstrainTarget;
    [SerializeField] private GameObject RotateTarget;
    [SerializeField] private GameObject ConstrainHint;
    [SerializeField] private TwoBoneIKConstraint _constraint;
    [SerializeField] private MultiAimConstraint _gunAim;

    private Transform _target;
    private float _distanceToHand;
    private Vector3 _hintOffset;
    private EventBus _eventBus;



    void Start()
    {
        _constraint = GameObject.Find("GunAim").GetComponent<TwoBoneIKConstraint>();

        _distanceToHand = Vector3.Distance(transform.position, ConstrainTarget.transform.position);
        _hintOffset = ConstrainHint.transform.position - transform.position;

        _eventBus = FindObjectOfType<EventBus>();
        _eventBus.TargetsChanged.AddListener(OnTargetsChanged);
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
                RotateTarget.transform.position = Vector3.Slerp(RotateTarget.transform.position, _target.position + Vector3.up * 0.7f, 0.08f);
                ConstrainTarget.transform.position = Vector3.Slerp(ConstrainTarget.transform.position, transform.position + dir * _distanceToHand, 0.08f);
                ConstrainHint.transform.position = transform.position + _hintOffset;
            }
            else
            {
                _target = null;
                TargetSelected?.Invoke(null);

                weight = 0;
            }
        }

        _constraint.weight = Mathf.Lerp(_constraint.weight, weight, 0.1f);
        _gunAim.weight = Mathf.Lerp(_constraint.weight, weight, 0.1f);
    }

    private (bool, Vector3) CanRotateHand(Vector3 targetPos)
    {
        var dir = (targetPos - transform.position).normalized;
        var angleCos = Mathf.Cos(MaxAngle * Mathf.Deg2Rad / 2);

        return (Vector3.Dot(dir, transform.forward) > angleCos, dir);
    }

    private void OnTargetsChanged(List<GameObject> targetsList)
    {
        if (_target != null)
        {
            return;
        }

        if (targetsList.Count > 0)
        {
            foreach (GameObject newTarget in targetsList)
            {
                var (canRotateHand, _) = CanRotateHand(newTarget.transform.position);

                if (canRotateHand)
                {

                    _target = newTarget.transform;
                    TargetSelected?.Invoke(newTarget);
                    break;
                }

            }

        }
        else
        {
            TargetSelected?.Invoke(null);
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
