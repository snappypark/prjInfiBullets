using UnityEngine;

[System.Serializable]
public struct i4
{
    public static i4 O = new i4(0, 0, 0, 0);
    [SerializeField] int x0, z0, x1, z1;

    public i4(int x0_, int z0_, int x1_, int z1_) {
        x0 = x0_; z0 = z0_; x1 = x1_; z1 = z1_; }

    public i4(Pt pt0, Pt pt1)
    {
        x0 = pt0.x; z0 = pt0.z; x1 = pt1.x; z1 = pt1.z;
    }

    public int left { get { return x0; } }
    public int bottom { get { return z0; } }
    public int right { get { return x1; } }
    public int top { get { return z1; } }

    public int X0 { get { return x0; } }
    public int Z0 { get { return z0; } }
    public int X1 { get { return x1; } }
    public int Z1 { get { return z1; } }

    public float I1 { get { return x0; } }
    public float I2 { get { return z0; } }
    public float I3 { get { return x1; } }
    public float I4 { get { return z1; } }

    public int xMin { get { return x0; } }
    public int yMin { get { return z0; } }
    public int zMin { get { return z0; } }

    public int xMax { get { return x1; } }
    public int yMax { get { return z1; } }
    public int zMax { get { return z1; } }
    
    public int xMidd { get {
            int half = (GapX+ Random.Range(0, 2)) >> 1;
            return x0 + half;  } }
    public int zMidd { get {
            int half = (GapZ + Random.Range(0, 2)) >> 1;
            return z0 + half;  } }
    
    public int xMid { get { return x0 + (GapX >> 1); } }
    public int zMid { get { return z0 + (GapZ >> 1); } }

    public int GapX { get { return x1 - x0; } }
    public int GapZ { get { return z1 - z0; } }

    public float CenterOf_X01
    {
        get { return x0 + 0.5f + (x1 - x0)*0.5f; }
    }

    public float CenterOf_Z01
    {
        get { return z0 + 0.5f + (z1 - z0) * 0.5f; }
    }

    public bool HasPt(int x, int z)
    {
        return x0 <= x && z0 <= z && x <= x1 && z <= z1;
    }

    public bool IsCollided(i4 r)
    {
        int sizeX = (1 + x1 - x0) + (1 + r.x1 - r.x0);
        int sizeZ = (1 + z1 - z0) + (1 + r.z1 - r.z0);
        int minX = Mathf.Min(x0, r.x0);
        int maxX = Mathf.Max(x1, r.x1);
        int minZ = Mathf.Min(z0, r.z0);
        int maxZ = Mathf.Max(z1, r.z1);
        int lenghX = maxX - minX + 1;
        int lenghZ = maxZ - minZ + 1;
        return lenghX < sizeX && lenghZ < sizeZ;
    }

    public bool IsContacted(i4 r)
    {
        int sizeX = (1 + x1 - x0) + (1 + r.x1 - r.x0);
        int sizeZ = (1 + z1 - z0) + (1 + r.z1 - r.z0);
        int minX = Mathf.Min(x0, r.x0);
        int maxX = Mathf.Max(x1, r.x1);
        int minZ = Mathf.Min(z0, r.z0);
        int maxZ = Mathf.Max(z1, r.z1);
        int lenghX = maxX - minX + 1;
        int lenghZ = maxZ - minZ + 1;
        return (lenghX < sizeX && lenghZ == sizeZ) ||
                (lenghX == sizeX && lenghZ < sizeZ);
    }

    public bool canInvolve(i4 r)
    {
        return (x0 <= r.x0 && z0 <= r.z0 &&
                x1 >= r.x1 && z1 >= r.z1);
    }

    public bool canInvolve(int x, int z)
    {
        return (x0 <= x && x <= x1 &&
                z0 <= z && z <= z1);
    }

    public i4 WithX(int gap)
    {
        return new i4(x0 + gap, z0, x1 + gap, z1);
    }

    public i4 WithZ(int gap)
    {
        return new i4(x0, z0 + gap, x1, z1 + gap);
    }

    public static i4 N(int x0_, int z0_, int x1_, int z1_)
    {
        return new i4(x0_, z0_, x1_, z1_);
    }
}