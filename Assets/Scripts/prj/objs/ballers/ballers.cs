using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;

public class ballers : ObjsQuePool<ballers, baller>
{
    public const short max = 28;
    protected override short getCapacityOfType(byte type) { return max; }

    static Queue<short> _curCdxes = new Queue<short>();

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void Active(cell info)
    {
        baller.data data = baller.datas[info.ddx];

        cell c = data.begin;
        if(!c.HasObj)
        {
            baller o = Reactive(new Vector3(c.ct.x, 0.45f, c.ct.z));
            o.OnActive(c, data.beginHasLine, true);
            _curCdxes.Enqueue(o.cdx);
            c.cdx = o.cdx;
        }
        
        c = data.end;
        if(c != null && !c.HasObj)
        {
            baller o = Reactive(new Vector3(c.ct.x, 0.45f, c.ct.z));
            o.OnActive(c, data.endHasLine, false);
            c.cdx = o.cdx;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inactive(cell info)
    {
        Unactive(info.cdx);
        info.cdx = -1;
    }

    public void Clear()
    {
        UnactiveAll();
        _curCdxes.Clear();
        baller.ClearData();
    }
    
    public static void SpawnOnPulse()
    {
#if UNITY_EDITOR
    if(flowEdit.IsOn)
        return;
#else
#endif
        if(_curCdxes.Count == 0)
            return;
        baller o = objs.ballers[_curCdxes.Dequeue()];
        if(o.gameObject.activeSelf)
        {
            _curCdxes.Enqueue(o.cdx);
            o.Spawn();
        }
    }
    

}
