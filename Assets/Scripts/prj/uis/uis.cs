using UnityEngine;

[ExecuteInEditMode]
public class uis : MonoBehaviour
{
    static uis _inst;

    public static bool IsNullInst{get{return _inst == null;}}
    public static float Width { get { return _inst._rt.sizeDelta.x; } }
    public static float Height { get { return _inst._rt.sizeDelta.y; } }

    [SerializeField] RectTransform _rt;
    
    public static ui_form form;
    public static ui_hud hud;
    public static ui_cover cover;
    public static ui_ads ads;
    public static ui_pops pops;

    [SerializeField] ui_form _form;
    
    [SerializeField] ui_hud _hud;
    [SerializeField] ui_cover _cover;
    [SerializeField] ui_ads _ads;
    [SerializeField] ui_pops _pops;
    
    void Awake()
    {
        _inst = this;
        form = _form;
        hud = _hud;
        cover = _cover;
        ads = _ads;
        pops = _pops;
    }
    
    void OnEnable()
    {
        _inst = this;
    }

    public static Vector2 WorldToCanvas(Vector3 pos)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(pos);
        return new Vector2( (ViewportPosition.x - 0.5f) * _inst._rt.sizeDelta.x,
                            (ViewportPosition.y - 0.5f) * _inst._rt.sizeDelta.y );
    }

    public static Vector2 WorldToCanvas_L(Vector3 pos)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(pos);
        return new Vector2((ViewportPosition.x) * _inst._rt.sizeDelta.x,
                            (ViewportPosition.y) * _inst._rt.sizeDelta.y);
    }

    /*
    //now you can set the position of the ui element
    UI_Element.anchoredPosition = WorldObject_ScreenPosition;
    */
    static delay _delayBtn = new delay(0.7f);
    public static bool IsWaitingBtn(float waitTime = 0.7f)
    {
        bool wait = _delayBtn.InTime();
        if (!wait)
            _delayBtn.Reset(waitTime);
        return wait;
    }
}
