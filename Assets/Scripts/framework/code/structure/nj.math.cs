using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nj
{
    public static class math
    {
        public static float angleByNormalVecXY(Vector3 nV)
        {
            float angle180 = Vector3.Angle(nV, Vector3.up);
            if (Vector3.Dot(nV, Vector3.left) > 0)
                return angle180;
            return -angle180;
        }

        public static float angleByNormalVecXY_Fast(Vector3 nV)
        {
            float angle180 = acos(Vector3.Dot(nV, Vector3.up)) * Mathf.Rad2Deg;// * 90;
            if (Vector3.Dot(nV, Vector3.left) > 0)
                return angle180;
            return -angle180;
        }

        public static float acos(float x)
        {
            return (-0.69813170079773212f * x * x - 0.87266462599716477f) * x + 1.5707963267948966f;
        }


    }
}