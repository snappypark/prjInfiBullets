using UnityEngine;

[System.Serializable]
public struct f2
{
    public static f2 x1 = new f2(1, 0);
    public static f2 z1 = new f2(0, 1);
    public static f2 x_1 = new f2(-1, 0);
    public static f2 z_1 = new f2(0, -1);

    [SerializeField] float _f1;
    [SerializeField] float _f2;

    public f2(float f1_ = 0, float f2_ = 0) { this._f1 = f1_; this._f2 = f2_; }

    public float v1 { get { return _f1; } set { _f1 = value; } }
    public float v2 { get { return _f2; } set { _f2 = value; } }

    public float x { get { return _f1; } set { _f1 = value; } }
    public float y { get { return _f2; } set { _f2 = value; } }
    public float z { get { return _f2; } set { _f2 = value; } }

    public override string ToString() {  return string.Format("({0},{1})", _f1, _f2);  }
    public override int GetHashCode() { return (int)_f1 ^ (int)_f2; }
    public override bool Equals(object obj)  {
        if (!(obj is Pt)) return false;
        Pt comp = (Pt)obj;
        return comp.x == this.x && comp.y == this.y && comp.z == this.z;
    }

    public static bool operator ==(f2 left, f2 right) { return left.v1 == right.v1 && left.v2 == right.v2; }
    public static bool operator !=(f2 left, f2 right) { return !(left == right); }
    public static f2 operator +(f2 left, f2 right) { return new f2(left.x + right.x, left.y + right.y); }
    public static f2 operator -(f2 left, f2 right) { return new f2(left.x - right.x, left.y - right.y); }
    public static f2 operator *(f2 left, float right) { return new f2(left.x * right, left.y * right); }

    public f2 normalized    { get{ float l = Magnitude(); return new f2(_f1/l, _f2/l);    }}
    public void Normalize()
    {
        float l = Magnitude();
        _f1 = _f1/l;
        _f2 = _f2/l;
    }

    public float Magnitude()
    {
        return Mathf.Sqrt(_f1 * _f1 + _f2 * _f2);
    }
    public float SqrMagnitude()
    {
        return _f1 * _f1 + _f2 * _f2;
    }
    public static float SqrMagnitude(f2 v1, f2 v2)
    {
        float x = v2.x - v1.x;
        float z = v2.z - v1.z;
        return x*x + z * z;
    }

    public f2 CrossProduct()
    {
        return new f2(_f2, -_f1);
    }
    public float Dot(f2 v0 )
    {
        return _f1 * v0.x + _f2 * v0.z;
    }
    public float DotXZ(Vector3 v0)
    {
        return _f1 * v0.x + _f2 * v0.z;
    }

    public static f2 VecXZ(Transform from, Transform to)
    {
        return new f2(to.localPosition.x - from.localPosition.x, 
            to.localPosition.z - from.localPosition.z);
    }
    public static f2 VecXZ(Vector3 to, Vector3 from)
    {
        return new f2(to.x - from.x, to.z - from.z);
    }
    public static float DotXZ(Vector3 v0, Vector3 v1)
    {
        return v0.x * v1.x + v0.z * v1.z;
    }

    public static float SqrMagnitudeXZ(f2 v0, f2 v1)
    {
        float x = v0.x - v1.x;
        float z = v0.z - v1.z;
        return x * x + z * z;
    }

}
