using Clicker;

namespace EnemyEditor
{
    public interface IEnemy
    {
        string Name { get; }
        string IconName { get; } // <--- Добавили
        BigNumber BaseLife { get; }
        BigNumber CurrentLife { get; }
        BigNumber Gold { get; }
        void TakeDamage(BigNumber damage);
        bool IsDead();
    }
}