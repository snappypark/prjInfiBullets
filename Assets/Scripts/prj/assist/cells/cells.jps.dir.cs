using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class cells
{
    public static i4 bd = new i4(1,1, MaxX-2, MaxZ-2);
    
    eDirections[] allDirections = Enum.GetValues(typeof(eDirections)).Cast<eDirections>().ToArray();

    eDirections[] getAllValidDirections(PathNode curr_node)
    {
        // If parent is null, then explore all possible directions
        return curr_node.from == null ?
            allDirections :
            validDirLookUpTable[curr_node.dirFromParent];
    }

    Dictionary<eDirections, eDirections[]> validDirLookUpTable = new Dictionary<eDirections, eDirections[]>
    {
        { eDirections.S,      new []{ eDirections.W,  eDirections.SW, eDirections.S, eDirections.SE, eDirections.E } },
        { eDirections.SE, new []{ eDirections.S, eDirections.SE, eDirections.E } },
        { eDirections.E,       new []{ eDirections.S, eDirections.SE, eDirections.E, eDirections.NE, eDirections.N } },
        { eDirections.NE, new []{ eDirections.E,  eDirections.NE, eDirections.N } },
        { eDirections.N,      new []{ eDirections.E,  eDirections.NE, eDirections.N, eDirections.NW, eDirections.W } },
        { eDirections.NW, new []{ eDirections.N, eDirections.NW, eDirections.W } },
        { eDirections.W,       new []{ eDirections.N, eDirections.NW, eDirections.W, eDirections.SW, eDirections.S } },
        { eDirections.SW, new []{ eDirections.W,  eDirections.SW, eDirections.S } }
    };


    private bool isCardinal(eDirections dir)
    {
        switch (dir) {
            case eDirections.S:
            case eDirections.E:
            case eDirections.N:
            case eDirections.W:
                return true;
        }
        return false;
    }

    private bool isDiagonal(eDirections dir)
    {
        switch (dir) {
            case eDirections.SE:
            case eDirections.SW:
            case eDirections.NE:
            case eDirections.NW:
                return true;
        }
        return false;
    }

    private bool goalIsInExactDirection(i2 curr, eDirections dir, i2 goal)
    {
        int diff_column = goal.x - curr.x;
        int diff_row = goal.z - curr.z;
        switch (dir)
        {
            case eDirections.N:
                return diff_row > 0 && diff_column == 0;
            case eDirections.NE:
                return diff_row > 0 && diff_column > 0 && diff_row == diff_column;
            case eDirections.E:
                return diff_row == 0 && diff_column > 0;
            case eDirections.SE:
                return diff_row < 0 && diff_column > 0 && -diff_row == diff_column;
            case eDirections.S:
                return diff_row < 0 && diff_column == 0;
            case eDirections.SW:
                return diff_row < 0 && diff_column < 0 && diff_row == diff_column;
            case eDirections.W:
                return diff_row == 0 && diff_column < 0;
            case eDirections.NW:
                return diff_row > 0 && diff_column < 0 && -diff_row == diff_column;
        }
        return false;
    }

    private bool goalIsInGeneralDirection(i2 curr, eDirections dir, i2 goal)
    {
        switch (dir)
        {
            case eDirections.N:
                return goal.z > curr.z && goal.x == curr.x;
            case eDirections.NE:
                return goal.z > curr.z && goal.x > curr.x;
            case eDirections.E:
                return goal.z == curr.z && goal.x > curr.x;
            case eDirections.SE:
                return goal.z < curr.z && goal.x > curr.x;
            case eDirections.S:
                return goal.z < curr.z && goal.x == curr.x;
            case eDirections.SW:
                return goal.z < curr.z && goal.x < curr.x;
            case eDirections.W:
                return goal.z == curr.z && goal.x < curr.x;
            case eDirections.NW:
                return goal.z > curr.z && goal.x < curr.x;
        }
        return false;
    }

    private PathNode getNodeDist(int x, int z, eDirections direction, int dist)
    {
        int new_x = x, new_z = z;
        switch (direction)
        {
            case eDirections.N:
                new_z += dist;
                break;
            case eDirections.NE:
                new_z += dist;
                new_x += dist;
                break;
            case eDirections.E:
                new_x += dist;
                break;
            case eDirections.SE:
                new_z -= dist;
                new_x += dist;
                break;
            case eDirections.S:
                new_z -= dist;
                break;
            case eDirections.SW:
                new_z -= dist;
                new_x -= dist;
                break;
            case eDirections.W:
                new_x -= dist;
                break;
            case eDirections.NW:
                new_z += dist;
                new_x -= dist;
                break;
        }

        // w/ the new coordinates, get the node
        if (bd.HasPt(new_x, new_z))
            return cells.Get(new_x, new_z).pNode;
        Debug.LogWarning(new_x + ", " + new_z);
        return null;
    }

    static readonly float SQRT_2 = Mathf.Sqrt(2);
    static readonly float SQRT_2_MINUS_1 = Mathf.Sqrt(2) - 1.0f;

    internal static int octileHeuristic(int curr_row, int curr_column, int goal_row, int goal_column)
    {
        int column_dist = goal_column - curr_column;
        int row_dist = goal_row - curr_row;
        return (int)(Mathf.Max(row_dist, column_dist) + SQRT_2_MINUS_1 * Mathf.Min(row_dist, column_dist));
    }


    /////////////////////////////////////////
}
