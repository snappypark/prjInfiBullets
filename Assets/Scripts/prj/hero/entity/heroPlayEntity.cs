using UnityEngine;

public partial class heroPlayEntity : absHeroEntity
{
    const float _spdX = 3.3f;
    void FixedUpdate()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.A))
            hero.MoveLeft(_spdX);
        else if(Input.GetKey(KeyCode.D))
            hero.MoveRight(_spdX);
#else
#endif

        hero.UpdateTran();
        hero.culling.OnUpdate(hero.pt.z);
        hero.MoveZ(hero.stat.zSpeed);

        if(uis.form.IsOn_BtnLeft)
            hero.MoveLeft(_spdX);
        else if(uis.form.IsOn_BtnRight)
            hero.MoveRight(_spdX);
            
        hero.Shoot();
            
        cells.OnUpdate_Jsp();

        cell c11 = cells.Get(hero.pt11.x, hero.pt11.z);
        for (int i = 0; i < Idx.MaxNum2x2; ++i)
        {
            cell c = c11.C2X2(i);
            switch(c.Type) {
                case cell.type.W0w: case cell.type.W1y: case cell.type.W2o: case cell.type.W3g1: case cell.type.W4g2:
                case cell.type.W5s: case cell.type.W6b: case cell.type.W7pu: case cell.type.W8pi: case cell.type.W9r: 
                case cell.type.WZb:
                if(hero.CollideByCube(c, c11))
                {
                    objs.cubes[c.cdx].HitByHp0();
                    hero.LvDown();
                }
                
                break;

                case cell.type.Br0w: case cell.type.Br1y: case cell.type.Br2o: case cell.type.Br3g1: case cell.type.Br4g2:
                case cell.type.Br5s: case cell.type.Br6b: case cell.type.Br7pu: case cell.type.Br8pi: case cell.type.Br9r: 
                case cell.type.BrZb:
                if(c.HasObj && hero.CollideByRadius(c11.pt, sqr.f0_48) && hero.dlyColli.IsEnd())
                {
                    objs.ballers[c.cdx].HitAsHp0();
                    hero.dlyColli.Reset();
                }
                break;

                case cell.type.TrapSmall:
                if(c.HasObj && hero.CollideByTrapSmall(c, c11) && hero.dlyColli.IsEnd())
                {
                    hero.LvDown();
                    hero.dlyColli.Reset();
                }else
                {
                    hero.CollideByBall(c);

                }
                break;
                case cell.type.Food:
                if(c.HasObj && hero.CollideByRadius(c.ct, sqr.f0_74))
                {
                    hero.LvUp();
                    objs.foods.Inactive(c);
                }
                break;

                case cell.type.Finish:
                hero.Init(hero.Type.Goal);
                return;

                case cell.type.PathEnd:
                hero.stat.zSpeed = 2.0f;
                
                hero.stat.bulletDly.Reset(70);
                break;

                default:
                hero.CollideByBall(c);
                break;
            }
        }
        
    }


}
