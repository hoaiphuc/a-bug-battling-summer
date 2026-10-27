using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleGrid
{
    public int Columns { get; }
    public int Lanes { get; }

    readonly List<BattleUnit> units = new();

    public BattleGrid(int columns = 3, int lanes = 3)
    {
        Columns = columns;
        Lanes = lanes;
    }

    public bool IsInside(Vector2Int cell) => cell.x >= 0 && cell.x < Columns && cell.y >= 0 && cell.y < Lanes;

    public BattleUnit GetUnit(Side side, Vector2Int cell) =>
        units.FirstOrDefault(u => u.Side == side && !u.IsOut && u.Cell == cell);

    public bool Place(BattleUnit unit)
    {
        if (!IsInside(unit.Cell) || GetUnit(unit.Side, unit.Cell) != null) return false;
        units.Add(unit);
        return true;
    }


    //Melee
    public BattleUnit FindMeleeTarget(BattleUnit attacker)
    {
        var targetCell = attacker.Cell + Vector2Int.right;
        return units.FirstOrDefault(u => u.Cell == targetCell);
    }
}