using Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CArmoredEnemy : CEnemy
    {
        public double Armor { get; private set; }

        public CArmoredEnemy(string name, string iconName, BigNumber baseLife, BigNumber gold, double armor)
    : base(name, iconName, baseLife, gold)
        {
            Armor = armor;
        }

        public override void TakeDamage(BigNumber damage)
        {
            double damageReduction = 1.0 - (Armor / 100.0);
            if (damageReduction < 0.1) damageReduction = 0.1;

            BigNumber reducedDamage = damage * damageReduction;
            CurrentLife = CurrentLife - reducedDamage;
        }
    }
}
