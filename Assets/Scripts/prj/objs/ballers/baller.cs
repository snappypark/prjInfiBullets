using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineEx;

public partial class baller : qObj
{
    [SerializeField] MeshRenderer[] _renders;
    [SerializeField] ParticleSystem _ps;
    [SerializeField] LineRenderer _line;
    [SerializeField] TextMesh _lb;
    cell _cell;
    int _hp;
    float _spd;
    delay _dlySpawn = new delay(1.2f);
    
    public void OnActive(cell cell, bool hasLine, bool isHead)
    {
        _renders[0].enabled = isHead;
        _cell = cell;
        setLine(hasLine, isHead);
        setByCellType();
    }
    
    public void HitByDmg1()
    {
        if(_hp == 0)
            return;
        --_hp;
        if(_hp == 0)
            HitAsHp0();
        else
        {
            if(_cell.Type != cell.type.BrZb)
                _lb.text = _hp.ToString();
            switch(_hp)
            {
                case objs.hp10: setMat(balls.mats_.B0w); _cell.Type = cell.type.Br0w; break;
                case objs.hp20: setMat(balls.mats_.B1y); _cell.Type = cell.type.Br1y; break;
                case objs.hp30: setMat(balls.mats_.B2o); _cell.Type = cell.type.Br2o; break;
                case objs.hp40: setMat(balls.mats_.B3g1); _cell.Type = cell.type.Br3g1; break;
                case objs.hp50: setMat(balls.mats_.B4g2); _cell.Type = cell.type.Br4g2; break;
                case objs.hp60: setMat(balls.mats_.B5s); _cell.Type = cell.type.Br5s; break;
                case objs.hp70: setMat(balls.mats_.B6b); _cell.Type = cell.type.Br6b; break;
                case objs.hp80: setMat(balls.mats_.B7pu); _cell.Type = cell.type.Br7pu; break;
                case objs.hp90: setMat(balls.mats_.B8pi); _cell.Type = cell.type.Br8pi; break;
            }
        }
    }

