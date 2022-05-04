using UnityEngineExJSON;
using UnityEngineEx;

public partial class stageEditor
{
    public enum saveType:byte{normal, reverse, decalFrLeft, decalFrRight, shift}
    public enum randType:byte{r0, r1, r2, r3, r4, r5,  r6, r7, r8, r9, r10}
    
    public void ReadJson(string strJson)
    {
        area.InitSafeZ();
        JSONObject jsStage = new JSONObject(strJson)[jsK.Stage];
        jsK.ObjInfo = jsStage[jsK.Info];
        jsK.ObjCells = jsStage[jsK.Cells];
        
        objs.Clear();
        
        for (int i = 0; i < jsK.ObjCells.Count; ++i)
            loads.ParseCellInfo(new jsArr(jsK.ObjCells[i]));
        hero.culling.Refresh();
    }
    
    public string CreateJson(saveType savetype, randType randtype)
    {
        int gap = 5 + UnityEngineEx.RandEx.GetN(3);

        JSONObject jsRoot = new JSONObject(); // for return jsRoot.Print()
        JSONObject jsStage = NewJSONObj.With(jsK.Stage, jsRoot, JSONObject.Type.OBJECT);
        
        JSONObject jsCells = NewJSONObj.With(jsK.Cells, jsStage, JSONObject.Type.ARRAY);

        int x1 = 1; int x2 = cells.MaxX-1;
        switch(savetype) {
            case saveType.decalFrLeft: x2=7; break;
            case saveType.decalFrRight: x1=6; break;
        }
        for(int x=x1; x<x2;++x)
        for(int z=1; z<cells.MaxZ-1;++z)
        {
            cell c = cells.Get(x,z);
            if(c.Type == cell.type.PathEnd)
                continue;
            if(c.Type != cell.type.None && c.Type != cell.type.Path && !c.IsBallr())
            {
                JSONObject js = NewJSONObj.With(jsCells, JSONObject.Type.ARRAY);
                switch(savetype)
                {
                    default: js.Add(c.pt.x); break;
                    case saveType.reverse: js.Add(12 - c.pt.x); break;
                    case saveType.shift:
                    int tmpX = c.pt.x + gap;
                    js.Add(tmpX<=11 ? tmpX: (tmpX%12) + 1);
                    break;
                }
                js.Add(c.pt.z);

                if(c.Type == cell.type.W0w && randtype != randType.r0)
                    js.Add((int)getRandWallByRandType(randtype));
                else 
                    js.Add((int)c.Type);

                switch(savetype) {
                    case saveType.decalFrLeft: 
                    case saveType.decalFrRight: 
                    if(x != 6)
                    {
                        js = NewJSONObj.With(jsCells, JSONObject.Type.ARRAY);
                        js.Add(12 - c.pt.x);
                        js.Add(c.pt.z);

                        if(c.Type == cell.type.W0w && randtype != randType.r0)
                            js.Add((int)getRandWallByRandType(randtype));
                        else 
                            js.Add((int)c.Type);
                    }
                    break;
                }
            }
            else if(c.IsBallr() && c.Type < cell.type.Br0w_)
            {
                baller.data data = baller.datas[c.ddx];

                JSONObject js = NewJSONObj.With(jsCells, JSONObject.Type.ARRAY);
                js.Add(c.pt.x);
                js.Add(c.pt.z);
                js.Add((int)c.Type);
                if(data.end != null)
                {
                    switch(savetype)
                    {
                        case saveType.reverse:
                        js.Add(12 - data.end.pt.x);
                        break;
                        default:
                        js.Add(data.end.pt.x);
                        break;
                    }
                    js.Add(data.end.pt.z);
                    js.Add((int)data.end.Type);
                }
                else { js.Add(0); js.Add(0); js.Add(0); }
                
                js.Add((int)data.Type);
                for(int i=0; i<data.pts.Num; ++i)
                {
                    f2 pt = data.pts[i];
                    switch(savetype)
                    {
                        case saveType.reverse:
                        js.Add(12 - pt.x);
                        break;
                        default:
                        js.Add(pt.x);
                        break;
                    }
                    js.Add(pt.z);
                }
                
            }
        }
        
        JSONObject jsInfo = NewJSONObj.With(jsK.Info, jsStage, JSONObject.Type.OBJECT);
        //jsInfo.AddField("numOs", (int)0);

        string result = jsRoot.Print(); //Debug.Log(jsRoot.Print());
        jsRoot.Clear();
        return result;
    }



    
    int getRandWallByRandType(randType randtype)
    {
        switch(randtype)
        {
            case randType.r1: return getRandWall(30,25,11,8,6,  /*80*/ 6,5,4,3,2     /*20*/);
            case randType.r2: return getRandWall(25,18,15,12,10,/*80*/ 6,5,4,3,2     /*20*/);
            case randType.r3: return getRandWall(20,15,11,10,10,/*66*/ 9,8,7,6,4     /*34*/);
            case randType.r4: return getRandWall(15,13,12,10,10,/*50*/ 10,10,10,10,5 /*50*/);
            case randType.r5: return getRandWall(10,10,10,10,10,/*50*/ 10,10,10,10,10/*50*/);
            case randType.r6: return getRandWall(5,6,7,8,9,     /*35*/ 15,15,15,10,10/*65*/);
            case randType.r7: return getRandWall(4,5,6,7,8,     /*30*/ 15,15,15,15,10/*70*/);
            case randType.r8: return getRandWall(5,6,7,8,9,     /*35*/ 10,11,14,15,15/*65*/);
            case randType.r9: return getRandWall(5,6,7,7,8,     /*33*/ 9,10,13,15,20 /*67*/);
            case randType.r10:return getRandWall(2,3,4,5,6,     /*20*/ 10,12,15,18,25/*80*/);
        }

        return (int)cell.type.W0w;
    }

    int getRandWall(params int[] p)
    {
        int rand = RandEx.GetN(100);
        int min=0, max = p[0];
        
        if(min <= rand && rand < max) return (int)cell.type.W0w;
        min = max; max = max + p[1];
        if(min <= rand && rand < max) return (int)cell.type.W1y;
        min = max; max = max + p[2];
        if(min <= rand && rand < max) return (int)cell.type.W2o;
        min = max; max = max + p[3];
        if(min <= rand && rand < max) return (int)cell.type.W3g1;
        min = max; max = max + p[4];
        if(min <= rand && rand < max) return (int)cell.type.W4g2;
        min = max; max = max + p[5];
        if(min <= rand && rand < max) return (int)cell.type.W5s;
        min = max; max = max + p[6];
        if(min <= rand && rand < max) return (int)cell.type.W6b;
        min = max; max = max + p[7];
        if(min <= rand && rand < max) return (int)cell.type.W7pu;
        min = max; max = max + p[8];
        if(min <= rand && rand < max) return (int)cell.type.W8pi;
        min = max; max = max + p[9];
        if(min <= rand && rand < max) return (int)cell.type.W9r;
        return (int)cell.type.W0w;
    }



}
