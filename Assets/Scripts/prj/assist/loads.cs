using System.Collections;
using UnityEngine;
using UnityEngineExJSON;

public static partial class loads
{
    public static IEnumerator StageFromJson_(int stageIdx)
    {
        area.ground.SetColor(stageIdx);
        area.InitSafeZ();
        JSONObject jsStage = new JSONObject(jsons.GetStageJson(stageIdx))[jsK.Stage];
        jsK.ObjInfo = jsStage[jsK.Info];
        jsK.ObjCells = jsStage[jsK.Cells];

        yield return objs.Clear_();
        for (int i = 0; i < jsK.ObjCells.Count; ++i)
        {
            if(i % 111 == 11)
                yield return null;
            ParseCellInfo(new jsArr(jsK.ObjCells[i]));
        }
        yield return null;
        area.Init(stageIdx);
        objs.foods.InitAppearOption(stageIdx);
        hero.ResetStat(stageIdx);
        yield return cells.InitJsp_();
        cells.InitStraights();
        hero.Init(hero.Type.Begin);
        pulses.Init();

        AppAudio.Inst.PlayMusic(AppAudio.eMusicType.ingame);
    }
    
    public static void ParseCellInfo(jsArr arJs)
    {
        cell c = cells.Get(arJs.Int, arJs.Int);
        cell.type type = (cell.type)arJs.Int;
        switch(type)
        {
            case cell.type.Finish:
            c.SetPath(type);
            endfield.z = c.pt.z;
            break;

            case cell.type.W0w: case cell.type.W1y: case cell.type.W2o: case cell.type.W3g1: case cell.type.W4g2:
            case cell.type.W5s: case cell.type.W6b: case cell.type.W7pu: case cell.type.W8pi: case cell.type.W9r: 
            case cell.type.WZb:
            case cell.type.Tnt: 
            c.SetWall(type);
            area.SetSafeZ(c.pt.z);
            break;

            case cell.type.Br0w: case cell.type.Br1y: case cell.type.Br2o: case cell.type.Br3g1: case cell.type.Br4g2:
            case cell.type.Br5s: case cell.type.Br6b: case cell.type.Br7pu: case cell.type.Br8pi: case cell.type.Br9r: 
            case cell.type.BrZb:
            c.SetPath(type);
            i2 endPt = new i2(arJs.Int, arJs.Int);
            cell.type endType = (cell.type)arJs.Int;
            baller.data.type datatype = (baller.data.type)arJs.Int;
            cell endcell = null;
            if(endPt.x > 0)
            {
                endcell = cells.Get(endPt.x, endPt.z);
                endcell.SetPath(endType);
            }
            baller.SetBaller_AddData(c, endcell, datatype);
            while(arJs.i < arJs.num)
                baller.datas[c.ddx].pts.Add(new f2(arJs.F, arJs.F));
            area.SetSafeZ(c.pt.z);
            break;

            default:
            c.SetPath(type);
            area.SetSafeZ(c.pt.z);
            break;
        }
    }
    
    public static void SetCellInfo(int x, int z, cell.type type)
    {
        cell c = cells.Get(x, z);
        switch(type)
        {
            case cell.type.Finish:
            c.SetPath(type);
            break;
            
            case cell.type.W0w: case cell.type.W1y: case cell.type.W2o: case cell.type.W3g1: case cell.type.W4g2:
            case cell.type.W5s: case cell.type.W6b: case cell.type.W7pu: case cell.type.W8pi: case cell.type.W9r: 
            case cell.type.WZb:
            case cell.type.Tnt: 
            c.SetWall(type);
            break;

            case cell.type.Br0w: case cell.type.Br1y: case cell.type.Br2o: case cell.type.Br3g1: case cell.type.Br4g2:
            case cell.type.Br5s: case cell.type.Br6b: case cell.type.Br7pu: case cell.type.Br8pi: case cell.type.Br9r: 
            case cell.type.BrZb:
            c.SetPath(type);
            baller.SetBaller_AddData(c, null, baller.data.type.toHero);
            break;

            default:
            c.SetPath(type);
            break;
        }
    }
}
