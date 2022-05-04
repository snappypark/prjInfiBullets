using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class cells/*path*/
{
    Queue<i2> usedNode = new Queue<i2>();
    PriorityQueue<PathNode, float> open_set = new PriorityQueue<PathNode, float>();

    public static bool CalcPathGoal(cell start, cell goal, float maxSqrDist)
    {
        return _inst.calcPathGoal(start, goal, maxSqrDist);
    }

    bool calcPathGoal(cell start, cell goal, float maxSqrDist)
    {
        usedNode.Clear();
        open_set.Clear();

        PathNode startNode = start.pNode;
        startNode.from = null;
        startNode.givenCost = 0; startNode.finalCost = 0;
        startNode.isOpen = true;

        usedNode.Enqueue(startNode.cell.pt);
        open_set.push(startNode, 0);

        while (!open_set.isEmpty())
        {
            PathNode current = open_set.pop();
            PathNode parent = current.from;
            cell cell = current.cell; // get jump point info

            if (current.finalCost > maxSqrDist)
                return false;

            if (cell.pt == goal.pt) // goal
                return current.from != null;


            foreach (eDirections dir in getAllValidDirections(current))
            {// foreach direction from parent
                PathNode new_successor = null;
                int given_cost = 0;

                // goal is closer than wall distance or closer than or equal to jump point distnace
                if (isCardinal(dir) &&
                     goalIsInExactDirection(cell.pt, dir, goal.pt) &&
                     Pt.diff(cell.pt, goal.pt) <= Mathf.Abs(cell.JpDist((int)dir)))
                {
                    new_successor = cells.Get(goal.pt.x, goal.pt.z).pNode;
                    given_cost = current.givenCost + heruis(cell, goal);
                }
                // Goal is closer or equal in either row or column than wall or jump point distance
                else if (isDiagonal(dir) &&
                          goalIsInGeneralDirection(cell.pt, dir, goal.pt) &&
                          (Mathf.Abs(goal.pt.x - cell.pt.x) <= Mathf.Abs(cell.JpDist((int)dir)) ||
                            Mathf.Abs(goal.pt.z - cell.pt.z) <= Mathf.Abs(cell.JpDist((int)dir))))
                {
                    int min_diff = Mathf.Min(Mathf.Abs(goal.pt.x - cell.pt.x), Mathf.Abs(goal.pt.z - cell.pt.z));
                    new_successor = getNodeDist(cell.pt.x, cell.pt.z, dir, min_diff);
                    given_cost = current.givenCost + heruis(cell, new_successor.cell); 
                }
                else if (cell.JpDist((int)dir) > 0)
                {
                    new_successor = getNodeDist(cell.pt.x, cell.pt.z, dir, cell.JpDist((int)dir));
                    given_cost = heruis(cell, new_successor.cell);
                    given_cost += current.givenCost;
                }

                if (new_successor != null &&
                    (!new_successor.isOpen || given_cost < new_successor.givenCost)) // Traditional A* from this point
                {
                    new_successor.from = current;
                    new_successor.givenCost = given_cost;
                    new_successor.dirFromParent = dir;
                    new_successor.finalCost = given_cost + heruis(new_successor.cell, goal);
                    
                     //   octileHeuristic(new_successor.cell.pt.x, new_successor.cell.pt.z, goal.pt.x, goal.pt.z);
                    new_successor.isOpen = true;
                    open_set.push(new_successor, new_successor.finalCost);
                    usedNode.Enqueue(new_successor.cell.pt);
                }
            }
        }
        return false;
    }
    
    public static void clearQuq()
    {
        int num = _inst.usedNode.Count;
        for (int i = 0; i < num; ++i)
        {
            i2 pt = _inst.usedNode.Dequeue();
            cells.Get(pt.x, pt.z).pNode.Reset();
        }
    }


    internal static int heruis(cell c0, cell c1)
    {
        int x = c0.pt.x - c1.pt.x;
        int z = c0.pt.z - c1.pt.z;
        return x * x + z * z;
    }


    public void InitcalcPathGoalForLong(int countForLimit_)
    {
        countForLimit = countForLimit_;
        _calcCout = 0;
        _calcNewCostCout = 0;
        _maxSqrDist = 0;
    }

    int countForLimit = 128;
    float _maxSqrDist = 0;
    int _calcCout = 0;
    int _calcNewCostCout = 0;
    public enum autoCalcPathType
    {
        Found,
        EndWithSave,
        RetryWithSave,
        Retry,
    }
    public autoCalcPathType calcPathGoalAuto(cell start, cell goal, float minsqrDist, float minisqrSingleDist)
    {
        if(_calcCout > 1000000000)
            _calcCout = 0;
        if (_calcNewCostCout > 1000000000)
            _calcNewCostCout = 0;
        usedNode.Clear();
        open_set.Clear();

        PathNode startNode = start.pNode;
        startNode.from = null;
        startNode.givenCost = 0; startNode.finalCost = 0;
        startNode.isOpen = true;

        usedNode.Enqueue(startNode.cell.pt);
        open_set.push(startNode, 0);

        while (!open_set.isEmpty())
        {
            PathNode current = open_set.pop();
            PathNode parent = current.from;
            cell cell = current.cell; // get jump point info

            if (cell.pt == goal.pt) // goal
            {
                if (current.finalCost > minsqrDist)
                {
                    Debug.Log("[Final dist]: " + current.finalCost);
                    return current.from != null ? autoCalcPathType.Found : autoCalcPathType.Retry;
                }

                if (_maxSqrDist < current.finalCost)
                {
                    _maxSqrDist = current.finalCost;
                    return autoCalcPathType.RetryWithSave;
                }

                if (++_calcCout > countForLimit)
                {
                    Debug.Log("[Final dist]: " + _maxSqrDist);
                    return autoCalcPathType.EndWithSave;
                }
                return autoCalcPathType.Retry;
            }

            foreach (eDirections dir in getAllValidDirections(current))
            {// foreach direction from parent
                PathNode new_successor = null;
                int given_cost = 0;

                // goal is closer than wall distance or closer than or equal to jump point distnace
                if (isCardinal(dir) &&
                     goalIsInExactDirection(cell.pt, dir, goal.pt) &&
                     Pt.diff(cell.pt, goal.pt) <= Mathf.Abs(cell.JpDist((int)dir)))
                {
                    new_successor = cells.Get(goal.pt.x, goal.pt.z).pNode;
                    given_cost = current.givenCost + heruis(cell, goal);
                }
                // Goal is closer or equal in either row or column than wall or jump point distance
                else if (isDiagonal(dir) &&
                          goalIsInGeneralDirection(cell.pt, dir, goal.pt) &&
                          (Mathf.Abs(goal.pt.x - cell.pt.x) <= Mathf.Abs(cell.JpDist((int)dir)) ||
                            Mathf.Abs(goal.pt.z - cell.pt.z) <= Mathf.Abs(cell.JpDist((int)dir))))
                {
                    int min_diff = Mathf.Min(Mathf.Abs(goal.pt.x - cell.pt.x), Mathf.Abs(goal.pt.z - cell.pt.z));
                    new_successor = getNodeDist(cell.pt.x, cell.pt.z, dir, min_diff);
                    given_cost = current.givenCost + heruis(cell, new_successor.cell);
                }
                else if (cell.JpDist((int)dir) > 0)
                {
                    new_successor = getNodeDist(cell.pt.x, cell.pt.z, dir, cell.JpDist((int)dir));
                    given_cost = heruis(cell, new_successor.cell);
                    given_cost += current.givenCost;
                }

                if (new_successor != null &&
                    (!new_successor.isOpen || given_cost < new_successor.givenCost)) // Traditional A* from this point
                {
                    new_successor.from = current;
                    new_successor.givenCost = given_cost;
                    new_successor.dirFromParent = dir;
                    new_successor.finalCost = given_cost + heruis(new_successor.cell, goal);

                    //   octileHeuristic(new_successor.cell.pt.x, new_successor.cell.pt.z, goal.pt.x, goal.pt.z);
                    new_successor.isOpen = true;
                    open_set.push(new_successor, new_successor.finalCost);
                    usedNode.Enqueue(new_successor.cell.pt);

                    float newCost = heruis(new_successor.cell, current.cell);
                    if (newCost > minisqrSingleDist)
                    {
                        if (++_calcNewCostCout % 100 == 0)
                            Debug.Log("newCost");
                        return autoCalcPathType.Retry;
                    }
                }
            }
        }
        return autoCalcPathType.Retry;
    }
    
}
