namespace  Game.Core.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(DamageData damage);
        bool IsDead { get; }
    }
}
