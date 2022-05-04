using System.Runtime.CompilerServices;
using UnityEngine;

public partial class bullet : qObj
{
    state _pre;state _state; enum state:byte { none, flying, hitCube, hitCubePoint, hitBall};
    [SerializeField] SpriteRenderer _render;
    [SerializeField] TrailRenderer _trailRender;
    const float _duration = 4.0f, _durationOv = 1/_duration, _lenghTail = 0.2f;
    float _spd, _endTime;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnActive(float spd, Color color)
    {
        _trailRender.time = _lenghTail;
        _spd = spd;
        _pre = state.flying;
        _render.color = color;
        _trailRender.startColor = new Color(color.r, color.g, color.b, 0.5f);
        _trailRender.endColor = new Color(color.r, color.g, color.b, 0.1f);
        _endTime = Time.time + _duration;
    }

    void Update()
    {
        if(transform.localPosition.x < bd.left_25 && transform.forward.x < 0)
            transform.forward = new Vector3(-transform.forward.x, 0, transform.forward.z);
        else if(transform.localPosition.x > bd.right_25 && transform.forward.x > 0)
            transform.forward = new Vector3(-transform.forward.x, 0, transform.forward.z);
        else if(transform.localPosition.z < hero.pt.z - 4 ||
                transform.localPosition.z > hero.pt.z + 29)
            objs.bullets.Unactive(0, this);
        else if(_endTime > Time.time)
            _trailRender.time = (_endTime - Time.time)* _durationOv*_lenghTail;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void FixedUpdate()
    {
        float spd =  _spd * Time.fixedDeltaTime;
        transform.localPosition = new Vector3(
            transform.localPosition.x + transform.forward.x * spd, transform.localPosition.y,
            transform.localPosition.z + transform.forward.z * spd);

        i2 pt11 = cell.Pt11(transform.localPosition);
        if (cells.IsOut(pt11.x, pt11.z))
            return;

        _state = state.flying;
        cell c11 = cells.Get(pt11.x, pt11.z);
        for (int i = 0; i < Idx.MaxNum2x2; ++i)
        {
            i2 pt = c11.Pt2X2(i);
            if (cells.IsOut(pt.x, pt.z))
                continue;

            cell c = cells.Get(pt.x, pt.z);
            switch(c.Type)
            {
                case cell.type.W0w: case cell.type.W1y: case cell.type.W2o: case cell.type.W3g1: case cell.type.W4g2:
                case cell.type.W5s: case cell.type.W6b: case cell.type.W7pu: case cell.type.W8pi: case cell.type.W9r: 
                case cell.type.WZb:
                if(c.HasObj && collideByCubeSide(c))
                {
                    _state = state.hitCube;
                    objs.cubes[c.cdx].HitByDmg1();
                }
                else if(_state != state.hitCube)
                    _state = state.hitCubePoint;
                break;
                
                case cell.type.Br0w: case cell.type.Br1y: case cell.type.Br2o: case cell.type.Br3g1: case cell.type.Br4g2:
                case cell.type.Br5s: case cell.type.Br6b: case cell.type.Br7pu: case cell.type.Br8pi: case cell.type.Br9r: 
                case cell.type.BrZb:
                if(c.HasObj && collideByBaller(c.ct))
                {
                    AppAudio.Play(AppAudio.eSoundType.hit);
                    _state = state.hitBall;
                    objs.ballers[c.cdx].HitByDmg1();
                }
                break;

                case cell.type.Finish:
                if(transform.localPosition.z > c.ct.z - 0.5f)
                {
                    ofts.particles.Create(effParticles.hitWall, transform.localPosition);
                    objs.bullets.Unactive(0, this);
                }
                return;

                default:
                collideByBall(c);
                break;
            }
        }
        if(_state == state.hitCubePoint)
        {
            collideByCubePoint(c11.ct);
            if(_endTime < Time.time)
            {
                ofts.particles.Create(effParticles.hitWall, transform.localPosition);
                objs.bullets.Unactive(0, this);
            }
        }
    }
    
}
