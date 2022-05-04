using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class ObjsQuePools<T, U> : ObjsPool<T, U> where T : MonoBehaviour where U : qObj
{
    public int NumObj(int type) { return _pool[type].Num; }
    public bool IsFull(byte type) { return _pool[type].IsFull; }

    protected QuePool<U>[] _pool = null;

    protected override void _awake()
    {
        base._awake();

        short cntCdx_atBegin = 0;
        _pool = new QuePool<U>[NumType];
        for (byte t = 0; t < NumType; ++t)
        {
            short capacity = getCapacityOfType(t);
            _pool[t] = new QuePool<U>(cntCdx_atBegin, capacity, _cObj);
            cntCdx_atBegin += capacity;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unactive(U obj)
    {
        _pool[obj.type].Unactive(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reactive(byte type, Vector3 pos)
    {
        return _pool[type].Reactive(pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unactive(byte type, U obj)
    {
        _pool[type].Unactive(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unactive(byte type, short cdx)
    {
        _pool[type].Unactive(cdx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnactiveAll()
    {
        for (int type = 0; type < NumType; ++type)
            _pool[type].UnactiveAll();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Recycle(byte type, Vector3 pos)
    {
        return _pool[type].Recycle(pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reuse(byte type, Vector3 pos)
    {
        return _pool[type].Reuse(pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reuse(byte type, float x, float y, float z)
    {
        return _pool[type].Reuse(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unuse(byte type, short cdx)
    {
        _pool[type].Unuse(cdx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unuse(U obj)
    {
        _pool[obj.type].Unuse(obj.cdx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unuse(byte type, short cdx, Vector3 pos)
    {
        _pool[type].Unuse(cdx, pos);
    }

    public void UnuseAllGamObj()
    {
        for (int type = 0; type < NumType; ++type)
            _pool[type].UnuseAll();
    }


    public void UnAssignAndUnuseAllGamObj()
    {
        for (int type = 0; type < NumType; ++type)
            _pool[type].UnassignAndUnuseAll();
    }
}
