using UnityEngine;

public class _jpss : ObjsQuePools<_jpss, _jps>
{
    [SerializeField] GameObject _ground;
    short[] _numClones = new short[] { 1 };
    protected override short getCapacityOfType(byte type) { return _numClones[type]; }

    public _jps Active(cell info)
    {
        if (_pool[0].IsFull)
            return null;
        _jps o = Reactive(0, new Vector3(info.ct.x, 0.0f, info.ct.z));
        o.OnActive(info);
        return o;
    }

    public void Inactive(cell info)
    {
        Unactive(0, info.cdx);
        info.cdx = -1;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _ground.SetActive(false);
            for (int x = 1; x < cells.MaxX; ++x)
            for (int z = 1; z < cells.MaxZ; ++z) {
                cell c = cells.Get(x, z);
                Active(c);
            }

        }
    }
}
