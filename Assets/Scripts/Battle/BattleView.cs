using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleView : MonoBehaviour
{
    [SerializeField] StageData stage;
    [SerializeField] List<UnitSlot> playerTeam;

    [SerializeField] SpriteRenderer cellPrefab;
    [SerializeField] UnitView unitPrefab;
    [SerializeField] float cellSize = 1.5f;
    [SerializeField] float gap = 1f;   // khoảng cách giữa hai bàn
    [SerializeField] Button startButton;

    // game started => IsArranging = false => không cho kéo bọ nữa
    public bool IsArranging { get; private set; } = true;

    BattleGrid grid;
    readonly List<UnitView> views = new();

    void Start()
    {
        grid = new BattleGrid(stage.Columns, stage.Lanes);
        SpawnCells(Side.Player);
        SpawnCells(Side.Enemy);
        foreach (var s in playerTeam) SpawnUnit(s, Side.Player);
        foreach (var s in stage.Enemies) SpawnUnit(s, Side.Enemy);
        startButton.onClick.AddListener(BeginBattle);
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

    void SpawnUnit(UnitSlot slot, Side side)
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
            grid.MoveOrSwap(unit, cell);
        foreach (var v in views) v.SnapToCell();
    }

    void BeginBattle()
    {
        IsArranging = false;
        startButton.gameObject.SetActive(false);
        StartCoroutine(RunBattle());
    }

    IEnumerator RunBattle()
    {
        var battle = new BattleManager(grid);
        while (!battle.IsOver)
        {
            var actor = battle.NextActor();
            var result = battle.BasicAttack(actor);   // logic tính xong ngay
            if (result == null) continue;

            Debug.Log($"Vòng {battle.Round}: [{actor.Side}] {actor.Species.DisplayName} đánh " +
                      $"{result.Target.Species.DisplayName} -{result.Damage}, còn {result.Target.Courage}");

            yield return StartCoroutine(ViewOf(actor).PlayAttack(ViewOf(result.Target)));   // rồi mới diễn
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log($"Kết thúc sau {battle.Round} vòng: {battle.Winner} thắng");
    }

    UnitView ViewOf(BattleUnit unit) => views.Find(v => v.Unit == unit);


}
