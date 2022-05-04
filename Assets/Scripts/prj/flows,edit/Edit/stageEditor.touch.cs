using UnityEngine;
using UnityEngine.EventSystems;

public partial class stageEditor
{
    const int _numHis = 3;
    public cell.type[,,] history = new cell.type[_numHis, 11, 32];
    public i2[] _gaps = new i2[3];
    void initHistorys()
    {
        for(int i=0; i<_numHis; ++i)
            _gaps[i] = new i2();
        for(int i=0; i<3; ++i)
            for(int x=0; x<11; ++x)
                for(int z=0; z<32; ++z)
                {
                    history[i,x,z] = new cell.type();
                    history[i,x,z] = cell.type.Path;
                }
    }

    void copy(int idx)
    {
        setPts();
        _gaps[idx] = new i2(_pt2.x - _pt1.x, _pt2.z - _pt1.z);
        for(int x=_pt1.x; x<=_pt2.x; ++x)
            for(int z=_pt1.z; z<=_pt2.z; ++z)
            {
                if(x<1 || x > 11 )
                    continue;
                i2 gap = new i2(x - _pt1.x, z - _pt1.z);
                cell c = cells.Get(x, z);
                history[idx, gap.x, gap.z] = c.IsWall() ? c.Type : cell.type.Path;
            }
    }
    void paste(int idx)
    {
        setPts();
        _pt2 = new i2(_pt1.x + _gaps[idx].x, _pt1.z + _gaps[idx].z);
        Debug.Log(_pt1);
        for(int x=_pt1.x; x<=_pt2.x; ++x)
            for(int z=_pt1.z; z<=_pt2.z; ++z)
            {
                i2 gap = new i2(x - _pt1.x, z - _pt1.z);
                if(x<1 || x > 11 || gap.x > _gaps[idx].x || gap.z > _gaps[idx].z)
                    continue;
                cell c = cells.Get(x, z);
                if(c.Type == cell.type.Path)
                    loads.SetCellInfo(x, z, history[idx, gap.x, gap.z]);
            }
        hero.culling.Refresh();
    }

    /// cio
    
    TouchInfo _touch = new TouchInfo();
    i2 _pt1, _pt2;
    void setPts()
    {
        _pt1 = new i2((int)(_touch.BeginPos.x<0?_touch.BeginPos.x-0.55f:_touch.BeginPos.x), (int)_touch.BeginPos.z);
        _pt2 = new i2((int)_touch.BeginPos2.x, (int)_touch.BeginPos2.z);
    }
    
    void _drawHoverCircle()
    {
        if (_touch.Is_PlanHovering)
        {
            
        }
    }

    class TouchInfo
    {
        public bool IsActing = false;
        public Vector3 HoverPos;
        public Vector3 DragGap { get { return EndPos - BeginPos; } }
        public Vector3 BeginPos, EndPos;
        public Vector3 BeginPos2, EndPos2;
        
        public bool Plan_Begin { get { return TouchPlan_Begin(ref BeginPos) && !EventSystem.current.IsPointerOverGameObject(); } }
        public bool Plan_Pressed { get { return TouchPlan_Pressed(ref EndPos); } }
        public bool Plan_End { get { return TouchPlan_End(ref EndPos); } }
        public bool Plan_BeginRight { get { return TouchPlan_Begin(ref BeginPos2, MOUSE_RIGHT) && !EventSystem.current.IsPointerOverGameObject(); } }
        public bool Plan_PressedRight { get { return TouchPlan_Pressed(ref EndPos2, MOUSE_RIGHT); } }
        public bool Plan_EndRight { get { return TouchPlan_End(ref EndPos2, MOUSE_RIGHT); } }
        public bool Is_PlanHovering { get { return PickingPlan(out HoverPos); } }
    }
    
    #region touch plan
    const int MOUSE_LEFT = 0;
    const int MOUSE_RIGHT = 1;

    public static bool TouchPlan_Begin(ref Vector3 touchedPos, int mouseLeftOrRight = MOUSE_LEFT)
    {
        if (Input.GetMouseButtonDown(mouseLeftOrRight))
            if (PickingPlan(out touchedPos))
                return true;
        return false;
    }

    public static bool TouchPlan_Pressed(ref Vector3 touchedPos, int mouseLeftOrRight = MOUSE_LEFT)
    {
        if (Input.GetMouseButton(mouseLeftOrRight))
            if (PickingPlan(out touchedPos))
                return true;
        return false;
    }

    public static bool TouchPlan_End(ref Vector3 touchedPos, int mouseLeftOrRight = MOUSE_LEFT)
    {
        if (Input.GetMouseButtonUp(mouseLeftOrRight))
            if (PickingPlan(out touchedPos))
                return true;
        return false;
    }

    public static bool PickingPlan(out Vector3 touchedPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int TouchPlanMask = 1 << LayerMask.NameToLayer("TouchPlan");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, TouchPlanMask))
        {
            touchedPos = new Vector3(hit.point.x, 0, hit.point.z);
            return true;
        }
        touchedPos = Vector3.positiveInfinity;
        return false;
    }
    #endregion
}
