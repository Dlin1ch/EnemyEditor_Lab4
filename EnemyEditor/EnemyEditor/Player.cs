using EnemyEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker
{
    public class Player
    {
        // ==================== ПОЛЯ ====================

        private int lvl;                    // Уровень игрока
        private BigNumber gold;             // Текущее золото
        private BigNumber damage;           // Текущий урон
        private double damageModifier;      // Множитель урона
        private BigNumber upgradeCost;      // Стоимость апгрейда
        private double upgradeModifier;     // Множитель стоимости
        private CCountdownTimer clickTimer;
        private double clickCooldown;
        private BigNumber upgradeCooldownCost;
        private double damageBoostTime = 0;
        private double originalDamage = 0;

        // ==================== СВОЙСТВА ====================

        public int Lvl
        {
            get { return lvl; }
            private set { lvl = value; }
        }

        public BigNumber Gold
        {
            get { return gold; }
            private set { gold = value; }
        }

        public BigNumber Damage
        {
            get { return damage; }
            private set { damage = value; }
        }

        public double DamageModifier
        {
            get { return damageModifier; }
            private set { damageModifier = value; }
        }

        public BigNumber UpgradeCost
        {
            get { return upgradeCost; }
            private set { upgradeCost = value; }
        }

        public double UpgradeModifier
        {
            get { return upgradeModifier; }
            private set { upgradeModifier = value; }
        }
        public double ClickCooldown
        {
            get { return clickCooldown; }
            private set { clickCooldown = value; }
        }

        public BigNumber UpgradeCooldownCost
        {
            get { return upgradeCooldownCost; }
            private set { upgradeCooldownCost = value; }
        }

        // ==================== КОНСТРУКТОР ====================

        public Player()
        {
            lvl = 1;
            gold = new BigNumber("0");
            damage = new BigNumber("1");
            damageModifier = 2.0;
            upgradeCost = new BigNumber("100");
            upgradeModifier = 1.2;
            clickCooldown = 1.0; // 1 секунда
            clickTimer = new CCountdownTimer(clickCooldown);
            upgradeCooldownCost = new BigNumber("50");
        }
        // ==================== МЕТОДЫ ====================

        // Добавить золото
        public void AddGold(BigNumber amount)
        {
            gold = gold + amount;
        }

        // Попытаться потратить золото
        private bool TrySpendGold(BigNumber amount)
        {
            if (gold < amount)
            {
                return false;  // Не хватает золота
            }
            gold = gold - amount;
            return true;
        }

        // Попытаться улучшить урон
        public bool TryUpgrade()
        {
            if (!TrySpendGold(upgradeCost))
            {
                return false;  // Не хватает золота
            }

            lvl++;
            RecalculateStats();
            return true;
        }

        // Пересчитать характеристики после апгрейда
        private void RecalculateStats()
        {
            damage = damage * damageModifier;
            upgradeCost = CalculateNextUpgradeCost();
        }

        // Вычислить стоимость следующего апгрейда
        private BigNumber CalculateNextUpgradeCost()
        {
            // upgradeCost = upgradeCost * (upgradeModifier * lvl)
            return upgradeCost * (upgradeModifier * lvl);
        }

        // Вычислить общий урон
        private BigNumber CalculateTotalDamage()
        {
            return damage;
        }

        // Нанести урон (возвращает текущий урон)
        public BigNumber DealDamage()
        {
            return damage;
        }
        // Обновление таймера перезарядки
        public void Update(double delta)
        {
            clickTimer.update(delta);

            // Обработка временного баффа урона
            if (damageBoostTime > 0)
            {
                damageBoostTime -= delta;
                if (damageBoostTime <= 0)
                {
                    damage = new BigNumber(originalDamage.ToString("0")); // Возвращаем урон
                }
            }
        }

        // Можно ли кликать?
        public bool CanClick()
        {
            return clickTimer.isReady();
        }

        // Сбросить перезарядку после клика
        public void ResetClick()
        {
            clickTimer.reset();
        }

        // Улучшить перезарядку за золото
        public bool TryUpgradeCooldown()
        {
            if (!TrySpendGold(upgradeCooldownCost))
            {
                return false;
            }

            clickCooldown *= 0.9; // Уменьшаем на 10%
            if (clickCooldown < 0.2) clickCooldown = 0.2; // Минимум 0.2 сек

            clickTimer = new CCountdownTimer(clickCooldown);
            upgradeCooldownCost = upgradeCooldownCost * 2.0; // Следующее улучшение дороже

            return true;
        }
        // Применить бафф урона
        public void ApplyDamageBoost(double duration)
        {
            if (damageBoostTime <= 0)
            {
                originalDamage = double.Parse(damage.ToString());
                damage = damage * 2.0; // Удваиваем урон
            }
            damageBoostTime = duration;
        }
    }
}
