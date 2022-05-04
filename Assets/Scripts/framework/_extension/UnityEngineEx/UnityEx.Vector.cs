using System.Runtime.CompilerServices;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngineEx
{
    public static class VectorEx
    {
        public static readonly Vector3 Huge = new Vector3(11921, 11921);

        #region Convert Between Vector2 and Vector3
        public static Vector2 xz(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3 xz(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }

        public static int IntX(this Vector3 v)
        {
            return (int)v.x;
        }

        public static int IntY(this Vector3 v)
        {
            return (int)v.y;
        }

        public static Pt Pt(this Vector3 v)
        {
            return new Pt((int)v.x, (int)v.y);
        }

        #endregion

        #region Vector3
        public static Vector3 WithGapY(this Vector3 v, float gapY)
        {
            return new Vector3(v.x, v.y + gapY, v.z);
        }

        public static void SetSnapXZ(this Vector3 v, float gap, bool snapMode = true)
        {
            v = v.SnapXZ(gap, snapMode);
        }
        public static Vector3 SnapXZ(this Vector3 v, float gap, bool snapMode = true)
        {
            if (!snapMode)
                return v;
            
            return gap < 0.000001f && gap > -0.000001f ?
                    Vector3.zero : 
                    new Vector3( Mathf.Round(v.x / gap ) * gap, 0,
                                 Mathf.Round(v.z / gap) * gap );
        }

        public static bool IsNearBy(this Vector3 p1, Vector3 p2, float dist)
        {
            return (p2 - p1).sqrMagnitude < dist* dist;
        }

        public static bool IsFarFrom(this Vector3 p1, Vector3 p2, float dist)
        {
            return (p2 - p1).sqrMagnitude > dist * dist;
        }

        public static Vector3 NearestPos_OnLine(this Vector3 p, Vector3 linePt1, Vector3 linePt2)
        {
            Vector3 line = (linePt2 - linePt1);
            float len = line.magnitude;
            line.Normalize();

            Vector3 v = p - linePt1;
            float d = Vector3.Dot(v, line);
            d = Mathf.Clamp(d, 0f, len);
            return linePt1 + line * d;
        }

        public static bool IsOnLine(this Vector3 p, Vector3 linePt1, Vector3 linePt2, float gap)
        {
            Vector3 nearestPos = p.NearestPos_OnLine(linePt1, linePt2);
            return p.IsNearBy(nearestPos, gap);
        }

        /*
        public static bool IsOnLine(this Vector3 p, Vector3 nearestPos, float gap)
        {
            return p.IsNearBy(nearestPos, gap);

            //return !nearestPos.IsNearBy(linePt1, gap) && !nearestPos.IsNearBy(linePt2, gap) && p.IsNearBy(nearestPos, gap);
        }*/

        // 주의! 오른손 좌표계 기준으로 ( Vector3.up )
        public static float GetAngleTo(this Vector3 from, Vector3 to)
        {
            float angle = Vector3.Angle(from, to);
            Vector3 right = Vector3.Cross(from, Vector3.up); 
            return (Vector3.Dot(right.normalized, to.normalized) < 0) ? 360f - angle : angle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DotBtw(this Vector3 from, Vector3 to1, Vector3 to2)
        {
            return Vector3.Dot(to1 - from, to2 - from);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 SqrGapXZ(Vector3 v0, Vector3 v1)
        {
            float x = v0.x - v1.x;
            float z = v0.z - v1.z;
            return new Vector2(x* x, z*z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitudeXZ(Vector3 v0, Vector3 v1)
        {
            float x = v0.x - v1.x;
            float z = v0.z - v1.z;
            return x * x + z * z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(Vector3 v0, Vector3 v1, float t)
        {
            float dot = Vector3.Dot(v0, v1);
            if (dot > -0.97)
                return Vector3.Lerp(v0, v1, t);
            return Vector3.Lerp(v0, Vector3.Cross(v1, Vector3.up), t*0.5f);
        }
        #endregion

        #region Vector3 array
        public static void Sort_LongDistFirst(this Vector3[] array, Vector3 target)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i+1; j < array.Length; j++)
                {
                    if ((array[j]-target).sqrMagnitude > (array[i] - target).sqrMagnitude)
                    {
                        Vector3 tmp = array[i];
                        array[i] = array[j];
                        array[j] = tmp;
                    }
                }
            }
        }
        #endregion

        //Union used by InvSqrt
        [StructLayout(LayoutKind.Explicit)]
        struct FloatIntUnion
        {
            [FieldOffset(0)]
            public float x;

            [FieldOffset(0)]
            public int i;
        }

        static FloatIntUnion union = new FloatIntUnion();

        //Fast inverse Sqrt
        static float InvSqrt(float x)
        {
            float x2 = x * 0.5f;
            union.x = x;
            union.i = 0x5f3759df - (union.i >> 1);
            x = union.x;
            x = x * (1.5f - x2 * x * x);
            return x;
        }


        //Normalize vector using fast inverse Sqrt
        public static Vector3 FastNorOnXY(this Vector3 src)
        {
            float inversedMagnitude = InvSqrt(src.sqrMagnitude);
            return new Vector3(src.x * inversedMagnitude, src.y * inversedMagnitude);
        }

        public static Vector2 FastNormalized(this Vector2 src)
        {
            float inversedMagnitude = InvSqrt(src.sqrMagnitude);
            return src * inversedMagnitude;
        }

        const float a0 = 15.0f * 0.125f;
        const float a1 = -5.0f * 0.25f;
        const float a2 = 3.0f * 0.125f;
        public static Vector3 FastNorXY(this Vector3 v)
        {
            return v * (1.5f - 0.5f * (v.x * v.x + v.y * v.y ));
            /*
            float len_sq = v.x * v.x + v.y * v.y;
            float len_inv =a0 + a1 * len_sq + a2 * len_sq * len_sq;
            //float len_inv = 1.875f - 0.78125f * len_sq + 0.375f * len_sq * len_sq;
            return new Vector3(v.x * len_inv, v.y * len_inv);
            //*/
        }
    }

}
