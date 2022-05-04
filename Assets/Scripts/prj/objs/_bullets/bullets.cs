using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineEx;

public class bullets : ObjsQuePools<bullets, bullet>
{
    short[] _numClones = new short[] { 256 };
    protected override short getCapacityOfType(byte type) { return _numClones[type]; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fire(Vector3 pos, float dirX, float dirZ, float spd, Color color)
    {
        if(pos.x < bd.left|| pos.x > bd.right)
            return;
        if (_pool[0].IsFull)
            return;
        bullet b = Reactive(0, pos);
        b.OnActive(spd, color);
        b.transform.forward = new Vector3(dirX, 0, dirZ);
    }
}
