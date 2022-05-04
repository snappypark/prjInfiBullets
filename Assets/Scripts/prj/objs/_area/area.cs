using UnityEngine;

public class area : MonoBehaviour
{
    static area _inst;
    public static beginfield begin;
    public static endfield end;
    public static groundfield ground;
    public static pillars pillars;
    [SerializeField] beginfield _begin;
    [SerializeField] endfield _end;
    [SerializeField] groundfield _ground;
    [SerializeField] pillars _pillars;
    
    void Awake()
    {
        _inst = this;
        begin = _begin;
        end = _end;
        ground = _ground;
        pillars = _pillars;
    }

    public static void InitOnEdit()
    {
        endfield.z = cells.MaxZ;
        Init(-1);
    }

    public static void InitSafeZ()
    {
        safeEndZ = 0;
        safeBeginZ = cells.MaxZ;
    }

    public static void Init(int stageIdx)
    {
        _inst._begin.Init(stageIdx);
        _inst._end.Init(stageIdx);
        _inst._pillars.Generate();
        if(stageIdx == -1)
            return;
        //
        for(int z=safeEndZ+1; z<cells.MaxZ; ++z)
        {
            for(int x=1; x<=cells.bd.xMax; ++x)
            {
                cell c = cells.Get(x,z);
                if(c.Type == cell.type.Finish)
                    return;
                c.Type = cell.type.PathEnd;
            }
        }
    }

    
    public static void SetSafeZ(int z)
    {
        if(z > safeEndZ)
            safeEndZ = z;
        if(z < safeBeginZ)
            safeBeginZ = z;
    }
    

    public static int safeEndZ;
    public static int safeBeginZ;

    static int _safeBeginX=1;
    public static void SafeBeginZOnPulse(int count = 10)
    {
        if(safeBeginZ >= safeEndZ)
            return;
        while(_safeBeginX<=cells.bd.X1)
        {
            if(cells.Get(_safeBeginX, safeBeginZ).IsPath)
                ++_safeBeginX;
            else
                return;
        }
        ++safeBeginZ;
        _safeBeginX = 1;
    }
}
