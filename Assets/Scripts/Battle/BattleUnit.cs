using UnityEngine;

public enum Side
{
    Player,
    Enemy
}

public class BattleUnit
{
    public const int MaxEnegy = 3;

    public SpeciesData Species { get; }
    public Side Side { get; }
    public Vector2Int Cell { get; set; }
    public int Courage { get; private set; }
    public int Energy { get; private set; } = 1;

    public bool IsOut => Courage <= 0;

    public BattleUnit(SpeciesData species, Side side, Vector2Int cell)
    {
        Species = species;
        Side = side;
        Cell = cell;
        Courage = species.Courage;
    }

    public void TakeDamage(int damage)
    {
        Courage -= damage;
        if (Courage < 0) Courage = 0;
    }
}