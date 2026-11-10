using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CNormalEnemyTemplate : CEnemyTemplate
    {
        public CNormalEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance)
            : base(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance) { }
    }
}
