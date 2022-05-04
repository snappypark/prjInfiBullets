using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effParticles : ObjsQuePools<effParticles, effParticle>
{
    public const byte hitWall = 0, hitHero = 1, numType = 2;
    float[] _times = new float[] { 0.5f, 0.5f};
    short[] _capacits = new short[] { 32, 8};
    protected override short getCapacityOfType(byte type) { return _capacits[type]; }
    protected override bool getInitActiveOfType(byte type) { return true; }

    protected override void _awake()
    {
        base._awake();
        for (byte i = 0; i < numType; ++i)
            _tasks[i].Awake(i);
    }
    
    public effParticle Create(byte type, Vector3 pos )
    {
        if(_pool[type].IsFull)
            return null;
        effParticle ef = _pool[type].Reuse(pos);
        _tasks[type].Q.Enqueue(new task(Time.time + _times[type], ef.cdx));
        for (int i = 0; i < ef.PS.Length; ++i)
            ef.PS[i].Play();
        return ef;
    }

    void Update()
    {
        for (byte i = 0; i < numType; ++i)
            _tasks[i].Update();
    }



    effsTasks[] _tasks = new effsTasks[numType] { new effsTasks(), new effsTasks(), };
    class effsTasks
    {
        public Queue<task> Q = new Queue<task>();
        byte _type; task _cur;

        public void Awake(byte type) { _type = type; _cur = new task(float.MaxValue, -1); }

        public void Update()
        {
            if (_cur.endTime < Time.time)
            { 
                ofts.particles.Unuse(_type, _cur.cdx);
                _cur = new task(float.MaxValue, -1);
            }
            else if(_cur.cdx == -1 && Q.Count > 0)
                _cur = Q.Dequeue();
        }
    }

    struct task
    { 
        public float endTime; public short cdx;
        public task(float endTime_, short cdx_) { endTime = endTime_; cdx = cdx_; } 
    }
}
