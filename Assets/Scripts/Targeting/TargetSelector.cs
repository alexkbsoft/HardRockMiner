using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public float DetectionAngle = 160;
    public float DetectionDistance = 20;
    public EventBus _eventBus;
    void Start()
    {
        _eventBus = FindObjectOfType<EventBus>();
        
        StartCoroutine(FindTarget());
    }

    private IEnumerator FindTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            var attackList = new List<GameObject>();
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject go in enemies)
            {
                if (Targeting.CanAttackEnemy(transform, go, DetectionAngle, DetectionDistance))
                {
                    attackList.Add(go);
                }
            }

            _eventBus.TargetsChanged?.Invoke(attackList);
        }

    }

        #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Color c = new Color(0.5f, 0, 0.5f, 0.3f);

        UnityEditor.Handles.color = c;

        var rotatedForward = Quaternion.Euler(0, - DetectionAngle * 0.5f, 0) * transform.forward;

        UnityEditor.Handles.DrawSolidArc(
            transform.position,
            Vector3.up,
            rotatedForward,
            DetectionAngle,
            DetectionDistance
        );
    }
    #endif
}
