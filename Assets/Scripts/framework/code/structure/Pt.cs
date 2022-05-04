using UnityEngine;
using System;

[System.Serializable]
public struct Pt
{
    public static readonly Pt Huge = new Pt(11921, 11921);
    public static readonly Pt Empty = new Pt();

    bool Has3Elements;
    [SerializeField]
    private int m_x;
    [SerializeField]
    private int m_y;
    private int m_z;

    public Pt(int x = 0, int y = 0)
    {
        this.m_x = x;
        this.m_y = y;
        this.m_z = 0;
        Has3Elements = false;
    }

    public Pt(int x, int y, int z)
    {
        this.m_x = x;
        this.m_y = y;
        this.m_z = z;
        Has3Elements = true;
    }

    public bool IsEmpty { get { return m_x == 0 && m_y == 0 && m_z == 0; } }

    public int x
    {
        get { return m_x; }
        set { m_x = value; }
    }

    public int y
    {
        get { return m_y; }
        set { m_y = value; }
    }

    public int z
    {
        get { return m_z; }
        set { m_z = value; }
    }

    public static bool operator ==(Pt left, Pt right)
    {
        return left.x == right.x && left.y == right.y && left.z == right.z;
    }

    public static bool operator !=(Pt left, Pt right)
    {
        return !(left == right);
    }

    public static Pt operator+ (Pt left, Pt right)
    {
        Pt pt = new Pt(left.x + right.x, left.y + right.y);
        return pt;
    }
    /*
    public static Pt Ceiling(Pt value)
    {
        return new Pt((int)Math.Ceiling((double)value.x),
                        (int)Math.Ceiling((double)value.y),
                        (int)Math.Ceiling((double)value.z));
    }*/

    public static Pt Truncate(Pt value)
    {
        return new Pt((int)value.x, (int)value.y, (int)value.z);
    }

    /*
    public static Pt Round(Pt value)
    {
        return new Pt((int)Math.Round((double)value.x),
                        (int)Math.Round((double)value.y),
                        (int)Math.Round((double)value.z));
    }*/

    public override bool Equals(object obj)
    {
        if (!(obj is Pt)) return false;
        Pt comp = (Pt)obj;
        return comp.x == this.x && comp.y == this.y && comp.z == this.z;
    }

    public override int GetHashCode()
    {
        return m_x ^ m_y ^ m_z;
    }

    public void Offset(int dx, int dy, int dz)
    {
        x += dx;
        y += dy;
        z += dz;
    }

    public void Offset(Pt p)
    {
        Offset(p.x, p.y, p.z);
    }

    public override string ToString()
    {
        if (Has3Elements)
            return string.Format("x:{0},y:{1},z:{2}", x, y, z);
        return string.Format("x:{0},y:{1}", x, y);
    }

    public Vector3 ToVec3()
    {
        return new Vector3(x, y, z);
    }
    // Get Difference between two points, assuming only cardianal or diagonal movement is possible
    public static int diff(i2 a, i2 b)
    {
        // because diagonal
        // 0,0 diff 1,1 = 1 
        // 0,0 diff 0,1 = 1 
        // 0,0 diff 1,2 = 2 
        // 0,0 diff 2,2 = 2 
        // return max of the diff row or diff column
        return Mathf.Max(Mathf.Abs(b.x - a.x), Mathf.Abs(b.z - a.z));
    }
}
