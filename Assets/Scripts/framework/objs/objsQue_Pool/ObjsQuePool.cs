using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class ObjsQuePool<T, U> : ObjsPool<T, U> where T : MonoBehaviour where U : qObj
{
    public int NumObj(int type) { return _pool.Num; }
    public bool IsFull(byte type) { return _pool.IsFull; }

    protected virtual bool IsShuffleOnAwake() { return false; }

    protected QuePool<U> _pool = null;

    protected override void _awake()
    {
        base._awake();
        short totalCapacity = 0;
        for (byte type = 0; type < NumType; ++type)
            totalCapacity += getCapacityOfType(type);
        _pool = new QuePool<U>(0, totalCapacity, _cObj);
        if(IsShuffleOnAwake())
            _pool.Shuffle();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unactive(U obj)
    {
        _pool.Unactive(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reactive(Vector3 pos)
    {
        return _pool.Reactive(pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unactive(short cdx)
    {
        _pool.Unactive(cdx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnactiveAll()
    {
        _pool.UnactiveAll();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnactiveAll_Shuffle()
    {
        _pool.UnactiveAll();
        _pool.Shuffle();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Recycle(Vector3 pos)
    {
        return _pool.Recycle(pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reuse(Vector3 pos)
    {
        return _pool.Reuse(pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reuse(float x, float y, float z)
    {
        return _pool.Reuse(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unuse(short cdx)
    {
        _pool.Unuse(cdx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unuse(U obj)
    {
        _pool.Unuse(obj.cdx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unuse(short cdx, Vector3 pos)
    {
        _pool.Unuse(cdx, pos);
    }

    public void UnuseAllGamObj()
    {
        _pool.UnuseAll();
    }


    public void UnAssignAndUnuseAllGamObj()
    {
        _pool.UnassignAndUnuseAll();
    }
}
