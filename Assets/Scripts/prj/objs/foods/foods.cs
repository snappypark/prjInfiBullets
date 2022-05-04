using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineEx;

public class foods : ObjsQuePool<foods, food>
{
    protected override bool IsShuffleOnAwake() { return true; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public food Active(cell c)
    {
        if (_pool.IsFull)
            return null;
        food o = Reactive(new Vector3(c.ct.x, 0.01f, c.ct.z));
        o.transform.localEulerAngles = new Vector3(90, 0, -170);
        o.OnActive(c);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inactive(cell c)
    {
        c.SetPath();
        Unactive(_cObj[c.cdx]);
        c.cdx = -1;
    }

    public bool CheckAndSpawnFood(cell c)
    {
        if(_maxAppear <= 0)
            return false;
        if(--_countAppear <= 0)
        {
            --_maxAppear;
            _countAppear = RandEx.GetN(_AppearRangeGap.v1, _AppearRangeGap.v2);
            objs.cubes.Inactive(c);
            food o = Active(c);
            c.cdx = o.cdx;
            return true;
        }

        return false;
    }

    i2 _AppearRangeGap;
    int _countAppear, _maxAppear;
    public void InitAppearOption(int stageIdx)
    {
        _countAppear = 0;
        switch(stageIdx)
        {
            default:
            _AppearRangeGap = new i2(8, 14);
            _countAppear = RandEx.GetN(_AppearRangeGap.v1, _AppearRangeGap.v2)>>1;
            _maxAppear = 11;
            break;
        }
    }
}
