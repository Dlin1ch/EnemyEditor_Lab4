using Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class Enemy
    {
        // ==================== ПОЛЯ ====================

        private string name;                    // Имя противника
        private BigNumber maxHitPoints;         // Максимальное здоровье
        private BigNumber currentHitPoints;     // Текущее здоровье
        private BigNumber goldReward;           // Награда за победу
        private bool isDead;                    // Мёртв ли противник
        private string iconPath;                // Путь к иконке

        // ==================== СВОЙСТВА ====================

        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        public BigNumber MaxHitPoints
        {
            get { return maxHitPoints; }
            private set { maxHitPoints = value; }
        }

        public BigNumber CurrentHitPoints
        {
            get { return currentHitPoints; }
            private set { currentHitPoints = value; }
        }

        public BigNumber GoldReward
        {
            get { return goldReward; }
            private set { goldReward = value; }
        }

        public bool IsDead
        {
            get { return isDead; }
            private set { isDead = value; }
        }

        public string IconPath
        {
            get { return iconPath; }
            private set { iconPath = value; }
        }

        // ==================== КОНСТРУКТОР ====================

        public Enemy(string name, BigNumber maxHitPoints, BigNumber goldReward, string iconPath)
        {
            this.name = name;
            this.maxHitPoints = maxHitPoints;
            this.currentHitPoints = maxHitPoints.Clone();  // Копия максимального здоровья
            this.goldReward = goldReward;
            this.isDead = false;
            this.iconPath = iconPath;
        }

        // ==================== МЕТОДЫ ====================

        // Нанести урон противнику
        // Возвращает true, если противник побеждён
        // В out-параметр записывает награду за победу
        public bool TakeDamage(BigNumber dmg, out BigNumber goldReward)
        {
            goldReward = new BigNumber("0");

            // Если противник уже мёртв — ничего не делаем
            if (isDead)
            {
                return false;
            }

            // Проверяем: если урон >= текущего здоровья — противник умирает
            if (currentHitPoints < dmg || currentHitPoints.CompareTo(dmg) == 0)
            {
                Die();
                goldReward = this.goldReward;
                return true;
            }

            // Иначе просто вычитаем урон
            currentHitPoints = currentHitPoints - dmg;
            return false;
        }

        // Убить противника
        private void Die()
        {
            isDead = true;
            currentHitPoints = new BigNumber("0");
        }
    }
}
