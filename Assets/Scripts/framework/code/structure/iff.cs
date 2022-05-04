using UnityEngine;

[System.Serializable]
public struct iff
{
    [SerializeField] private int _i1;
    [SerializeField] private float _f2;
    [SerializeField] private float _f3;

    public iff(int i1_ = 0, float f2_ = 0, float f3_ = 0)
    {
        this._i1 = i1_;
        this._f2 = f2_;
        this._f3 = f3_;
    }
    
    public int i  { get { return _i1; }
                      set { _i1 = value; } }
    public float x  { get { return _f2; }
                      set { _f2 = value; } }
    public float y { get { return _f3; }
                     set { _f3 = value; } }
    public float z { get { return _f3; }
                     set { _f3 = value; } }

    public static iff operator +(iff left, iff right)
    {
        return new iff(left.i + right.i, left.x + right.x, left.z + right.z);
    }

    public override string ToString()
    {
        return string.Format("({0},{1},{2})", _i1, _f2, _f2);
    }
}
