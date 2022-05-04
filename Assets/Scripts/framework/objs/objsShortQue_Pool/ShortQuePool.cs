using System.Collections.Generic;
using System.Collections.GenericEx;
using UnityEngine;
using UnityEngineEx;

public class ShortQuePool<U> where U : sqObj
{
    public Queue<short> Q { get { return _usedCdxes; } }

    public short Capacity { get { return _capacity; } }
    public short Num { get { return (short)_usedCdxes.Count; } }

    short _capacity = 0;
    short _cdx_begin = 0;

    Queue<short> _waitCdxes = new Queue<short>();
    Queue<short> _usedCdxes = new Queue<short>();
    U[] _objs = null;

    public ShortQuePool(short cdx_begin, short capacity, U[] objs)
    {
        _cdx_begin = cdx_begin;
        _objs = objs;
        _capacity = capacity;

        for (int i = 0; i < _capacity; ++i)
            _waitCdxes.Enqueue((short)(_cdx_begin + i));
    }

    public U Reuse(Vector3 pos)
    {
        if (_waitCdxes.Count == 0)
            return null;

        short cdx = _waitCdxes.Dequeue();
        _usedCdxes.Enqueue(cdx);

        U obj = _objs[cdx];
        obj.transform.localPosition = pos;
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void Unuse(U obj)
    {
        if (_usedCdxes.DequeueOne(obj.cdx))
            _waitCdxes.Enqueue(obj.cdx);
        obj.transform.localPosition = VectorEx.Huge;
        obj.gameObject.SetActive(false);
    }

    public void UnuseAll()
    {
        int num = _usedCdxes.Count;
        for (int i = 0; i < num; ++i)
        {
            short cdx = _usedCdxes.Dequeue();
            _waitCdxes.Enqueue(cdx);
            _objs[cdx].transform.localPosition = VectorEx.Huge;
            _objs[cdx].gameObject.SetActive(false);
        }
    }
}