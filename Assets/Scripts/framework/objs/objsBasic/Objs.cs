using UnityEngine;
using UnityEngineEx;

public class Obj : MonoBehaviour
{
    [HideInInspector] public byte type;
}

public class Objs<T, U> : MonoSingleton<T> where T : MonoBehaviour where U : Obj
{
    [HideInInspector] public int NumType;
    Transform _rootPreLoadObjs;

    void Awake()
    {
        _awake();
    }

    protected virtual void _awake()
    {
        _rootPreLoadObjs = transform.GetChild(0);
        NumType = _rootPreLoadObjs.childCount;
    }

    public U CloneObj(byte objtype, Transform parent)
    {
        U obj = GamObjEx.Create<U>(objtype, _rootPreLoadObjs, parent);
        obj.type = objtype;
        return obj;
    }
}
