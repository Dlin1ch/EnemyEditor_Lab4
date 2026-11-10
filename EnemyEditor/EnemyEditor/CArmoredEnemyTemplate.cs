using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CArmoredEnemyTemplate : CEnemyTemplate
    {
        [JsonInclude]
        public double Armor { get; set; }

        public CArmoredEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance, double armor)
            : base(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance)
        {
            Armor = armor;
        }
    }
}
