using UnityEngine;
using UnityEngineEx;

public partial class stageEditor : MonoBehaviour
{
    public enum SelectType{ Copy0 = 0, Copy1, Copy2 , Single, Rect,  } 
    [HideInInspector] public SelectType select = SelectType.Rect;
    cell.type _type;  public void SetType(cell.type type){_type = type;}
    [HideInInspector] public cell CurCell;
    [HideInInspector] public bool created = true;
    void OnEnable()
    {
        flows.Change<flowEdit>();
        area.InitOnEdit();
        Application.targetFrameRate = 60;
        created = true;
        initHistorys();
        cam.SetTranInfo(new Vector2(27,-12), 60);
    }

    void OnDisable()
    {
        flows.Change<flowPlay>(); 
        Application.targetFrameRate = 32;
        cam.SetTranInfo(new Vector2(20, -13.5f), 45);
    }
    
    public static bool setAlpha0 = false, setAlpha1 = false, setAlpha2 = false, setAlpha3 = false, setBackQuote = false;
    void FixedUpdate() {
        switch(select){
        case SelectType.Single: onUpdateOnSingleSelectMode(); break;
        case SelectType.Rect: onUpdateOnRectSelectMode(); break; 
        default: onUpdateOnCopyMode((int)select); break; }
        DebugEx.DrawLineOnXZ(1,1, 12,1, Color.black);
        DebugEx.DrawLineOnXZ(1,1, 1,47, Color.black);
        DebugEx.DrawLineOnXZ(12,1, 12,47, Color.black);
        if(Input.GetKeyUp(KeyCode.Alpha0))
        {
            setAlpha0 = true;
            select = SelectType.Single;
        }
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            setAlpha1 = true;
            select = SelectType.Single;
        }
        else if(Input.GetKeyUp(KeyCode.Alpha2))
        {
            setAlpha2 = true;
            select = SelectType.Rect;
        }
        else if(Input.GetKeyUp(KeyCode.Alpha3))
        {
            setAlpha3 = true;
            select = SelectType.Single;
            //select = SelectType.Copy0;
        }
        else if(Input.GetKeyUp(KeyCode.Alpha4))
            select = SelectType.Copy1;
        else if(Input.GetKeyUp(KeyCode.Alpha5))
            select = SelectType.Copy2;
        else if(Input.GetKeyUp(KeyCode.BackQuote))
        {
            setBackQuote = true;
            select = SelectType.Rect;
        }
    }

    void onUpdateOnSingleSelectMode()
    {
        if (_touch.Plan_Begin)
        {
            _touch.BeginPos = new Vector3((int)(_touch.BeginPos.x)+0.5f, 0, (int)(_touch.BeginPos.z)+0.5f);
            i2 pt = new i2((int)_touch.BeginPos.x, (int)_touch.BeginPos.z);
            CurCell = cells.Get(pt.x, pt.z);
        }
        DebugEx.DrawSqureXZ(_touch.BeginPos, 0.5f, Color.yellow);
        DebugEx.DrawSqureXZ(_touch.BeginPos, 0.45f, Color.black);
        
        if(pressed_Space_Create)
        {
            created = true;
            i2 pt = new i2((int)_touch.BeginPos.x, (int)_touch.BeginPos.z);
            objs.InavtiveObjs(pt.x, pt.z, pt.x, pt.z);
            loads.SetCellInfo(pt.x, pt.z, _type);
            hero.culling.Refresh();
        }
    }
    
    void onUpdateOnRectSelectMode()
    {
        if (_touch.Plan_Begin)
            _touch.BeginPos = new Vector3((int)(_touch.BeginPos.x)+0.5f, 0, (int)(_touch.BeginPos.z)+0.5f);
        if (_touch.Plan_BeginRight)
            _touch.BeginPos2 = new Vector3((int)(_touch.BeginPos2.x)+0.5f, 0, (int)(_touch.BeginPos2.z)+0.5f);
        i2 pt = new i2((int)_touch.BeginPos.x, (int)_touch.BeginPos.z);
        i2 pt2 = new i2((int)_touch.BeginPos2.x, (int)_touch.BeginPos2.z);
        for(int x=pt.x; x<=pt2.x; ++x)
            for(int z=pt.z; z<=pt2.z; ++z)
            {
                Vector3 pos = new Vector3(x + 0.5f, 0, z + 0.5f);
                DebugEx.DrawSqureXZ(pos, pos, 0.5f, Color.yellow);
                DebugEx.DrawSqureXZ(pos, pos, 0.45f, Color.black);
            }
        
        if(pressed_Space_Create)
        {
            created = true;
            objs.InavtiveObjs(pt.x, pt.z, pt2.x, pt2.z);
            for(int x=pt.x; x<=pt2.x; ++x)
                for(int z=pt.z; z<=pt2.z; ++z)
                    loads.SetCellInfo(x, z, _type);
            hero.culling.Refresh();
        }
    }
    
    void onUpdateOnCopyMode(int copyIdx)
    {
        if (_touch.Plan_Begin)
        {
            _touch.BeginPos = new Vector3((int)(_touch.BeginPos.x)+0.5f, 0, (int)(_touch.BeginPos.z)+0.5f);
            if(Input.GetKey(KeyCode.LeftControl))
                _touch.BeginPos2 = _touch.BeginPos + new Vector3(_gaps[copyIdx].x,0,_gaps[copyIdx].z);
        }
        if (_touch.Plan_BeginRight)
        {
            _touch.BeginPos2 = new Vector3((int)(_touch.BeginPos2.x)+0.5f, 0, (int)(_touch.BeginPos2.z)+0.5f);
            if(Input.GetKey(KeyCode.LeftControl))
                _touch.BeginPos = _touch.BeginPos2 - new Vector3(_gaps[copyIdx].x,0,_gaps[copyIdx].z);
        }
        DebugEx.DrawSqureXZ(_touch.BeginPos, _touch.BeginPos2, 0.5f, Color.yellow);
        DebugEx.DrawSqureXZ(_touch.BeginPos, _touch.BeginPos2, 0.45f, Color.black);
        
        DebugEx.DrawSqureXZ(_touch.BeginPos, 
        _touch.BeginPos + new Vector3(_gaps[copyIdx].x,0,_gaps[copyIdx].z), 0.45f, Color.green);
        

        if(pressed_Copy)
            copy(copyIdx);
        if(pressed_Space_Create)
        {
            created = true;
            paste(copyIdx);
        }
    }
    
    bool pressed_Space_Create{ get { return Input.GetKeyUp(KeyCode.Space); } }
    bool pressed_Copy{ get { return Input.GetKeyUp(KeyCode.C); } }
    bool pressed_Delete{ get { return Input.GetKeyUp(KeyCode.Delete); } }
}

static class jsK
{
    public const string Stage = "stg";
    public const string Info = "inf";
    public const string Cells = "ec";
    public static JSONObject ObjStage = new JSONObject();
    public static JSONObject ObjInfo = new JSONObject();
    public static JSONObject ObjCells = new JSONObject();

    public const string Proj = "prj";
    public const string AimGaps = "aimgaps";
    public const string AimDirs = "aimdirs";
    public static JSONObject ObjPrj = new JSONObject();
    public static JSONObject ObjAimGaps = new JSONObject();
    public static JSONObject ObjAimDirs = new JSONObject();
}