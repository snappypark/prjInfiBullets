using UnityEngine;

public abstract class cObj : Obj
{
    [HideInInspector] public short cdx;
    public virtual void OnUnuse() { }

}
