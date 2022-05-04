using UnityEngine;

public class bd
{
    public static readonly float left = 1f;
    public static readonly float right = cells.MaxX-1;
    
    public static readonly float left_25 = left + 0.25f;
    public static readonly float right_25 = cells.MaxX-1.25f;
}

public static class I_I
{
    public static readonly Vector3 Center = new Vector3(12.5f, 18.5f);
    public static readonly float Width = 12;
    public static readonly float Height = 18;
    public static readonly float HalfWidth = 6f;
    public static readonly float HalfHeight = 9f;
    public static readonly float MinX = Center.x - HalfWidth;
    public static readonly float MaxX = Center.x + HalfWidth;
    public static readonly float MinY = Center.y - HalfHeight;
    public static readonly float MaxY = Center.y + HalfHeight;
    
    public static readonly float MinX_ = Center.x - HalfWidth*1.03f;
    public static readonly float MaxX_ = Center.x + HalfWidth*1.03f;
    public static readonly float MinY_ = Center.y - HalfHeight*1.015f;
    public static readonly float MaxY_ = Center.y + HalfHeight*1.015f;

    public static readonly float MinX_1 = Center.x - HalfWidth*1.22f;
    public static readonly float MaxX_1 = Center.x + HalfWidth*1.22f;
    public static readonly float MinY_1 = Center.y - HalfHeight*1.11f;
    public static readonly float MaxY_1 = Center.y + HalfHeight*1.11f;


    public static bool IsOut1(Vector3 pos)
    {
        return pos.x < MinX_1 || pos.x > MaxX_1 || 
                pos.y < MinY_ || pos.y > MaxY_;
    }
}
