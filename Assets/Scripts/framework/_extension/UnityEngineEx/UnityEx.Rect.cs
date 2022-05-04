using UnityEngine;

namespace UnityEngineEx
{
    public static class RectEx
    {
        public static Rect MinMax(this Rect rect, Vector3 pos)
        {
            return Rect.MinMaxRect(
                Mathf.Min(rect.xMin, pos.x),
                Mathf.Min(rect.yMin, pos.y),
                Mathf.Max(rect.xMax, pos.x),
                Mathf.Max(rect.yMax, pos.y));
        }
    }

}
