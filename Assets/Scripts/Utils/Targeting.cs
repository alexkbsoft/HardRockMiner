using UnityEngine;

public class Targeting
{
    public static bool CanAttackEnemy(Transform positionTransform, GameObject enemy, float angle, float distance)
    {
        float angleCos = Mathf.Cos(Mathf.Deg2Rad * angle / 2);
        var enemyPos = enemy.transform.position;
        enemyPos.y = 0;
        var currentPos = positionTransform.position;
        currentPos.y = 0;
        Vector3 direction = (enemyPos - currentPos);

        return Vector3.Dot(direction.normalized, positionTransform.forward) > angleCos
            && direction.magnitude < distance;
    }

    public static float Dot(Vector3 dir, Vector3 source, Transform to)
    {
        return Vector3.Dot(dir, (to.transform.position - source).normalized);
    }

    public static (GameObject, RaycastHit?, float) GetClosest(RaycastHit[] hits, Vector3 toPosition)
    {
        GameObject closest = null;
        RaycastHit? hit = null;

        float dist = float.PositiveInfinity;

        foreach (RaycastHit target in hits)
        {
            var newDist = Vector3.Distance(toPosition, target.point);

            if (newDist < dist)
            {
                dist = newDist;
                closest = target.collider.gameObject;
                hit = target;
            }
        }

        return (closest, hit, dist);
    }
}