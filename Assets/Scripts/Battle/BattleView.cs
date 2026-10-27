using System.Collections.Generic;
using UnityEngine;

public class BattleView : MonoBehaviour
{
    [System.Serializable]
    struct Slot
    {
        public SpeciesData species;
        public Vector2Int cell;
    }

    [SerializeField] SpriteRenderer cellPrefab;
    [SerializeField] UnitView unitPrefab;
    [SerializeField] float cellSize = 1.5f;
    [SerializeField] float gap = 1f;   // khoảng cách giữa hai bàn
    [SerializeField] List<Slot> playerTeam;
    [SerializeField] List<Slot> enemyTeam;

    BattleGrid grid;
    readonly List<UnitView> views = new();

    void Start()
    {
        grid = new BattleGrid();
        SpawnCells(Side.Player);
        SpawnCells(Side.Enemy);
        foreach (var s in playerTeam) SpawnUnit(s, Side.Player);
        foreach (var s in enemyTeam) SpawnUnit(s, Side.Enemy);
    }

    void SpawnCells(Side side)
    {
        for (int x = 0; x < grid.Columns; x++)
            for (int y = 0; y < grid.Lanes; y++)
            {
                var cell = Instantiate(cellPrefab, CellToWorld(side, new Vector2Int(x, y)), Quaternion.identity, transform);
                cell.transform.localScale = Vector3.one * cellSize * 0.95f;
            }
    }

    void SpawnUnit(Slot slot, Side side)
    {
        var unit = new BattleUnit(slot.species, side, slot.cell);
        if (!grid.Place(unit))
        {
            Debug.LogError($"Không đặt được {slot.species.DisplayName} vào ô {slot.cell}");
            return;
        }
        var view = Instantiate(unitPrefab, transform);
        view.Init(unit, this, cellSize);
        views.Add(view);
    }

    // Cột 0 (Trước) của hai bên nằm sát giữa màn hình; làn 0 ở trên cùng
    public Vector3 CellToWorld(Side side, Vector2Int cell)
    {
        float x = gap / 2 + cellSize / 2 + cell.x * cellSize;
        if (side == Side.Player) x = -x;
        float y = ((grid.Lanes - 1) / 2f - cell.y) * cellSize;
        return new Vector3(x, y, 0);
    }

    bool TryWorldToCell(Side side, Vector3 pos, out Vector2Int cell)
    {
        float d = (side == Side.Player ? -pos.x : pos.x) - gap / 2;
        cell = new Vector2Int(
            Mathf.FloorToInt(d / cellSize),
            Mathf.RoundToInt((grid.Lanes - 1) / 2f - pos.y / cellSize));
        return grid.IsInside(cell);
    }

    // Ô trống thì chuyển sang, ô có đồng đội thì đổi chỗ, ngoài bàn thì về chỗ cũ
    public void OnUnitDropped(UnitView view, Vector3 worldPos)
    {
        var unit = view.Unit;
        if (TryWorldToCell(unit.Side, worldPos, out var cell))
        {
            var other = grid.GetUnit(unit.Side, cell);
            if (other != null) other.Cell = unit.Cell;
            unit.Cell = cell;
        }
        foreach (var v in views) v.SnapToCell();
    }
}