    public void HitAsHp0()
    {
        _hp = 0;
        _renders[0].enabled = false;
        _lb.text = string.Empty;

        objs.ballers.Inactive(_cell);
        _cell.SetPath();
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void Spawn()
    {
        if(_dlySpawn.InTime())
            return;
        if(hero.pt.z >= _cell.pt.z || _hp <= 0)
            return;
        _dlySpawn.Reset();
        
        switch(_data.Type)
        {
            case data.type.toHero:
            if(balls.FindPath(_cell, ref balls.pathPts))
            {
                if(balls.pathPts.Num>0)
                    objs.balls.ActiveBySpawn(_cell, ref balls.pathPts, data.type.toHero, _spd);
            }
            break;
            case data.type.way1:
            if(_data.pts.Num>0)
                objs.balls.ActiveBySpawn(_cell, ref _data.pts, data.type.way1, _spd);
            break;
        }
    }

//♩
    void setMat(byte idxMat) { _line.material = _renders[0].material = _renders[1].material = objs.balls.mats[idxMat];}

    const float _gapSpawn = 2.7f;//2.7f;
    const float _gap2 = 0.26f;
    const float _gap3 = 0.75f;
    void setByCellType()
    {
        switch (_cell.Type) {
            case cell.type.Br0w:  _spd = 1.2f; _dlySpawn.ResetRandGap(_gapSpawn);      _hp = objs.hp10; setMat(balls.mats_.B0w); _lb.text = _hp.ToString();break;
            case cell.type.Br1y:  _spd = 1.2f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2); _hp = objs.hp20; setMat(balls.mats_.B1y); _lb.text = _hp.ToString();break;
            case cell.type.Br2o:  _spd = 1.1f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*2)); _hp = objs.hp30; setMat(balls.mats_.B2o); _lb.text = _hp.ToString();break;
            case cell.type.Br3g1: _spd = 1.0f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*3)); _hp = objs.hp40; setMat(balls.mats_.B3g1); _lb.text = _hp.ToString();break;
            case cell.type.Br4g2: _spd = 0.9f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*4)); _hp = objs.hp50; setMat(balls.mats_.B4g2); _lb.text = _hp.ToString();break;
            case cell.type.Br5s:  _spd = 0.8f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*5)); _hp = objs.hp60; setMat(balls.mats_.B5s); _lb.text = _hp.ToString();break;
            case cell.type.Br6b:  _spd = 0.7f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*6)); _hp = objs.hp70; setMat(balls.mats_.B6b); _lb.text = _hp.ToString();break;
            case cell.type.Br7pu: _spd = 0.6f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*7)); _hp = objs.hp80; setMat(balls.mats_.B7pu); _lb.text = _hp.ToString();break;
            case cell.type.Br8pi: _spd = 0.5f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*8)); _hp = objs.hp90; setMat(balls.mats_.B8pi); _lb.text = _hp.ToString();break;
            case cell.type.Br9r:  _spd = 0.4f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*9)); _hp = objs.hpEE; setMat(balls.mats_.B9r); _lb.text = _hp.ToString();break;
            case cell.type.BrZb:  _spd = 0.5f; _dlySpawn.ResetRandGap(_gapSpawn+_gap2*(_gap3*10)); _hp = objs.hpZZ; setMat(balls.mats_.BZb); _lb.text = string.Empty;break; 
            
            case cell.type.Br0w_:  _hp = 16; setMat(balls.mats_.B0w); _lb.text = string.Empty;break;
            case cell.type.Br1y_:  _hp = 16; setMat(balls.mats_.B1y); _lb.text = string.Empty;break;
            case cell.type.Br2o_:  _hp = 16; setMat(balls.mats_.B2o); _lb.text = string.Empty;break;
            case cell.type.Br3g1_: _hp = 16; setMat(balls.mats_.B3g1); _lb.text = string.Empty;break;
            case cell.type.Br4g2_: _hp = 16; setMat(balls.mats_.B4g2); _lb.text = string.Empty;break;
            case cell.type.Br5s_:  _hp = 16; setMat(balls.mats_.B5s); _lb.text = string.Empty;break;
            case cell.type.Br6b_:  _hp = 16; setMat(balls.mats_.B6b); _lb.text = string.Empty;break;
            case cell.type.Br7pu_: _hp = 16; setMat(balls.mats_.B7pu); _lb.text = string.Empty;break;
            case cell.type.Br8pi_: _hp = 16; setMat(balls.mats_.B8pi); _lb.text = string.Empty;break;
            case cell.type.Br9r_:  _hp = 16; setMat(balls.mats_.B9r); _lb.text = string.Empty;break;
            case cell.type.BrZb_:  _hp = 16; setMat(balls.mats_.BZb); _lb.text = string.Empty;break;
            }
    }
    

    void setLine(bool hasLine, bool isHead)
    {
        if(hasLine)
        {
            _line.positionCount = _data.pts.Num+2;
            _line.SetPosition(0, Vector3.zero);
            if(isHead)
            {
                for(int i=0; i<_data.pts.Num; ++i)
                {
                    f2 ct = _data.pts[i];
                    _line.SetPosition(i+1, new Vector3(ct.x - _cell.ct.x, 0, ct.z - _cell.ct.z));
                }
                _line.SetPosition(_data.pts.Num+1, new Vector3(_data.end.ct.x - _cell.ct.x, 0, _data.end.ct.z - _cell.ct.z));
            }
            else
            {
                for(int i=_data.pts.Num-1; i>=0; --i)
                {
                    f2 ct = _data.pts[i];
                    _line.SetPosition(i+1, new Vector3(ct.x - _cell.ct.x, 0, ct.z - _cell.ct.z));
                }
                _line.SetPosition(_data.pts.Num+1, new Vector3(_data.begin.ct.x - _cell.ct.x, 0, _data.begin.ct.z - _cell.ct.z));
            }
        }
        else
            _line.positionCount = 0;
    }
    

#if UNITY_EDITOR
    void Update()
    {
        if(flowEdit.IsOn && _cell.IsBallr() && _data.pts.Num > 0 && _data.end == null)
        {
            f2 p1 = _cell.ct;
            f2 p2 = _data.pts[0];
            Debug.DrawLine( new Vector3(p1.x, 0.1f, p1.z), new Vector3(p2.x, 0.1f, p2.z));
            for(int i=1; i<_data.pts.Num; ++i)
            {
                p1 = _data.pts[i - 1];
                p2 = _data.pts[i];
                Debug.DrawLine( new Vector3(p1.x, 0.1f, p1.z), new Vector3(p2.x, 0.1f, p2.z));
            }
        }
    }
#else
#endif
}
