using Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace EnemyEditor
{
    // Абстрактный базовый класс, реализующий интерфейс IEnemy
    public abstract class CEnemy : IEnemy
    {
        public string Name { get; private set; }
        public string IconName { get; private set; }
        public BigNumber BaseLife { get; private set; }
        public BigNumber CurrentLife { get; protected set; }
        public BigNumber Gold { get; private set; }

        protected CEnemy(string name, string iconName, BigNumber baseLife, BigNumber gold)
        {
            Name = name;
            IconName = iconName;
            BaseLife = baseLife.Clone();
            CurrentLife = baseLife.Clone();
            Gold = gold;
        }

        public virtual void TakeDamage(BigNumber damage)
        {
            CurrentLife = CurrentLife - damage;
        }

        public bool IsDead()
        {
            return CurrentLife.ToString() == "0" || CurrentLife < new BigNumber("1");
        }
    }
}