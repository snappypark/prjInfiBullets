using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjsShortQuePool<T, U> : ObjsPool<T, U> where T : MonoBehaviour where U : sqObj
{
    public int NumObj
    {
        get
        {
            int numObj = 0;
            for (int i = 0; i < NumType; ++i)
                numObj += _pool[i].Num;
            return numObj;
        }
    }

    protected ShortQuePool<U>[] _pool = null;

    protected override void _awake()
    {
        base._awake();

        short cntCdx_atBegin = 0;
        _pool = new ShortQuePool<U>[NumType];
        for (byte t = 0; t < NumType; ++t)
        {
            short capacity = getCapacityOfType(t);
            _pool[t] = new ShortQuePool<U>(cntCdx_atBegin, capacity, _cObj);
            cntCdx_atBegin += capacity;
        }
    }

    public void UnuseAllGamObj()
    {
        for (int objtype = 0; objtype < NumType; ++objtype)
            _pool[objtype].UnuseAll();
    }

    public void Query_IfTrueReturn(Func<U, bool> callback)
    {
        for (int type = 0; type < NumType; ++type)
        {
            Queue<short> q = _pool[type].Q;
            int numObj = q.Count;
            for (int i = 0; i < numObj; ++i)
            {
                U obj = _cObj[q.Dequeue()];
                q.Enqueue(obj.cdx);
                if (callback(obj))
                    return;
            }
        }
    }

    public void TotalQuery(Action<U> callback)
    {
        for (int type = 0; type < NumType; ++type)
        {
            Queue<short> q = _pool[type].Q;
            int numObj = q.Count;
            for (int i = 0; i < numObj; ++i)
            {
                U obj = _cObj[q.Dequeue()];
                q.Enqueue(obj.cdx);
                callback(obj);
            }
        }
    }

    public void TotalCmpQuery(Action<U, U> callback)
    {
        for (int typeObj = 0; typeObj < NumType; ++typeObj)
        {
            Queue<short> q = _pool[typeObj].Q;
            int numObj = q.Count;
            for (int i = 0; i < numObj; ++i)
            {
                U obj = _cObj[q.Dequeue()];
                _totalCmpQuery_rec(q, obj, callback);
                q.Enqueue(obj.cdx);
            }
        }
    }

    void _totalCmpQuery_rec(Queue<short> q, U obj1, Action<U, U> callback)
    {
        int numObj = q.Count;
        for (int i = 0; i < numObj; ++i)
        {
            U obj2 = _cObj[q.Dequeue()];
            q.Enqueue(obj2.cdx);
            callback(obj1, obj2);
        }

        if (numObj > 1)
        {
            U obj = _cObj[q.Dequeue()];
            _totalCmpQuery_rec(q, obj, callback);
            q.Enqueue(obj.cdx);
        }
    }


}
