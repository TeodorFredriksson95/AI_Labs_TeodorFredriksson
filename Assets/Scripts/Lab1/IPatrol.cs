
using JetBrains.Annotations;
using static EnemyPatrol;

public interface IPatrol
{
    public void HandleInput(EnemyPatrol guard, GuardState state);

    public void Update(EnemyPatrol guard);
}