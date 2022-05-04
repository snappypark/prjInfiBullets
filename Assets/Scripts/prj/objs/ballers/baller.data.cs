using UnityEngine;

public partial class baller
{
    public static arr<data> datas = new arr<data>(ballers.max);
    data _data{get{return datas[_cell.ddx];} set{ datas[_cell.ddx] = value;}}

    public static void SetBaller_AddData(
        cell beginCell, cell endCell, baller.data.type dataType)
    {
        short ddx = (short)datas.Num; 
        beginCell.ddx = ddx;
        datas.CountNum();
        datas[ddx].Init(beginCell, endCell, dataType);
    }

    public static void ClearData()
    {
        for(int i=0; i<datas.Num; ++i)
            datas[i].pts.Clear();
        datas.Clear();
    }

    public class data
    {
        public enum type:byte { toHero=0, way1, }
     
        public type Type;
        public arr<f2> pts = new arr<f2>(16);

        public cell begin, end;
        public bool beginHasLine;
        public bool endHasLine;
        public void Init(cell c1, cell c2, type type_)
        {
            Type = type_;
            
            begin = c1;
            switch(type_)
            {
                case type.way1:
                SetEnd(c2);
                break;
                case type.toHero:
                beginHasLine = false;
                endHasLine = false;
                break;
            }
        }

        public void SetEnd(cell cell)
        {
            end = cell;
            if(cell != null)
            {
                cell.ddx = begin.ddx;
                beginHasLine = begin.pt.z > end.pt.z;
                endHasLine = !beginHasLine;
            }
        }

        public void AddPt()
        {

        }
    }

}
