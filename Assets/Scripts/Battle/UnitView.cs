using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

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

    // Chạy tới trước mặt mục tiêu, đánh, rồi chạy về chỗ cũ
    public IEnumerator PlayAttack(UnitView target)
    {
        Vector3 home = transform.position;
        Vector3 dir = (home - target.transform.position).normalized;
        Vector3 hitPos = target.transform.position + dir * 0.8f;   // dừng cách mục tiêu 0.8 đơn vị

        body.sortingOrder = 2;
        yield return StartCoroutine(MoveTo(hitPos, 0.25f));
        target.RefreshState();                                   // kết quả hiện đúng lúc chạm
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(MoveTo(home, 0.2f));
        body.sortingOrder = 1;
    }

    IEnumerator MoveTo(Vector3 to, float duration)
    {
        Vector3 from = transform.position;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from, to, t / duration);
            yield return null;   // chờ sang khung hình sau
        }
        transform.position = to;
    }

    // Tạm thời: bỏ chạy thì ẩn đi. Sau này thay bằng thanh Dũng khí và hoạt ảnh bỏ chạy
    public void RefreshState()
    {
        if (Unit.IsOut) gameObject.SetActive(false);
    }

}
