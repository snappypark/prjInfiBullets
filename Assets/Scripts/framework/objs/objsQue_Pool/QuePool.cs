using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineEx;

public class QuePool<U> where U : qObj
{
    public short Capacity { get { return _capacity; } }
    public short Num { get { return (short)(_capacity - _waitCdxes.Count); } }
    public bool IsFull { get { return _waitCdxes.Count == 0; } }
    public bool IsNotFull { get { return _waitCdxes.Count != 0; } }

    short _capacity = 0;
    short _cdx_begin = 0;

    Queue<short> _waitCdxes = new Queue<short>();

    U[] _objs = null;

    public QuePool(short cdx_begin, short capacity, U[] objs)
    {
        _objs = objs;
        _capacity = capacity;
        _cdx_begin = cdx_begin;

        for (short i = 0; i < _capacity; ++i)
            _waitCdxes.Enqueue((short)(_cdx_begin + i));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reactive(Vector3 pos)
    {
        // if (_waitCdxes.Count == 0)
        //     return null;
        U obj = _objs[_waitCdxes.Dequeue()];
        obj.transform.localPosition = pos;
        obj.gameObject.SetActive(true);
        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unactive(U obj)
    {
        _waitCdxes.Enqueue(obj.cdx);
        obj.gameObject.SetActive(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unactive(short cdx)
    {
        _waitCdxes.Enqueue(cdx);
        _objs[cdx].gameObject.SetActive(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnactiveAll()
    {
        _waitCdxes.Clear();
        for (short i = 0; i < _capacity; ++i)
            Unactive(_objs[_cdx_begin + i]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Recycle(Vector3 pos)
    {
        short cdx = _waitCdxes.Dequeue();
        _waitCdxes.Enqueue(cdx);
        U obj = _objs[cdx];
        obj.transform.localPosition = pos;
        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reuse(Vector3 pos)
    {
        U obj = _objs[_waitCdxes.Dequeue()];
        obj.transform.localPosition = pos;
        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public U Reuse(float x, float y, float z)
    {
        U obj = _objs[_waitCdxes.Dequeue()];
        obj.transform.localPosition = new Vector3(x, y, z);
        return obj;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unuse(short cdx)
    { //if (_waitCdxes.Count > _capacity) Debug.LogWarning("_waitCdxes.Count > _capacity");
        _objs[cdx].transform.localPosition = VectorEx.Huge;
        _waitCdxes.Enqueue(cdx);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unuse(short cdx, Vector3 pos)
    {//if (_waitCdxes.Count > _capacity) Debug.LogWarning("_waitCdxes.Count > _capacity");
        _objs[cdx].transform.localPosition = pos;
        _waitCdxes.Enqueue(cdx);
    }

    public void Unuse(U obj)
    {//if (_waitCdxes.Count > _capacity) Debug.LogWarning("_waitCdxes.Count > _capacity");
        obj.transform.localPosition = VectorEx.Huge;
        _waitCdxes.Enqueue(obj.cdx);
    }

    public void UnuseAll()
    {
        _waitCdxes.Clear();
        for (short i = 0; i < _capacity; ++i)
        {
            _objs[_cdx_begin + i].transform.localPosition = VectorEx.Huge;
            Unuse(_objs[_cdx_begin + i]);
        }
    }


    public void UnassignAndUnuseAll()
    {
        _waitCdxes.Clear();
        for (short i = 0; i < _capacity; ++i)
        {
            U obj = _objs[_cdx_begin + i];
            // obj.UnAssign();
            Unuse(obj);
        }
    }

    public void Shuffle()
    {
        int num = _waitCdxes.Count>>1;
        for(int i=0; i<num; ++i)
        {
            short cdx = _waitCdxes.Dequeue();
            int remain = Random.Range(0, _waitCdxes.Count>>1);
            for(int j=0; j<remain; ++j)
                _waitCdxes.Enqueue(_waitCdxes.Dequeue());
            _waitCdxes.Enqueue(cdx);
        }

    }
}
