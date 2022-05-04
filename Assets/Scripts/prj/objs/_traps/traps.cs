using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.GenericEx;
using System.Collections;
using UnityEngine;

public class traps : ObjsQuePools<traps, trap>
{
    public const byte typePike = 0, typeBigPike = 1;
    short[] _numClones = new short[] { 64, 48 };
    protected override short getCapacityOfType(byte type) { return _numClones[type]; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public trap ActivePike(cell cell)
    {
        if (_pool[typePike].IsFull)
            return null;
        trap o = Reactive(typePike, new Vector3(cell.ct.x, 0.0f, cell.ct.z));
        o.OnActivePike(cell);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public trap ActiveBigPike(cell cell)
    {
        if (_pool[typeBigPike].IsFull)
            return null;
        trap o = Reactive(typeBigPike, new Vector3(cell.ct.x, 0.4f, cell.ct.z));
        o.OnActiveBigPike(cell);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inactive(cell c)
    {
        trap o = _cObj[c.cdx];
        Unactive(o.type, c.cdx);
        c.cdx = -1;
    }
    
}
