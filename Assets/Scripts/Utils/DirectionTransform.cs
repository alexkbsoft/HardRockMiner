using UnityEngine;

public class DirectionTransform
{
    public static Vector3 MoveToSpecifiedDir(Vector2 move, Transform dir)
    {
        var forward = dir.forward;
        forward.y = 0;

        return dir.right * move.x  + forward.normalized * move.y;
    }
}