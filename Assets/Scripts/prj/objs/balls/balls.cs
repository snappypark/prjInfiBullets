using System.Runtime.CompilerServices;
using UnityEngine;

public partial class balls : ObjsQuePools<balls, ball>
{
    [SerializeField] public mats_ mats;

    short[] _numClones = new short[] { 196 };
    protected override short getCapacityOfType(byte type) { return _numClones[type]; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ball Active(cell info)
    {
        if (_pool[0].IsFull)
            return null;
        ball o = Reactive(0, new Vector3(info.ct.x, 0, info.ct.z));
        o.OnActive(info);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ball ActiveBySpawn(cell info, ref arr<f2> pts, baller.data.type datatype, float spd)
    {
        if (_pool[0].IsFull)
            return null;
        ball o = Reactive(0, new Vector3(info.ct.x, 0, info.ct.z));
        o.OnActiveBySpawn(info, ref pts, datatype, spd);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inactive(short cdx)
    {
        Unactive(0, cdx);
    }
    
    public static arr<f2> pathPts = new arr<f2>(16);
#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static bool FindPath(cell begin, ref arr<f2> pts)
    {
        int backZ = hero.pt.z-5;
        cell goal = cells.Get(hero.pt.x, backZ);
        bool result = makePath(begin, goal, ref pts);
        cells.clearQuq();
        return result;
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    static bool makePath(cell begin, cell end, ref arr<f2> pts)
    {
        if (cells.CalcPathGoal(begin, end, sqr.f36_0))
        {
            pts.Reset_Add(end.ct);
            PathNode cur = end.pNode.from;
            PathNode from = cur.from;
            
            while (from != null) {
                if(pts.IsFull)
                    return false;
                
                pts.Add(cur.cell.ct);
                cur = cur.from;
                from = cur.from;
            }
            return true;
        }
        return false;
    }

    [System.Serializable]
    public class mats_
    {
        public enum Type : byte {B0w=0, B1y, B2o, B3g1, B4g2, B5s, B6b, B7pu, B8pi, B9r, BZb,}
        public const byte B0w=0, B1y=1, B2o=2, B3g1=3, B4g2=4, 
                          B5s=5, B6b=6, B7pu=7, B8pi=8, B9r=9, BZb=10;

        [SerializeField] Material[] _mats;
        public Material this[byte idx] { get { return _mats[idx]; } }
    }
}
