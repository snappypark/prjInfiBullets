using UnityEngine;

[System.Serializable]
public struct i3
{
    [SerializeField]
    private int _i1;
    [SerializeField]
    private int _i2;
    [SerializeField]
    private int _i3;

    public i3(int i1_ = 0, int i2_ = 0, int i3_ = 0)
    {
        this._i1 = i1_;
        this._i2 = i2_;
        this._i3 = i3_;
    }

    public int ky { get { return _i1; } set { _i1 = value; } }
    public int v1 { get { return _i1; } set { _i1 = value; } }
    public int v2 { get { return _i2; } set { _i2 = value; } }
    public int v3 { get { return _i3; } set { _i3 = value; } }

    public int begin { get { return _i1; } set { _i1 = value; } }
    public int middle { get { return _i2; } set { _i2 = value; } }
    public int end { get { return _i3; } set { _i3 = value; } }

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
        get { return _i3; }
        set { _i3 = value; }
    }

    public float SqrMagnitude()
    {
        return _i1 * _i1 + _i2 * _i2 + _i3 * _i3;
    }

    public static bool operator ==(i3 left, i3 right)
    {
        return left.v1 == right.v1 && left.v2 == right.v2 && left.v3 == right.v3;
    }

    public static bool operator !=(i3 left, i3 right)
    {
        return !(left == right);
    }

    public static i3 operator +(i3 left, i3 right)
    {
        return new i3(left.v1 + right.v1, left.v2 + right.v2, left.v3 + right.v3);
    }

    public override string ToString()
    {
        return string.Format("({0},{1},{2})", _i1, _i2, _i3);
    }

}
