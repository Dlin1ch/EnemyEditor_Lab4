using Clicker;
using System;
using System.Collections.Generic;
using System.IO;

namespace EnemyEditor
{
    public class EnemyStorage
    {
        private List<CEnemyTemplate> templates;
        private Random random;

        public EnemyStorage()
        {
            templates = new List<CEnemyTemplate>();
            random = new Random();
        }

        public void LoadFromJson(string path)
        {
            CEnemyTemplateList list = new CEnemyTemplateList();
            list.LoadFromJson(path);
            templates = new List<CEnemyTemplate>();

            List<string> names = list.GetListOfEnemyNames();
            for (int i = 0; i < names.Count; i++)
            {
                CEnemyTemplate? template = list.GetEnemyByIndex(i);
                if (template != null)
                {
                    templates.Add(template);
                }
            }

            NormalizeChances();
        }

        public void LoadFromDefaultPath()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string jsonPath = Path.Combine(desktopPath, "ClickerData", "enemies.json");

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"Файл не найден: {jsonPath}");
            }

            LoadFromJson(jsonPath);
        }

        private void NormalizeChances()
        {
            if (templates.Count == 0) return;

            double sum = 0;
            for (int i = 0; i < templates.Count; i++)
                sum += templates[i].SpawnChance;

            if (sum == 0)
            {
                double equalChance = 1.0 / templates.Count;
                for (int i = 0; i < templates.Count; i++)
                {
                    // ИСПОЛЬЗУЕМ ОБЫЧНЫЙ ШАБЛОН ВМЕСТО АБСТРАКТНОГО
                    templates[i] = new CNormalEnemyTemplate(
                        templates[i].Name, templates[i].IconName, templates[i].BaseLife,
                        templates[i].LifeModifier, templates[i].BaseGold, templates[i].GoldModifier, equalChance);
                }
                return;
            }

            for (int i = 0; i < templates.Count; i++)
            {
                double normalizedChance = templates[i].SpawnChance / sum;
                templates[i] = new CNormalEnemyTemplate(
                    templates[i].Name, templates[i].IconName, templates[i].BaseLife,
                    templates[i].LifeModifier, templates[i].BaseGold, templates[i].GoldModifier, normalizedChance);
            }
        }

        public CEnemyTemplate? GetRandomTemplate()
        {
            if (templates.Count == 0) return null;

            double chance = random.NextDouble();
            double sum = 0;

            for (int i = 0; i < templates.Count; i++)
            {
                sum += templates[i].SpawnChance;
                if (sum >= chance) return templates[i];
            }

            return templates[templates.Count - 1];
        }

        public Enemy CreateEnemy(CEnemyTemplate template, int playerLevel)
        {
            BigNumber maxHP = new BigNumber(template.BaseLife.ToString());
            maxHP = maxHP * template.LifeModifier;
            maxHP = maxHP * playerLevel;

            BigNumber gold = new BigNumber(template.BaseGold.ToString());
            gold = gold * template.GoldModifier;
            gold = gold * playerLevel;

            return new Enemy(template.Name, maxHP, gold, template.IconName);
        }

        public int Count => templates.Count;
    }
}