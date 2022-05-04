using System.Runtime.CompilerServices;
using UnityEngine;

public class cubes : ObjsQuePools<cubes, cube>
{
    [SerializeField] public mats_ mats;

    protected override short getCapacityOfType(byte type) { return 256; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public cube Active(cell info)
    {
        if (_pool[0].IsFull)
            return null;
        cube o;
        switch(info.Type) {
            case cell.type.Finish: o = Reactive(0, new Vector3(info.ct.x, 0.01f, info.ct.z)); 
                                    o.transform.localScale = new Vector3(1,0.1f,1); break;
            default:             o = Reactive(0, new Vector3(info.ct.x, 0.0f, info.ct.z)); 
                                o.transform.localScale = new Vector3(1,1,1);break; }
        o.OnActive(info);
        return o;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inactive(cell info)
    {
        Unactive(0, info.cdx);
        info.cdx = -1;
    }
    
    [System.Serializable]
    public class mats_
    {
        public enum Type : byte {W0w=0, W1y, W2o, W3g1, W4g2, W5s, W6b, W7pu, W8pi, W9r, WZb, Finish, Tnt}
        public const byte W0w=0, W1y=1, W2o=2, W3g1=3, W4g2=4, 
                          W5s=5, W6b=6, W7pu=7, W8pi=8, W9r=9, WZb=10, Finish = 11;
        [SerializeField] Material[] _mats;
        public Material this[byte idx] { get { return _mats[idx]; } } 
    }
}


