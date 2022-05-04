using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Collections.GenericEx;
using UnityEngine;
using UnityEngineEx;

public partial class ball : qObj
{
    enum state:byte{ none, find, spawnToHero, toHero, downway, spawnWay1, way1, hp0 } state _state;
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] TextMesh _lb;
    cell _cell;
    int _hp;

    arr<f2> _pts = new arr<f2>(16);
    i2 _pt;
    f2 _dir;

    delay _delayHit = new delay(0.13f);
    float _speed = 1;

    void OnDisable()
    {
        if(null != _cell)
            _cell.cdxs.Deq(cdx);
    }

    public void OnActive(cell cell)
    {
        _speed = 1;
        onActive(cell);
        _state = state.find;
    }

    public void OnActiveBySpawn(cell cell, ref arr<f2> pts, baller.data.type datatype, float spd)
    {
        _speed = spd;
        onActive(cell);
        switch(datatype) {
            case baller.data.type.toHero: _state = state.spawnToHero; _pts.Copy(ref pts);break;
            case baller.data.type.way1:   _state = state.spawnWay1; _pts.CopyOfReverse(ref pts); break; }
        
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void onActive(cell cell)
    {
        transform.localScale = Vector3.zero;
        _cell = cell;
        setByCellType();
        _delayHit.End();
    }

    void Update()
    {
#if UNITY_EDITOR
    if(flowEdit.IsOn) return;
#else
#endif
        if(_delayHit.InTime())
            transform.Scale(1 + _delayHit.Ratio10()*0.32f);

        switch(_state)
        {
            case state.find:
                if(balls.FindPath(_cell, ref _pts))
                    setState_toHero();
            break;
            
            case state.spawnToHero:
                transform.Scale(transform.localScale.x + 0.1f);
                if(transform.localScale.x>0.95f)
                    setState(state.toHero);
            break;

            case state.spawnWay1:
                transform.Scale(transform.localScale.x + 0.1f);
                if(transform.localScale.x>0.95f)
                    setState(state.way1);
            break;

            


            case state.way1:
                f2 to = _pts.Peek;
                float dx = to.x - transform.localPosition.x;
                float dz = to.z - transform.localPosition.z;

                if(dx*dx + dz*dz < 0.003f)
                {
                    --_pts.Num;
                    _dir = (_pts.Peek - to).normalized;
                    transform.localPosition = new Vector3(to.x, 0, to.z);
                    
                    if(_pts.Num == 0)
                        _state = state.hp0;
                }
                else
                    transform.MoveXZ(_dir.x, _dir.z, _speed* Time.smoothDeltaTime);
                
                setCellOfCdx();
            break;
            
            case state.toHero:
                if (_pts.Num == 0)
                    return;
                f2 target = _pts.Peek;
                dx = target.x - transform.localPosition.x;
                dz = target.z - transform.localPosition.z;

                if(dx*dx + dz*dz < 0.003f )
                {
                    --_pts.Num;
                    if(_pts.Num>0)
                        _dir = (_pts.Peek - target).normalized;
                    transform.localPosition = new Vector3(target.x, 0, target.z);
                        
                    if(_cell.IsStraight)
                        _state = state.downway;
                }
                else
                    transform.MoveXZ(_dir.x, _dir.z, _speed* Time.smoothDeltaTime);
                
                setCellOfCdx();

                
            break;
            
            case state.downway:
        
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z - _speed * Time.smoothDeltaTime);
            
                setCellOfCdx();
                if(_cell.pt.z < hero.backZ)
                {
                    _cell.cdxs.Deq(cdx);
                    objs.balls.Inactive(cdx);
                }
            break;

            case state.hp0:
            transform.Scale(transform.localScale.x*0.70f);
            if(transform.localScale.x<0.01f)
            {
                transform.localScale = Vector3.zero;
                objs.balls.Inactive(cdx);
            }
            break;
        }
        
        
    }

    public void HitByBullet(cell c)
    {
        --_hp;
        if(_hp == 0)
        {
            _state = state.hp0;
            _lb.text = string.Empty;
        }
        else
        {
            if(_cell.Type != cell.type.BrZb)
            {
                _delayHit.Reset();
                _lb.text = _hp.ToString();
            }
            c.cdxs.Enqueue(cdx);
            switch(_hp)
            {
                case objs.hp10: setMat(balls.mats_.B0w); break;
                case objs.hp20: setMat(balls.mats_.B1y); break;
                case objs.hp30: setMat(balls.mats_.B2o); break;
                case objs.hp40: setMat(balls.mats_.B3g1); break;
                case objs.hp50: setMat(balls.mats_.B4g2); break;
                case objs.hp60: setMat(balls.mats_.B5s); break;
                case objs.hp70: setMat(balls.mats_.B6b); break;
                case objs.hp80: setMat(balls.mats_.B7pu); break;
                case objs.hp90: setMat(balls.mats_.B8pi); break;
            }
        }
    }
    

