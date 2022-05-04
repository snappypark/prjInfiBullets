using UnityEngine;
using UnityEngineEx;

public class traper : qObj
{
    enum state:byte{ none, straight, left, right, leftdown, rightdown, } state _state;

    delay _delayMove = new delay(0.13f);
    cell _cur;

    public void OnActiveStraight(cell cell) { _cur = cell; _state = state.straight; onActive(); }
    public void OnActiveLeft(cell cell) {  _cur = cell; _state = _cur.pt.x == 1  ? state.rightdown : state.left; onActive(); }
    public void OnActiveRight(cell cell) { _cur = cell; _state = _cur.pt.x == 11 ? state.leftdown :state.right; onActive(); }

    void onActive()
    {
        _delayMove.Reset(0.3f + 0.1f*RandEx.GetN(3));
    }

    void Update()
    {
#if UNITY_EDITOR
        if(flowEdit.IsOn)
        {
            DebugEx.DrawCircleXZ(_cur.ct.x, 0.1f, _cur.ct.z, 0.5f, Color.red);
            DebugEx.DrawCircleXZ(_cur.ct.x, 0.1f, _cur.ct.z, 0.45f, Color.cyan);
            DebugEx.DrawCircleXZ(_cur.ct.x, 0.1f, _cur.ct.z, 0.4f, Color.blue);
            DebugEx.DrawCircleXZ(_cur.ct.x, 0.1f, _cur.ct.z, 0.3f, Color.green);
            if(_state == state.straight)
                DebugEx.DrawLineOnXZ(_cur.ct.x, _cur.ct.z-0.3f, _cur.ct.x, _cur.ct.z+0.3f, Color.red);
            return;
        }
#else
#endif
        if(_cur == null)
            return;

        if(_delayMove.InTime())
            return;
        _delayMove.Reset();
        objs.traps.ActiveBigPike(_cur);
        switch(_state)
        {
            case state.straight: _cur = _cur.South(); break;
            
            case state.left:
                _cur = _cur.West();
                _state = _cur.pt.x == 1 ? state.rightdown : state.leftdown;
            break;
            case state.right:
                _cur = _cur.East();
                _state = _cur.pt.x == 11 ? state.leftdown : _state = state.rightdown;
            break;
            
            case state.leftdown:
                _cur = _cur.South();
                _state = state.left;
            break;
            case state.rightdown:
                _cur = _cur.South();
                _state = state.right;
            break;
            
        }
        
        
        if(_cur.pt.z < hero.backZ)
            _cur = null;
    }
}
