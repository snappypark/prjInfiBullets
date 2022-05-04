using UnityEngine;

public class ObjsPool<T, U> : Objs<T, U> where T : MonoBehaviour where U : cObj
{
    Transform _rootClones;
    protected virtual short getCapacityOfType(byte type) { return 1; }
    protected virtual bool getInitActiveOfType(byte type) { return false; }
    protected U[] _cObj = null;

    public U this[int cdx] { get { return _cObj[cdx]; } }

    protected override void _awake()
    {
        base._awake();

        _rootClones = transform.GetChild(1);

        short totalCapacity = 0;
        for (byte type = 0; type < NumType; ++type)
            totalCapacity += getCapacityOfType(type);
        _cObj = new U[totalCapacity];

        short cntCdx = 0;
        for (byte type = 0; type < NumType; ++type)
        {
            int capacity = getCapacityOfType(type);
            bool initActive = getInitActiveOfType(type);
            for (int i = 0; i < capacity; ++i)
            {
                U obj = CloneObj(type, _rootClones);
                obj.transform.localPosition = Vector3.zero;
                obj.cdx = cntCdx++;
                obj.gameObject.SetActive(initActive);
                _cObj[obj.cdx] = obj;
            }
        }
    }



}
