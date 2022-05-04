using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class cell
{
    public enum type:byte {
        None, 
                Path, 
                Finish, W0_Floor, 
                Food, FoodA, FoodB, FoodC, FoodD,
                Spdx2, Spdx3, Spdx4, Spdx5, Spdx6, 
                SpdxA, SpdxB, SpdxC, SpdxD, SpdxE, 
                
                TrapSmall, TrapBig, TrapA, TrapB, TrapC,
                Traper, TraperI,TraperL,TraperR, TraperB,

                Tnt, BoxRandom, BoxA,BoxB,BoxC,BoxD,BoxE,BoxF,

                W0w, W1y, W2o, W3g1, W4g2, W5s, W6b, W7pu, W8pi, W9r, WZb,
                WzA, WzB, WzC, WzD, WzE, WzF, 

                B0w, B1y, B2o, B3g1, B4g2, B5s, B6b, B7pu, B8pi, B9r, BZb,
                BzA, BzB, BzC, BzD, BzE, BzF, 
                
                Br0w, Br1y, Br2o, Br3g1, Br4g2, Br5s, Br6b, Br7pu, Br8pi, Br9r, BrZb,
                BrzA, BrzB, BrzC, BrzD, BrzE, BrzF, 
                Br0w_, Br1y_, Br2o_, Br3g1_, Br4g2_, Br5s_, Br6b_, Br7pu_, Br8pi_, Br9r_, BrZb_,
                BrzA_, BrzB_, BrzC_, BrzD_, BrzE_, BrzF_, 
                
                PathBegin, PathEnd, PathA, PathB,
                }

    public type Type;

    public bool HasObj {get{return cdx != -1;}}
    public bool IsPath = false;
    public bool IsStraight = false;

    public i2 pt; public f2 ct;
    public short cdx = -1;
    public short ddx = -1;
    public Queue<short> cdxs = new Queue<short>();

    public void SetWall(type type_) { Type = type_; IsPath = false;}
    public void SetPath(type type_ = type.Path){ Type = type_; IsPath = true; }
    
    public void Clear()
    {
        Type = type.Path;
        IsPath = true;
        IsStraight = false;
        cdxs.Clear();
        cdx = -1;
        ddx = -1;
    }
    
    public cell East() { return cells.Get(pt.x+1, pt.z); }
    public cell West() { return cells.Get(pt.x-1, pt.z); }
    public cell North() { return cells.Get(pt.x, pt.z+1); }
    public cell South() { return cells.Get(pt.x, pt.z-1); }
    public cell NorthEast() { return cells.Get(pt.x + 1, pt.z + 1); }
    public cell NorthWest() { return cells.Get(pt.x - 1, pt.z + 1); }
    public cell SouthEast() { return cells.Get(pt.x + 1, pt.z - 1); }
    public cell SouthWest() { return cells.Get(pt.x - 1, pt.z - 1); }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static i2 Pt(float x, float z) { return new i2((int)(x), (int)(z)); }
    
    public static i2 Pt11(Vector3 pos) { return new i2((int)(pos.x+0.5f), (int)(pos.z + 0.5f)); }
    public i2 Pt2X2(int idx) { return new i2(pt.x + Idx.INxN(idx), pt.z + Idx.JNxN(idx)); }
    public cell C2X2(int idx) { return cells.Get(pt.x + Idx.INxN(idx), pt.z + Idx.JNxN(idx)); }
    

    //
    public bool IsWall()
    {
        return Type >= type.W0w && Type <= type.WzF;
    }
    public bool IsBallr()
    {
        return Type >= type.Br0w && Type <= type.BrzF_;
    }
}
