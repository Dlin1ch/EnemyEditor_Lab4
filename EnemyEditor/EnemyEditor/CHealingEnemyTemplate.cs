using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CHealingEnemyTemplate : CEnemyTemplate
    {
        [JsonInclude]
        public double HealChance { get; set; }

        public CHealingEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance, double healChance)
            : base(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance)
        {
            HealChance = healChance;
        }
    }
}
