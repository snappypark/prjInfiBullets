using System.Runtime.CompilerServices;
using System.Collections;
using UnityEngine;

public partial class cells
{
    public enum eJspState : byte
    {
        None,
        Step0_CheckChange,
        Build1_JumpPoints,
        Build1_SJP_1,
        Build1_SJP_2,
        Build1_DJP_1,
        Build1_DJP_2,
        Build1_Result_Reset,
    }

    public static bool changed = false;
    public static eJspState jspState = eJspState.None;
    int _jx, _jz, _jj = 0;
    int _jumpDistSoFar = -1;
    bool _jpSeen = false;

    public static IEnumerator InitJsp_()
    {
        yield return _inst.initJsp_();
    }

    IEnumerator initJsp_()
    {
        ResetJspState(eJspState.Build1_Result_Reset);
        while (++_jz <= bd.Z1)
        {
            for (int x = bd.X0; x <= bd.X1; ++x)
                cells.Get(x, _jz).ResultAndReset();
        }
        yield return null;
        ResetJspState(eJspState.Build1_JumpPoints);
        while (++_jz <= bd.Z1)
            checkJumpPoints();
        yield return null;
        ResetJspState(eJspState.Build1_SJP_1);
        while (++_jz <= bd.Z1)
            checkSJP_1();
        yield return null;
        ResetJspState(eJspState.Build1_SJP_2);
        while (++_jx <= bd.X1)
            checkSJP_2();
        yield return null;
        ResetJspState(eJspState.Build1_DJP_1);
        while (++_jz <= bd.Z1)
            checkDJP_1();
        yield return null;
        ResetJspState(eJspState.Build1_DJP_2);
        while (--_jz >= bd.Z0)
            checkDJP_2();
        ResetJspState(eJspState.Build1_Result_Reset);
        while (++_jz <= bd.Z1)
        {
            for (int x = bd.X0; x <= bd.X1; ++x)
                cells.Get(x, _jz).ResultAndReset();
        }
        ResetJspState(eJspState.Step0_CheckChange);
    }

