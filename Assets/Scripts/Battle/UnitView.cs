using UnityEngine;
using UnityEngine.EventSystems;

public class UnitView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public BattleUnit Unit { get; private set; }

    BattleView board;
    SpriteRenderer body;

    void Awake() => body = GetComponent<SpriteRenderer>();

    public void Init(BattleUnit unit, BattleView board, float cellSize)
    {
        Unit = unit;
        this.board = board;
        name = $"{unit.Side} {unit.Species.DisplayName}";
        body.color = unit.Species.Color;
        transform.localScale = Vector3.one * cellSize * 0.6f;
        // Tam giác mặc định chĩa lên; xoay cho mũi hướng về phía địch
        transform.rotation = Quaternion.Euler(0, 0, unit.Side == Side.Player ? -90 : 90);
        SnapToCell();
    }

    public void SnapToCell() => transform.position = board.CellToWorld(Unit.Side, Unit.Cell);

    public void OnBeginDrag(PointerEventData e)
    {
        // Không cho kéo bọ bên địch: bỏ pointerDrag thì OnDrag/OnEndDrag không được gọi
        if (Unit.Side != Side.Player)
        {
            e.pointerDrag = null;
            return;
        }
        body.sortingOrder = 2;
    }

    public void OnDrag(PointerEventData e) => transform.position = ToWorld(e);

    public void OnEndDrag(PointerEventData e)
    {
        body.sortingOrder = 1;
        board.OnUnitDropped(this, ToWorld(e));
    }

    static Vector3 ToWorld(PointerEventData e)
    {
        var p = Camera.main.ScreenToWorldPoint(e.position);
        p.z = 0;
        return p;
    }
}
