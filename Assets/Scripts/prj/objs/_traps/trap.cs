using UnityEngine;
using UnityEngineEx;

public class trap : qObj
{
    enum state:byte{ none, up, down, upOnce, downOnce} state _state;

    [SerializeField] Transform _pike;
    cell _cell;

    delay _delay = new delay(0.61f);
    float _dt;
    public void OnActivePike(cell cell)
    {
        _cell = cell;
        _state = state.up;
        _pike.localPosition = Vector3.zero;
        _delay.Reset(0.5f);
    }

    public void OnActiveBigPike(cell cell)
    {
        _cell = cell;
        _state = state.upOnce;
        _pike.localPosition = new Vector3(0, moveaixY0, 0);
        _delay.Reset(1.4f);
    }

    const float moveGap = 0.33f;
    const float moveaixY0 = -3.2f;
    const float moveGap2 = 2.2f;
    void FixedUpdate()
    {
#if UNITY_EDITOR
    if(flowEdit.IsOn && _cell.Type == cell.type.TrapSmall)
        return;
#else
#endif
        switch(_state)
        {
            case state.up:
            _pike.localPosition = new Vector3(0,-0.4f+_delay.Ratio01()*moveGap,0);
            if(_delay.IsEnd())
            {
                _delay.Reset();
                _state = state.down;
            }
            break;
            
            case state.down:
            _pike.localPosition = new Vector3(0,-0.4f+_delay.Ratio10()*moveGap,0);
            if(_delay.IsEnd())
            {
                _delay.Reset();
                _state = state.up;
            }
            break;
            
            case state.upOnce:
            _dt = _delay.Ratio01();
            _pike.localPosition = new Vector3(0,moveaixY0+_dt*moveGap2,0);
            _pike.localEulerAngles = new Vector3(0, _pike.localEulerAngles.y+5, 0); 
            if(_delay.IsEnd())
            {
                _delay.Reset();
                _state = state.downOnce;
            }
            checkColliByHero();
            break;
            
            case state.downOnce:
            _dt = _delay.Ratio10();
            _pike.localPosition = new Vector3(0,moveaixY0+_dt*moveGap2,0);
            _pike.localEulerAngles = new Vector3(0, _pike.localEulerAngles.y+5, 0); 
            if(_delay.IsEnd())
            {
                objs.traps.Unactive(this);
                return; 
            }
            checkColliByHero();
            break;
        }
    }

    void checkColliByHero()
    {
        float dist = 0.33f + _dt*0.498f;
        if(hero.CollideByRadius(_cell.ct, dist*dist) )
        {
            hero.LvDown();
            objs.traps.Unactive(this);
        }
    }
}
