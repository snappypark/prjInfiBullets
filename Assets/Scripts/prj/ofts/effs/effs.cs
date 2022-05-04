using UnityEngine;
using UnityEngineEx;

public partial class effs : ObjsQuePools<effs, eff>
{
    public const byte tEdge = 0, tBallEff = 1, tStageClear = 2, tHitMain = 3,
                      tLvUp = 4, numType = 5;
    short[] _capacits = new short[] { 96, 256, 4, 2, 1 };
    protected override short getCapacityOfType(byte type) { return _capacits[type]; }
    protected override bool getInitActiveOfType(byte type) { return true; }

    protected override void _awake()
    {
        base._awake();
        for (int i = 0; i < numType; ++i)
            _tasks[i].Init(this);
    }
    
    public eff ExplosionO(Vector3 pos)
    {
        eff ef = _pool[tEdge].Reuse(pos);
        if (null != ef)
        {
            _tasks[tEdge].Add(Time.time + 1.7f, ef.cdx);
            for (int i = 0; i < ef.PS.Length; ++i)
                ef.PS[i].Play();
        }
        return ef;
    }

    public eff ExplosionI(Vector3 pos, Color color, Vector3 dir)
    {
        if(_pool[tBallEff].IsFull)
            return null;
        eff ef = _pool[tBallEff].Reuse(pos);
        if (null != ef)
        {
            ef.transform.up = dir;
            for (int i = 0; i < ef.PS.Length; ++i)
            {
                ef.PS[i].Play();
                ef.PS[i].SetBeginColor(color.Alpha(0.5f));
            }
            _tasks[tBallEff].Add(Time.time + 0.96f, ef.cdx);
        }
        return ef;
    }

    public eff Create(byte type, Vector3 pos )
    {
        if(_pool[type].IsFull)
            return null;
        eff ef = _pool[type].Reuse(pos);
        if (null != ef)
        {
            _tasks[type].Add(Time.time + 3.7f, ef.cdx);
            for (int i = 0; i < ef.PS.Length; ++i)
                ef.PS[i].Play();
        }
        return ef;
    }
}
