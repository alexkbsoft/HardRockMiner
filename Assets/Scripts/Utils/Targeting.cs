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

    public static float Dot(Vector3 dir, Vector3 source, Transform to) {
        return Vector3.Dot(dir, to.transform.position - source);
    }
}