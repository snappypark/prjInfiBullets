using UnityEngine;

public partial class cells
{
    public static void InitStraights()
    {
        for (int x = bd.X0; x <= bd.X1; ++x)
        {
            _zStraights[x] = 1;
            for(int z=1; z<bd.Z1; ++z)
            {
                cell c = _inst._pool[x,z];

                if(c.IsPath)
                {
                    c.IsStraight = true;
                    ++_zStraights[x];
                }
                else
                    break;

            }
        }
    }

    static int[] _zStraights = new int[MaxX]{0,0,0,0,0, 0,0,0,0,0, 0,0,0};
    static int _xStraight=1;
    public static void StraightOnPulse(int count = 10)
    {
        ++_xStraight;
        if(_xStraight > bd.X1)
            _xStraight = bd.X0;

        int z = _zStraights[_xStraight];
        
        for(int i=0; i<count; ++i)
        {
            if(z > bd.Z1)
                break;
            cell c = cells.Get(_xStraight, z);
            if(c.IsPath)
            {
                c.IsStraight = true;
                ++z;
            }
            else
                break;
        }
        _zStraights[_xStraight] = z;
    }
}
