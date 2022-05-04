using UnityEngine;

public enum eDirections
{
    N = 0,
    NE = 1,
    E = 2,
    SE = 3,
    S = 4,
    SW = 5,
    W = 6,
    NW = 7,
}

public class PathNode
{
    public cell cell;
    public eDirections dirFromParent;
    public PathNode from = null;
    public int givenCost = 0;
    public int finalCost = 0;
    public bool isOpen = false;

    public void Reset()
    {
        this.from = null;
        this.givenCost = 0;
        this.finalCost = 0;
        this.isOpen = false;
    }
}

public class jpInfo {
    public const byte N = 0, NE = 1, E = 2, SE = 3, S = 4, SW = 5, W = 6, NW = 7;
}

public partial class cell
{
    public int JpDist(int idx)
    {
        switch (cells.jspState)
        {
            case cells.eJspState.Build1_Result_Reset:
                return jpDists_[idx];
            default:
                return jpDists[idx];
        }
    }
    public int[] jpDists = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0, };
    public int[] jpDists_ = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0, };

    public bool[] jpDir = new bool[8] { false, false, false, false, false, false, false, false, };
    public bool isJumpPoint = false;

    public bool isJumpPointComingFrom(int dir)
    {
        return this.isJumpPoint && this.jpDir[dir];
    }

    public PathNode pNode = new PathNode();

    public void ResultAndReset()
    {
        for (int i = 0; i < 8; ++i)
        {
            jpDists[i] = jpDists_[i];
            //jpDists_[i] = 0;
            jpDir[i] = false;
        }
        isJumpPoint = false;

        pNode.Reset();
    }
}
