using UnityEngine;

[System.Serializable]
public struct i2
{
    [SerializeField]
    private int _i1;
    [SerializeField]
    private int _i2;
    
    public i2(int i1_ = 0, int i2_ = 0)
    {
        this._i1 = i1_;
        this._i2 = i2_;
    }

    public int v1 { get { return _i1; } set { _i1 = value; } } 
    public int v2 { get { return _i2; } set { _i2 = value; } }

    public int min { get { return _i1; } set { _i1 = value; } }
    public int max { get { return _i2; } set { _i2 = value; } }
    public int begin { get { return _i1; } set { _i1 = value; } }
    public int end { get { return _i2; } set { _i2 = value; } }

    public int x
    {
        get { return _i1; }
        set { _i1 = value; }
    }

    public int y
    {
        get { return _i2; }
        set { _i2 = value; }
    }

    public int z
    {
        get { return _i2; }
        set { _i2 = value; }
    }

    public float SqrMagnitude()
    {
        return _i1 * _i1 + _i2 * _i2;
    }

    public static bool operator ==(i2 left, i2 right)
    {
        return left.v1 == right.v1 && left.v2 == right.v2;
    }

    public static bool operator !=(i2 left, i2 right)
    {
        return !(left == right);
    }

    public static i2 operator +(i2 left, i2 right)
    {
        return new i2(left.v1 + right.v1, left.v2 + right.v2);
    }

    public static float SqrDist(int x0, int z0, int x1, int z1)
    {
        int gapx = x1 - x0;
        int gapz = z1 - z0;
        return gapx * gapx + gapz * gapz;
    }

    public static float Dist(int x0, int z0, int x1, int z1)
    {
        int gapx = x1 - x0;
        int gapz = z1 - z0;
        return Mathf.Sqrt(gapx * gapx + gapz * gapz);
    }

    public override string ToString()
    {
        return string.Format("({0},{1})", _i1, _i2);
    }



}
