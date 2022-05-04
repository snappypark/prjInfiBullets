using UnityEngine;
using System.Collections.GenericEx;
using System.Runtime.CompilerServices;

public partial class cells : MonoBehaviour
{
    static cells _inst;
    public const int MaxX = 13, MaxZ = 89;//99

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static cell Get(int x, int z) { return _inst._pool[x, z];  }
    public static bool IsOut(int x, int z) { return x < 0 || x >= MaxX || z < 0 || z >= MaxZ; }
    
    arr2d<cell> _pool;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static cell SwitchCell(cell cur, short cdx, int newX, int newZ)
    {
        if(cur.pt.x == newX && cur.pt.z == newZ)
            return null;
        cur.cdxs.Deq(cdx);
        cell c = _inst._pool[newX, newZ];
        c.cdxs.Enqueue(cdx);
        return c;
    }
    
    void Awake()
    {
        _inst = this;
        _pool = new arr2d<cell>(MaxX, MaxZ);
        for (int x = 0; x < MaxX; ++x)
            for (int z = 0; z < MaxZ; ++z) {
                cell c = _inst._pool[x, z];
                c.pt = new i2(x,z); 
                c.ct = new f2(x+0.5f, z+0.5f);
                c.pNode.cell = c;
            }
    }
    
    public static void Clear()
    {
        for (int x = bd.X0; x <= bd.X1; ++x)
            for (int z = bd.Z0; z <= bd.Z1; ++z)
                    _inst._pool[x, z].Clear();
    }
}
