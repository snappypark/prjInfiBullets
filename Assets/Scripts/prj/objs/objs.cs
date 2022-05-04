using System.Collections;
using UnityEngine;

public class objs : MonoBehaviour
{
    public static cubes cubes; 
    public static ballers ballers; 
    public static balls balls; 
    public static foods foods;
    public static trapers trapers;

    public static bullets bullets;
    public static traps traps;
    
    void Awake() { cubes = cubes.Inst; ballers = ballers.Inst; balls = balls.Inst; 
                   foods = foods.Inst; trapers = trapers.Inst; 
                    bullets = bullets.Inst; traps = traps.Inst;}

    public static IEnumerator Clear_() {
        bullets.UnactiveAll();
        yield return null;
        pulses.Clear();
        cells.Clear();
        yield return null; ballers.Clear(); yield return null; balls.UnactiveAll(); 
        yield return null; cubes.UnactiveAll(); yield return null; foods.UnactiveAll_Shuffle();
        yield return null; trapers.UnactiveAll(); yield return null; traps.UnactiveAll();   }

    public static void Clear() {
        pulses.Clear();
        cells.Clear(); 
        ballers.Clear(); balls.UnactiveAll(); 
        cubes.UnactiveAll(); foods.UnactiveAll(); 
        trapers.UnactiveAll();  traps.UnactiveAll(); }

    public static void InavtiveObjs(int x0, int z0, int x1, int z1)
    {
        for (int x = x0; x <= x1; ++x)
            for (int z = z0; z <= z1; ++z)
                InactiveObj(cells.Get(x,z));
    }
    
    public static void InactiveObj(cell c)
    {
        if(c.HasObj)
        {
            switch(c.Type)
            {
                case cell.type.Food: objs.foods.Inactive(c); break;
                case cell.type.TrapSmall: objs.traps.Inactive(c); break;

                case cell.type.Finish: case cell.type.W0_Floor:
                case cell.type.W0w: case cell.type.W1y: case cell.type.W2o: case cell.type.W3g1: case cell.type.W4g2:
                case cell.type.W5s: case cell.type.W6b: case cell.type.W7pu: case cell.type.W8pi: case cell.type.W9r: 
                case cell.type.WZb:
                objs.cubes.Inactive(c);
                break;

                case cell.type.Br0w: case cell.type.Br1y: case cell.type.Br2o: case cell.type.Br3g1: case cell.type.Br4g2:
                case cell.type.Br5s: case cell.type.Br6b: case cell.type.Br7pu: case cell.type.Br8pi: case cell.type.Br9r: 
                case cell.type.BrZb:
                case cell.type.Br0w_: case cell.type.Br1y_: case cell.type.Br2o_: case cell.type.Br3g1_: case cell.type.Br4g2_:
                case cell.type.Br5s_: case cell.type.Br6b_: case cell.type.Br7pu_: case cell.type.Br8pi_: case cell.type.Br9r_: 
                case cell.type.BrZb_:
                objs.ballers.Inactive(c);
                break;
                
                case cell.type.Traper: case cell.type.TraperI: case cell.type.TraperL: case cell.type.TraperR:
                objs.trapers.Inactive(c);
                break;
            }
        }
    }

    public static void ActiveObj(cell c)
    {
        if(c.HasObj)
            return;
        
        switch(c.Type)
        {
            case cell.type.Food: c.cdx = foods.Active(c).cdx; break;
            case cell.type.TrapSmall: c.cdx = traps.ActivePike(c).cdx; break;
            
            case cell.type.Finish:  case cell.type.W0_Floor:
            case cell.type.W0w: case cell.type.W1y: case cell.type.W2o: case cell.type.W3g1: case cell.type.W4g2:
            case cell.type.W5s: case cell.type.W6b: case cell.type.W7pu: case cell.type.W8pi: case cell.type.W9r: 
            case cell.type.WZb:
            c.cdx = cubes.Active(c).cdx;
            break;
            
            case cell.type.B0w: case cell.type.B1y: case cell.type.B2o: case cell.type.B3g1: case cell.type.B4g2:
            case cell.type.B5s: case cell.type.B6b: case cell.type.B7pu: case cell.type.B8pi: case cell.type.B9r: 
            case cell.type.BZb:
            c.cdx = balls.Active(c).cdx;
            break;
            
            case cell.type.Br0w: case cell.type.Br1y: case cell.type.Br2o: case cell.type.Br3g1: case cell.type.Br4g2:
            case cell.type.Br5s: case cell.type.Br6b: case cell.type.Br7pu: case cell.type.Br8pi: case cell.type.Br9r: 
            case cell.type.BrZb:
            case cell.type.Br0w_: case cell.type.Br1y_: case cell.type.Br2o_: case cell.type.Br3g1_: case cell.type.Br4g2_:
            case cell.type.Br5s_: case cell.type.Br6b_: case cell.type.Br7pu_: case cell.type.Br8pi_: case cell.type.Br9r_: 
            case cell.type.BrZb_:
            ballers.Active(c);
            break;

            case cell.type.Traper:  c.cdx = trapers.Active(c).cdx; break;
            case cell.type.TraperI: c.cdx = trapers.Active_Straight(c).cdx; break;
            case cell.type.TraperL: c.cdx = trapers.Active_Left(c).cdx; break;
            case cell.type.TraperR: c.cdx = trapers.Active_Right(c).cdx; break;
        }
    }


    public const int hp10 = 10;
    public const int hp20 = 20;
    public const int hp30 = 30;
    public const int hp40 = 40;
    public const int hp50 = 50;
    public const int hp60 = 60;
    public const int hp70 = 70;
    public const int hp80 = 80;
    public const int hp90 = 90;
    public const int hpEE = 100;
    public const int hpZZ = 9999;
}
