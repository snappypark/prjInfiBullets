using UnityEngine;

[System.Serializable]
public struct b3
{
    [SerializeField]
    private byte _b1;
    [SerializeField]
    private byte _b2;
    [SerializeField]
    private byte _b3;

    public b3(byte b1_ = 0, byte b2_ = 0, byte b3_ = 0)
    {
        this._b1 = b1_;
        this._b2 = b2_;
        this._b3 = b3_;
    }

    public byte v1
    {
        get { return _b1; }  set { _b1 = value; }
    }
    public byte v2
    {
        get { return _b2; } set { _b2 = value; }
    }
    public byte v3
    {
        get { return _b3; } set { _b3 = value; }
    }

}

[System.Serializable]
public struct b4
{
    [SerializeField]
    private byte _b1;
    [SerializeField]
    private byte _b2;
    [SerializeField]
    private byte _b3;
    [SerializeField]
    private byte _b4;

    public b4(byte b1_ = 0, byte b2_ = 0, byte b3_ = 0, byte b4_ = 0)
    {
        this._b1 = b1_;
        this._b2 = b2_;
        this._b3 = b3_;
        this._b4 = b4_;
    }

    public byte v1
    {
        get { return _b1; }
        set { _b1 = value; }
    }
    public byte v2
    {
        get { return _b2; }
        set { _b2 = value; }
    }
    public byte v3
    {
        get { return _b3; }
        set { _b3 = value; }
    }
    public byte v4
    {
        get { return _b4; }
        set { _b4 = value; }
    }
}

