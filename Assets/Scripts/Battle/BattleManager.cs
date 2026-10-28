using System.Collections.Generic;
using System.Linq;

public class AttackResult
{
    public BattleUnit Attacker;
    public BattleUnit Target;
    public int Damage;
}

public class BattleManager
{
    readonly BattleGrid grid;
    readonly Queue<BattleUnit> turnQueue = new();

    public int Round { get; private set; }

    public BattleManager(BattleGrid grid) => this.grid = grid;

    public bool IsOver => !grid.Alive(Side.Player).Any() || !grid.Alive(Side.Enemy).Any();
    public Side Winner => grid.Alive(Side.Player).Any() ? Side.Player : Side.Enemy;

    // Con đi lượt tiếp theo; hết hàng đợi thì sang vòng mới. Chỉ gọi khi !IsOver
    public BattleUnit NextActor()
    {
        while (true)
        {
            if (turnQueue.Count == 0) StartRound();
            var unit = turnQueue.Dequeue();
            if (!unit.IsOut) return unit;   // con đã bỏ chạy giữa vòng thì bỏ qua
        }
    }

    void StartRound()
    {
        Round++;
        foreach (var u in TurnOrder.Build(grid.Units)) turnQueue.Enqueue(u);
    }

    // Đòn thường: cận chiến, hệ số 1.0
    public AttackResult BasicAttack(BattleUnit attacker)
    {
        var target = grid.FindMeleeTarget(attacker);
        if (target == null) return null;

        int damage = DamageCalculator.CalculateDamage(attacker.Species.Attack, 1f, target.Species.Shell);
        target.TakeDamage(damage);
        return new AttackResult { Attacker = attacker, Target = target, Damage = damage };
    }
}
