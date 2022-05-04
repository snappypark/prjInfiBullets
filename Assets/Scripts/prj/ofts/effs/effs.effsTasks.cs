using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class effs /* effsTasks*/
{
    effsTasks[] _tasks = new effsTasks[numType] { new effsTasks(), new effsTasks(), new effsTasks(), new effsTasks(), new effsTasks() };

    //nj.skip effUpdate = new nj.skip(2);
    void Update()
    {
        for (int i = 0; i < numType; ++i)
            _tasks[i].Update();
    }

    private class effsTasks
    {
        Queue<task> _q = new Queue<task>();
        effs _effs;
        task _cur;

        public void Init(effs effs_)
        {
            _effs = effs_;
            _cur = new task(-1, -1);
        }

        public void Update()
        {
            if (_cur.cdx != -1 )
            {
                if (_cur.endTime < Time.time)
                { 
                    _effs.Unuse(_effs[_cur.cdx]);
                    _cur.cdx = -1;
                }
            }
            else if (_q.Count > 0)
            {
                _cur = _q.Dequeue();
            }
        }

        public void Add(float endTime_, short cdx_)
        {
            _q.Enqueue(new task(endTime_, cdx_));
        }
    }

    private struct task
    {
        public float endTime;
        public short cdx;

        public task(float endTime_, short cdx_)
        {
            endTime = endTime_;
            cdx = cdx_;
        }
    }
}
