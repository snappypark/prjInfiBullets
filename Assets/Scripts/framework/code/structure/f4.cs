using UnityEngine;

[System.Serializable]
public struct f4
{
    public static f4 O = new f4(0, 0, 0, 0);
    public static f4 One = new f4(1, 1, 1, 1);
    public static f4 HugeNegative = new f4(-10001, -10001, -10001, -10001);
    [SerializeField] float x0, z0, x1, z1;
    
    public f4(float x0_, float z0_, float x1_, float z1_ = 0.0f)
    {
        x0 = x0_; z0 = z0_; x1 = x1_; z1 = z1_;
    }

    public float left { get { return x0; } }
    public float bottom { get { return z0; } }
    public float right { get { return x1; } }
    public float top { get { return z1; } }

    public float X0 { get { return x0; } }
    public float Z0 { get { return z0; } }
    public float X1 { get { return x1; } }
    public float Z1 { get { return z1; } }

    public float F1 { get { return x0; } }
    public float F2 { get { return z0; } }
    public float F3 { get { return x1; } }
    public float F4 { get { return z1; } }

    public float xMin { get { return x0; } }
    public float yMin { get { return z0; } }
    public float zMin { get { return z0; } }

    public float xMax { get { return x1; } }
    public float yMax { get { return z1; } }
    public float zMax { get { return z1; } }

    public float GapX { get { return x1 - x0; } }
    public float GapZ { get { return z1 - z0; } }
}
