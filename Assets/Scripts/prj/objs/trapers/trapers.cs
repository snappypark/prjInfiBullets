using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineEx;

public class trapers : ObjsQuePool<trapers, traper>
{
    protected override short getCapacityOfType(byte type) { return 32; }
   
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public traper Active(cell cell)
    {
        switch(RandEx.GetN(3)) {
            case 0: return Active_Left(cell);
            case 1: return Active_Right(cell);
            default: return Active_Straight(cell);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public traper Active_Straight(cell cell)
    {
        if (_pool.IsFull)
            return null;
        traper o = Reactive(new Vector3(cell.ct.x, 0.0f, cell.ct.z));
        o.OnActiveStraight(cell);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public traper Active_Left(cell cell)
    {
        if (_pool.IsFull)
            return null;
        traper o = Reactive(new Vector3(cell.ct.x, 0.0f, cell.ct.z));
        o.OnActiveLeft(cell);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public traper Active_Right(cell cell)
    {
        if (_pool.IsFull)
            return null;
        traper o = Reactive(new Vector3(cell.ct.x, 0.0f, cell.ct.z));
        o.OnActiveRight(cell);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inactive(cell info)
    {
        Unactive(info.cdx);
        info.cdx = -1;
    }
}
