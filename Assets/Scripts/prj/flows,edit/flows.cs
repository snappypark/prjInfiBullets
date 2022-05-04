using System.Collections;
using UnityEngine;
using UnityEngineEx;

public class flows : MonoBehaviour
{
    static flows _inst;
    flowAbs _cur = null;
    bool _ready = true;
    void Awake() { _inst = this; }

    public static void Change<T>() where T : flowAbs, new()
    {
        if(_inst._ready)
        {
            _inst._ready = false;
            _inst.StartCoroutine(_inst.change_<T>());
        }
    }
    
    IEnumerator change_<T>() where T : flowAbs, new()
    {
        yield return _cur.OnExit_();
        _cur = new T();
        yield return _cur.OnEnter_();
        _ready = true;
    }

    IEnumerator Start()
    {
        yield return loads.PrjFromJson_();
        dCore.Init();
        yield return null; ads.Inst.Init();
        uis.form.OnActive();
        _cur = new flowPlay();
        yield return _cur.OnEnter_();
        //yield return uis.cover.Img.FadeOut_();
    } 
}

public class flowPlay : flowAbs
{
    public override byte iType { get { return iTypePlay; } }

    int curStage;
    public override IEnumerator OnEnter_()
    {
        curStage = dStage.lv;
        ads.Inst.StartLoad();
        ads.Inst.PreLoad(curStage);
        uis.form.RefreshStage();
        yield return loads.StageFromJson_(dStage.lv);
    }

    public override IEnumerator OnExit_()
    {
        yield return objs.Clear_();
        ads.Inst.Check_ForNextStage(curStage);
    }
}


public class flowEdit : flowAbs
{
    public static bool IsOn = false;

    public override byte iType { get { return iTypeEdit; } }
    
    public override IEnumerator OnEnter_()
    {
        IsOn = true;
        uis.form.gameObject.SetActive(false);
        yield return null;
        hero.Init(hero.Type.Edit);
    }

    public override IEnumerator OnExit_()
    {
        yield return objs.Clear_();
        IsOn = false;
        uis.form.gameObject.SetActive(true);
    }
}

public abstract class flowAbs
{
    public virtual byte iType { get { return iTypeNone; } }
    
    public virtual IEnumerator OnEnter_() { yield return null; }
    public virtual IEnumerator OnExit_() { yield return null; }

    public const byte iTypeNone = 0;
    public const byte iTypePlay = 1;
    public const byte iTypeEdit = 2;
}
