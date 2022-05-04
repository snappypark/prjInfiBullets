using UnityEngine;
using UnityEngineEx;

public class cube : qObj
{
    enum state:byte{ none, alive, hp0 } state _state;
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] TextMesh _lb;
    cell _cell;
    int _hp;
    delay _delayHit = new delay(0.12f);
    
    public void OnActive(cell cell)
    {
        _cell = cell;
        _renderer.transform.localScale = Vector3.one;
        setByCellType();
        _delayHit.End();
    }

    void Update()
    {
        switch(_state)
        {
            case state.alive:
            if(_delayHit.InTime())
                _renderer.transform.Scale(1 - _delayHit.Ratio10()*0.27f);

            break;
            case state.hp0:
            float dt0 = _renderer.transform.localScale.x*0.70f;
            _renderer.transform.Scale(dt0);
            if(dt0<0.01f)
            {
                _renderer.transform.localPosition = new Vector3(0,-2,0);
                _state = state.none;
            }
            break;
        }
    }

    const float _sizeHp0 = 1.57f;
    public void HitByDmg1()
    {
        if(_hp == 0)
            return;
            --_hp;
        if(_hp == 0) 
            HitByHp0();
        else
        {
            if(_cell.Type != cell.type.WZb)
            {
                _delayHit.Reset();
                _lb.text = _hp.ToString();
            }
            switch(_hp)
            {
                case objs.hp10: setMat(cubes.mats_.W0w); break;
                case objs.hp20: setMat(cubes.mats_.W1y); break;
                case objs.hp30: setMat(cubes.mats_.W2o); break;
                case objs.hp40: setMat(cubes.mats_.W3g1); break;
                case objs.hp50: setMat(cubes.mats_.W4g2); break;
                case objs.hp60: setMat(cubes.mats_.W5s); break;
                case objs.hp70: setMat(cubes.mats_.W6b); break;
                case objs.hp80: setMat(cubes.mats_.W7pu); break;
                case objs.hp90: setMat(cubes.mats_.W8pi); break;
            }
        }
    }
    
    public void HitByHp0()
    {
        AppAudio.Play(AppAudio.eSoundType.hit);
        _renderer.transform.localScale = new Vector3(_sizeHp0, _sizeHp0, _sizeHp0);
        _state = state.hp0;
        _lb.text = string.Empty;
        cells.changed = true;
        if(objs.foods.CheckAndSpawnFood(_cell))
            _cell.SetPath(cell.type.Food);
        else
            _cell.SetPath(cell.type.W0_Floor);
    }


//♩
    void setByCellType()
    {
        switch(_cell.Type) {
            
            case cell.type.W0w: _hp = objs.hp10; setMat(cubes.mats_.W0w); _lb.text = _hp.ToString();break;
            case cell.type.W1y: _hp = objs.hp20; setMat(cubes.mats_.W1y); _lb.text = _hp.ToString();break;
            case cell.type.W2o: _hp = objs.hp30; setMat(cubes.mats_.W2o); _lb.text = _hp.ToString();break;
            case cell.type.W3g1:_hp = objs.hp40; setMat(cubes.mats_.W3g1); _lb.text = _hp.ToString();break;
            case cell.type.W4g2:_hp = objs.hp50; setMat(cubes.mats_.W4g2); _lb.text = _hp.ToString();break;
            case cell.type.W5s: _hp = objs.hp60; setMat(cubes.mats_.W5s); _lb.text = _hp.ToString();break;
            case cell.type.W6b: _hp = objs.hp70; setMat(cubes.mats_.W6b); _lb.text = _hp.ToString();break;
            case cell.type.W7pu:_hp = objs.hp80; setMat(cubes.mats_.W7pu); _lb.text = _hp.ToString();break;
            case cell.type.W8pi:_hp = objs.hp90; setMat(cubes.mats_.W8pi); _lb.text = _hp.ToString();break;
            case cell.type.W9r: _hp = objs.hpEE; setMat(cubes.mats_.W9r); _lb.text = _hp.ToString();break;
            case cell.type.WZb: _hp = objs.hpZZ; setMat(cubes.mats_.WZb); _lb.text = string.Empty;break;
                
            case cell.type.Finish:   _lb.text = string.Empty; _state = state.none; setMat(cubes.mats_.Finish);
            _renderer.transform.localPosition = new Vector3(0,0.5f,0);
            return;
            case cell.type.W0_Floor: _lb.text = string.Empty; _state = state.none; 
            _renderer.transform.localPosition = new Vector3(0,-2,0);
            return;
        }
        _renderer.transform.localPosition = new Vector3(0,0.5f,0);
        
        _state = state.alive;
    }


    void setMat(byte idxMat)
    {
        _renderer.material = objs.cubes.mats[idxMat];
    }
}
