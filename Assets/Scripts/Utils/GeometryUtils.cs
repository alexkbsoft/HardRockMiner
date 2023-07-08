using UnityEngine;

namespace Utils
{
    public class GeometryUtils
    {
        public static float IntersectArea(Bounds b1, Bounds b2)
        {
            float x1 = Mathf.Min(b1.max.x, b2.max.x);
            float x2 = Mathf.Max(b1.min.x, b2.min.x);
            float y1 = Mathf.Min(b1.max.y, b2.max.y);
            float y2 = Mathf.Max(b1.min.y, b2.min.y);

            var width = x1 - x2;
            var height = y1 - y2;

            return width * height;
        }
    }
}