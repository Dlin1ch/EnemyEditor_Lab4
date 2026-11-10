using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EnemyEditor
{
    public abstract class CEnemyTemplate
    {
        [JsonInclude]
        public string Name { get; set; }
        [JsonInclude]
        public string IconName { get; set; }
        [JsonInclude]
        public int BaseLife { get; set; }
        [JsonInclude]
        public double LifeModifier { get; set; }
        [JsonInclude]
        public int BaseGold { get; set; }
        [JsonInclude]
        public double GoldModifier { get; set; }
        [JsonInclude]
        public double SpawnChance { get; set; }

        public CEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier, int baseGold, double goldModifier, double spawnChance)
        {
            Name = name;
            IconName = iconName;
            BaseLife = baseLife;
            LifeModifier = lifeModifier;
            BaseGold = baseGold;
            GoldModifier = goldModifier;
            SpawnChance = spawnChance;
        }
    }
}