    public static void OnUpdate_Jsp()
    {
        _inst.onUpdate_Jsp();
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    void onUpdate_Jsp()
    {
        switch (jspState)
        {
            case eJspState.None:
                break;
            case eJspState.Step0_CheckChange:
                if (changed)
                {
                    ResetJspState(eJspState.Build1_JumpPoints);
                    changed = false;
                }
                break;
            case eJspState.Build1_JumpPoints:
                if (++_jz <= bd.Z1)
                    checkJumpPoints();
                else
                    ResetJspState(eJspState.Build1_SJP_1);
                break;
            case eJspState.Build1_SJP_1:
                if (++_jz <= bd.Z1)
                    checkSJP_1();
                else
                    ResetJspState(eJspState.Build1_SJP_2);
                break;
            case eJspState.Build1_SJP_2:
                if (++_jx <= bd.X1)
                    checkSJP_2();
                else
                    ResetJspState(eJspState.Build1_DJP_1);
                break;
            case eJspState.Build1_DJP_1:
                if (++_jz <= bd.Z1)
                    checkDJP_1();
                else
                    ResetJspState(eJspState.Build1_DJP_2);
                break;
            case eJspState.Build1_DJP_2:
                if (--_jz >= bd.Z0)
                    checkDJP_2();
                else
                    ResetJspState(eJspState.Build1_Result_Reset);
                break;
            case eJspState.Build1_Result_Reset:
                if (++_jz <= bd.Z1)
                    for (int x = bd.X0; x <= bd.X1; ++x)
                        cells.Get(x, _jz).ResultAndReset();
                else
                {
                    ResetJspState(eJspState.Step0_CheckChange);
                    }
                break;
        }
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void ResetJspState(eJspState state)
    {
        jspState = state;
        _jumpDistSoFar = -1;
        _jpSeen = false;
        switch (jspState)
        {
            case eJspState.Build1_JumpPoints:
                _jz = bd.Z0-1;
                break;
            case eJspState.Build1_SJP_1:
            case eJspState.Build1_DJP_1:
            case eJspState.Build1_Result_Reset:
                _jz = bd.Z0-1;
                break;
            case eJspState.Build1_SJP_2:
                _jx = bd.X0-1;
                break;
            case eJspState.Build1_DJP_2:
                _jz = bd.Z1+1;
                break;
        }
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    void checkJumpPoints()
    {
        for (_jx = bd.X0; _jx <= bd.X1; ++_jx)
        {
            cell c = cells.Get(_jx, _jz);
            c.jpDists_[0] = 0; c.jpDists_[1] = 0; c.jpDists_[2] = 0; c.jpDists_[3] = 0;
            c.jpDists_[4] = 0; c.jpDists_[5] = 0; c.jpDists_[6] = 0; c.jpDists_[7] = 0;
            if (c.IsPath)
                continue;
            c = getNorthEastCell();
            if (null != c && c.IsPath && c.West().IsPath && c.South().IsPath)
            {
                c.isJumpPoint = true;
                c.jpDir[jpInfo.S] = true;
                c.jpDir[jpInfo.W] = true;
            }
            c = getSouthEastCell();
            if (null != c && c.IsPath && c.West().IsPath && c.North().IsPath)
            {
                c.isJumpPoint = true;
                c.jpDir[jpInfo.N] = true;
                c.jpDir[jpInfo.W] = true;
            }
            c = getSouthWestCell();
            if (null != c && c.IsPath && c.North().IsPath && c.East().IsPath)
            {
                c.isJumpPoint = true;
                c.jpDir[jpInfo.N] = true;
                c.jpDir[jpInfo.E] = true;
            }
            c = getNorthWestCell();
            if (null != c && c.IsPath && c.South().IsPath && c.East().IsPath)
            {
                c.isJumpPoint = true;
                c.jpDir[jpInfo.S] = true;
                c.jpDir[jpInfo.E] = true;
            }
        }
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    void checkSJP_1()
    {
        _jumpDistSoFar = -1;
        _jpSeen = false;
        for (_jx = bd.X0; _jx <= bd.X1; ++_jx)
        {
            cell c = cells.Get(_jx, _jz);
            if (!c.IsPath)
            {
                _jumpDistSoFar = -1;
                _jpSeen = false;
                c.jpDists_[jpInfo.W] = 0;
                continue;
            }

            ++_jumpDistSoFar;
            if (_jpSeen)
                c.jpDists_[jpInfo.W] = _jumpDistSoFar;
            else
                c.jpDists_[jpInfo.W] = -_jumpDistSoFar;   // Set wall distance

            if (c.isJumpPointComingFrom(jpInfo.E))
            {
                _jumpDistSoFar = 0;
                _jpSeen = true;
            }
        }

        _jumpDistSoFar = -1;
        _jpSeen = false;
        for (_jx = bd.X1; _jx >= bd.X0; --_jx)
        {
            cell c = cells.Get(_jx, _jz);
            if (!c.IsPath)
            {
                _jumpDistSoFar = -1;
                _jpSeen = false;
                c.jpDists_[jpInfo.E] = 0;
                continue;
            }

            ++_jumpDistSoFar;
            if (_jpSeen)
                c.jpDists_[jpInfo.E] = _jumpDistSoFar;
            else
                c.jpDists_[jpInfo.E] = -_jumpDistSoFar;   // Set wall distance

            if (c.isJumpPointComingFrom(jpInfo.W))
            {
                _jumpDistSoFar = 0;
                _jpSeen = true;
            }
        }
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    void checkSJP_2()
    {
        _jumpDistSoFar = -1;
        _jpSeen = false;
        for (_jz = bd.Z0; _jz <= bd.Z1; ++_jz)
        {
            cell c = cells.Get(_jx, _jz);
            if (!c.IsPath)
            {
                _jumpDistSoFar = -1;
                _jpSeen = false;
                c.jpDists_[jpInfo.S] = 0;
                continue;
            }

            ++_jumpDistSoFar;
            if (_jpSeen)
                c.jpDists_[jpInfo.S] = _jumpDistSoFar;
            else
                c.jpDists_[jpInfo.S] = -_jumpDistSoFar;   // Set wall distance

            if (c.isJumpPointComingFrom(jpInfo.N))
            {
                _jumpDistSoFar = 0;
                _jpSeen = true;
            }
        }
        _jumpDistSoFar = -1;
        _jpSeen = false;
        for (_jz = bd.Z1; _jz >= bd.Z0; --_jz)
        {
            cell c = cells.Get(_jx, _jz);
            if (!c.IsPath)
            {
                _jumpDistSoFar = -1;
                _jpSeen = false;
                c.jpDists_[jpInfo.N] = 0;
                continue;
            }

            ++_jumpDistSoFar;
            if (_jpSeen)
                c.jpDists_[jpInfo.N] = _jumpDistSoFar;
            else
                c.jpDists_[jpInfo.N] = -_jumpDistSoFar;   // Set wall distance

            if (c.isJumpPointComingFrom(jpInfo.S))
            {
                _jumpDistSoFar = 0;
                _jpSeen = true;
            }
        }
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    void checkDJP_1()
    {
        for (_jx = bd.X0; _jx <= bd.X1; ++_jx)
        {
            cell c = cells.Get(_jx, _jz);
            if (!c.IsPath)
                continue;
                
            if (_jx == bd.X0 || _jz == bd.Z0 ||  // If we in the north west corner
                (!c.West().IsPath || !c.South().IsPath || !c.SouthWest().IsPath))     // if the node to the North west is an obstacle
                c.jpDists_[jpInfo.SW] = 0;
            else if (c.South().IsPath && c.West().IsPath &&
                    (c.SouthWest().jpDists_[jpInfo.S] > 0 ||    // If the node to the north west has is a straight jump point ( or primary jump point) going north
                     c.SouthWest().jpDists_[jpInfo.W] > 0))     // If the node to the north west has is a straight jump point ( or primary jump point) going West
                c.jpDists_[jpInfo.SW] = 1;
            else
            {
                int jumpDistance = c.SouthWest().jpDists_[jpInfo.SW];
                if (jumpDistance > 0)
                    c.jpDists_[jpInfo.SW] = 1 + jumpDistance;
                else //if( jumpDistance <= 0 )
                    c.jpDists_[jpInfo.SW] = -1 + jumpDistance;
            }

            if (_jx == bd.X1 || _jz == bd.Z0 ||          // If we in the top right corner
                (!c.South().IsPath || !c.East().IsPath || !c.SouthEast().IsPath))         // If the node to the north is an obstacle
                c.jpDists_[jpInfo.SE] = 0;
            else if (c.South().IsPath && c.East().IsPath &&                                                          // if the node to the east is empty
                    (c.SouthEast().jpDists_[jpInfo.S] > 0 ||    // If the node to the north east has is a straight jump point ( or primary jump point) going north
                     c.SouthEast().jpDists_[jpInfo.E] > 0))     // If the node to the north east has is a straight jump point ( or primary jump point) going east
                c.jpDists_[jpInfo.SE] = 1;
            else
            {
                int jumpDistance = c.SouthEast().jpDists_[jpInfo.SE];
                if (jumpDistance > 0)
                    c.jpDists_[jpInfo.SE] = 1 + jumpDistance;
                else //if( jumpDistance <= 0 )
                    c.jpDists_[jpInfo.SE] = -1 + jumpDistance;
            }
        }
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    void checkDJP_2()
    {
        for (_jx = bd.X0; _jx <= bd.X1; ++_jx)
        {
            cell c = cells.Get(_jx, _jz);
            if (!c.IsPath)
                continue;

            if (_jx == bd.X0 || _jz == bd.Z1 ||  // If we in the north west corner
                (!c.West().IsPath || !c.North().IsPath || !c.NorthWest().IsPath))     // if the node to the North west is an obstacle
                c.jpDists_[jpInfo.NW] = 0;
            else if (c.North().IsPath && c.West().IsPath &&
                    (c.NorthWest().jpDists_[jpInfo.N] > 0 ||    // If the node to the north west has is a straight jump point ( or primary jump point) going north
                     c.NorthWest().jpDists_[jpInfo.W] > 0))     // If the node to the north west has is a straight jump point ( or primary jump point) going West
                c.jpDists_[jpInfo.NW] = 1;
            else
            {
                int jumpDistance = c.NorthWest().jpDists_[jpInfo.NW];
                if (jumpDistance > 0)
                    c.jpDists_[jpInfo.NW] = 1 + jumpDistance;
                else //if( jumpDistance <= 0 )
                    c.jpDists_[jpInfo.NW] = -1 + jumpDistance;
            }

            if (_jx == bd.X1 || _jz == bd.Z1 ||          // If we in the top right corner
                (!c.North().IsPath || !c.East().IsPath || !c.NorthEast().IsPath))         // If the node to the north is an obstacle
                c.jpDists_[jpInfo.NE] = 0;
            else if (c.North().IsPath && c.East().IsPath &&                      // if the node to the east is empty
                    (c.NorthEast().jpDists_[jpInfo.N] > 0 ||    // If the node to the north east has is a straight jump point ( or primary jump point) going north
                     c.NorthEast().jpDists_[jpInfo.E] > 0))     // If the node to the north east has is a straight jump point ( or primary jump point) going east
                c.jpDists_[jpInfo.NE] = 1;
            else
            {
                int jumpDistance = c.NorthEast().jpDists_[jpInfo.NE];
                if (jumpDistance > 0)
                    c.jpDists_[jpInfo.NE] = 1 + jumpDistance;
                else //if( jumpDistance <= 0 )
                    c.jpDists_[jpInfo.NE] = -1 + jumpDistance;
            }
        }
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    cell getNorthEastCell()
    {
        if (_jx >= bd.X1 || _jz >= bd.Z1)
            return null;
        return cells.Get(_jx + 1, _jz + 1);
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    cell getSouthEastCell()
    {
        if (_jx >= bd.X1 || _jz <= bd.Z0)
            return null;
        return cells.Get(_jx + 1, _jz - 1);
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    cell getSouthWestCell()
    {
        if (_jx <= bd.X0 || _jz <= bd.Z0)
            return null;
        return cells.Get(_jx - 1, _jz - 1);
    }

#if UNITY_EDITOR
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    cell getNorthWestCell()
    {
        if (_jx <= bd.X0 || _jz >= bd.Z1)
            return null;
        return cells.Get(_jx - 1, _jz + 1);
    }
    
}
