using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineEx;

public class pillars : ObjsQuePools<pillars, pillar>
{
    protected override short getCapacityOfType(byte type) { return 64; }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public pillar Active(float x, float y, float z, float yAngle = 0)
    {
        if (_pool[0].IsFull)
            return null;
            
        pillar o = Reactive(0, new Vector3(x, y, z));
        o.transform.localEulerAngles = new Vector3(-2+RandEx.GetN(5), -2+RandEx.GetN(5), yAngle);
        o.OnActive();
        return o;
    }


    enum genType{ none=0, none1, none2,colume, row, bothColume, Max}
    public void Generate()
    {
        UnactiveAll();
        float x, y, z = 6.0f + RandEx.GetN(6);
        genType pre = genType.none, cur;
        for(int i=0; i<60 ; ++i)
        {
            y = -17+ RandEx.GetN(16);
            cur = (genType)RandEx.GetN((int)genType.Max);
            if(pre == cur)
                continue;
            z += 8 + RandEx.GetN(10);
            switch(cur)
            {
                case genType.colume:
                if(RandEx.IsTrueOrNot())
                    x = -RandEx.GetN(1, 5);
                else
                    x = cells.MaxX + RandEx.GetN(1, 5);
                Active(x, y, z);
                break;

                case genType.row:
                x = 6.5f;
                Active(x, y, z, 90);
                pre = cur;
                break;
                
                case genType.bothColume:
                Active(-RandEx.GetN(1, 5), y, z+RandEx.GetN(4));
                Active(cells.MaxX + RandEx.GetN(1, 5), y, z+RandEx.GetN(4));
                pre = cur;
                break;

                default:
                break;
            }
        }
    }
}
