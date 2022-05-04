using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct i5
{
    [SerializeField] int x0, z0, x1, z1, y;

    public i5(int x0_, int z0_, int x1_, int z1_, int y_)
    {
        x0 = x0_; z0 = z0_; x1 = x1_; z1 = z1_; y = y_;
    }

    public i5(Pt pt0, Pt pt1, int y_)
    {
        x0 = pt0.x; z0 = pt0.z; x1 = pt1.x; z1 = pt1.z; y = y_;
    }

    public int left { get { return x0; } }
    public int bottom { get { return z0; } }
    public int right { get { return x1; } }
    public int top { get { return z1; } }

    public int X0 { get { return x0; } }
    public int Z0 { get { return z0; } }
    public int X1 { get { return x1; } }
    public int Z1 { get { return z1; } }
    public int Y { get { return y; } }

    public Pt Pt0 { get { return new Pt(x0, y, z0); } }
    public Pt Pt1 { get { return new Pt(x1, y, z1); } }

    public int xMin { get { return x0; } }
    public int yMin { get { return z0; } }
    public int zMin { get { return z0; } }

    public int xMax { get { return x1; } }
    public int yMax { get { return z1; } }
    public int zMax { get { return z1; } }

    public int GapX { get { return x1 - x0; } }
    public int GapZ { get { return z1 - z0; } }
    public int SizeX { get { return x1 - x0 + 1; } }
    public int SizeZ { get { return z1 - z0 + 1; } }

    public int xMidd { get {
            int half = (GapX+ Random.Range(0, 2)) >> 1;
            return x0 + half;  } }
    public int zMidd { get {
            int half = (GapZ + Random.Range(0, 2)) >> 1;
            return z0 + half;  } }
    
    public int xMid { get { return x0 + (GapX >> 1); } }
    public int zMid { get { return z0 + (GapZ >> 1); } }

    public bool IsCollided(i5 r)
    {
        int sizeX = (1 + x1 - x0) + (1 + r.x1 - r.x0);
        int sizeZ = (1 + z1 - z0) + (1 + r.z1 - r.z0);
        int minX = Mathf.Min(x0, r.x0);
        int maxX = Mathf.Max(x1, r.x1);
        int minZ = Mathf.Min(z0, r.z0);
        int maxZ = Mathf.Max(z1, r.z1);
        int lenghX = maxX - minX + 1;
        int lenghZ = maxZ - minZ + 1;
        return y == r.Y && lenghX < sizeX && lenghZ < sizeZ;
    }

    public bool IsContacted(i5 r)
    {
        if (y != r.Y)
            return false;
        int sizeX = (1 + x1 - x0) + (1 + r.x1 - r.x0);
        int sizeZ = (1 + z1 - z0) + (1 + r.z1 - r.z0);
        int minX = Mathf.Min(x0, r.x0);
        int maxX = Mathf.Max(x1, r.x1);
        int minZ = Mathf.Min(z0, r.z0);
        int maxZ = Mathf.Max(z1, r.z1);
        int totalLenghX = maxX - minX + 1;
        int totalLenghZ = maxZ - minZ + 1;
        return (totalLenghX < sizeX && totalLenghZ == sizeZ) ||
                (totalLenghX == sizeX && totalLenghZ < sizeZ);
    }

    public bool IsInside(i5 r)
    {
        return y == r.Y && x0 <= r.x0 && z0 <= r.z0 &&
                x1 >= r.x1 && z1 >= r.z1;
    }

    public bool HasPt(Pt p)
    {
        return x0 <= p.x && z0 <= p.z &&
                p.x <= x1 && p.z <= z1;
    }

    public bool HasPt(int x, int z)
    {
        return x0 <= x && z0 <= z && x <= x1 && z <= z1;
    }

    public bool IsSame(Pt pt)
    {
        return pt.x == x0 && pt.y == y && pt.z == z0;
    }

    public i4 i4 { get { return new i4(x0, z0, x1, z1);  } }

    public i4 CenterArea(int radiuX, int radiuZ)
    {
        int x = x0 + (GapX >> 1); int z = z0 + (GapZ >> 1);
        return new i4(x - radiuX, z - radiuZ, x + radiuX, z + radiuZ);
    }

    public static i5 Minimize(i5 i1, i5 i2)
    {
        return new i5(
            Mathf.Max(i1.xMin, i2.xMin),
            Mathf.Max(i1.zMin, i2.zMin),
            Mathf.Min(i1.xMax, i2.xMax),
            Mathf.Min(i1.zMax, i2.zMax),
            i1.Y
            );
    }

    public static i5 Maximize(i5 i1, i5 i2)
    {
        return new i5(
            Mathf.Min(i1.xMin, i2.xMin),
            Mathf.Min(i1.zMin, i2.zMin),
            Mathf.Max(i1.xMax, i2.xMax),
            Mathf.Max(i1.zMax, i2.zMax),
            i1.Y
            );
    }

    public static i5 N(int x0_, int z0_, int x1_, int z1_, int y_)
    {
        return new i5(x0_, z0_, x1_, z1_, y_);
    }
}
