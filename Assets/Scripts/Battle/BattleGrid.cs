using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleGrid
{
    public int Columns { get; }
    public int Lanes { get; }

    readonly List<BattleUnit> units = new();
    public IReadOnlyList<BattleUnit> Units => units;
    public IEnumerable<BattleUnit> Alive(Side side) =>
         units.Where(u => u.Side == side && !u.IsOut);

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

    public bool MoveOrSwap(BattleUnit unit, Vector2Int cell)
    {
        if (!IsInside(cell)) return false;
        var other = GetUnit(unit.Side, cell);
        if (other != null) other.Cell = unit.Cell;
        unit.Cell = cell;
        return true;
    }

    //Melee
    public BattleUnit FindMeleeTarget(BattleUnit attacker)
    {
        Side enemy = attacker.Side == Side.Player ? Side.Enemy : Side.Player;
        int lane = attacker.Cell.y;

        return Alive(enemy)
            .OrderBy(u => Mathf.Abs(u.Cell.y - lane))  // làn gần nhất
            .ThenBy(u => u.Cell.x)                     // cột trước nhất
            .ThenBy(u => u.Cell.y)                     // hai làn cách đều thì lấy làn trên
            .FirstOrDefault();
    }
}