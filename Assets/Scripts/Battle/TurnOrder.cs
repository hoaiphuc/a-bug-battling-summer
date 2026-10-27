using System.Collections.Generic;
using System.Linq;

public static class TurnOrder
{
    public static List<BattleUnit> Build(IEnumerable<BattleUnit> units)
    {
        return units.Where(u => !u.IsOut)
            .OrderByDescending(u => u.Species.Speed)
            .ThenBy(u => u.Side)
            .ToList();
    }
}