//♩
    void setByCellType()
    {
        switch (_cell.Type) {
            case cell.type.B0w:case cell.type.Br0w:     _hp = objs.hp10; setMat(balls.mats_.B0w); _lb.text = _hp.ToString(); break;
            case cell.type.B1y:case cell.type.Br1y:     _hp = objs.hp20; setMat(balls.mats_.B1y); _lb.text = _hp.ToString(); break;
            case cell.type.B2o:case cell.type.Br2o:     _hp = objs.hp30; setMat(balls.mats_.B2o); _lb.text = _hp.ToString(); break;
            case cell.type.B3g1:case cell.type.Br3g1:   _hp = objs.hp40; setMat(balls.mats_.B3g1); _lb.text = _hp.ToString(); break;
            case cell.type.B4g2:case cell.type.Br4g2:   _hp = objs.hp50; setMat(balls.mats_.B4g2); _lb.text = _hp.ToString(); break;
            case cell.type.B5s:case cell.type.Br5s:     _hp = objs.hp60; setMat(balls.mats_.B5s); _lb.text = _hp.ToString(); break;
            case cell.type.B6b:case cell.type.Br6b:     _hp = objs.hp70; setMat(balls.mats_.B6b); _lb.text = _hp.ToString(); break;
            case cell.type.B7pu:case cell.type.Br7pu:   _hp = objs.hp80; setMat(balls.mats_.B7pu); _lb.text = _hp.ToString(); break;
            case cell.type.B8pi:case cell.type.Br8pi:   _hp = objs.hp90; setMat(balls.mats_.B8pi); _lb.text = _hp.ToString(); break;
            case cell.type.B9r:case cell.type.Br9r:     _hp = objs.hpEE; setMat(balls.mats_.B9r); _lb.text = _hp.ToString(); break;
            case cell.type.BZb:case cell.type.BrZb:     _hp = objs.hpZZ; setMat(balls.mats_.BZb); _lb.text = string.Empty; break; }
        
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void setMat(byte idxMat){   _renderer.material = objs.balls.mats[idxMat];  }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void setState(state state)
    {
        transform.localScale = Vector3.one;
        _state = state;
        _cell.cdxs.Enque_NoDupli(cdx);
        _dir = (_pts.Peek - _cell.ct).normalized;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void setState_toHero()
    {
        transform.localScale = Vector3.one;
        _state = state.toHero;
        _cell.cdxs.Enque_NoDupli(cdx);
        _dir = (_pts.Peek - _cell.ct).normalized;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void setCellOfCdx()
    {
        cell c = cells.SwitchCell(_cell, cdx, (int)transform.localPosition.x, (int)transform.localPosition.z);
        if(c != null)
            _cell = c;
    }


/**/

    /*
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Alignments(unit u_, Vector3 pos, Vector3 dir, out Vector3 newDir)
    {
        bool result = false;
        newDir = Vector3.zero;
        cel1l c11 = core.zells[cel1l.Pt11(pos)];
        for (int i = 0; i < nj.idx.MaxNum2x2; ++i)
        {
            cel1l c = c11.C2X2(i);
            int num = c.units.Count;
            for (int r = 0; r < num; ++r)
            {
                unit unit = c.RollUnit;
                if (unit.checkHp0_SameCdx(u_.cdx) )
                    continue;

                f2 vTo = f2.VecXZ(unit.tran.localPosition, pos);
                if (vTo.DotXZ(dir) < 0)
                    continue;
                float sv = vTo.SqrMagnitude();
                if (sv < dist.sqr0_64)
                {
                    float srv = (1.2f - sv);// *0.5f;
                    //float srv = (Dist.sqr0_7 - sv);
                    f2 right = vTo.CrossProduct();

                    if (right.DotXZ(dir) > 0)
                        newDir += new Vector3(right.x - vTo.x, 0, right.z - vTo.z) * srv;
                    else
                        newDir -= new Vector3(right.x + vTo.x, 0, right.z + vTo.z) * srv;

                    result = true;
                }
            }
        }
        return result;
    }
    */
}
