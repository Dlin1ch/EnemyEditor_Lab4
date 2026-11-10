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

            for (int i = 0; i < templates.Count; i++)
            {
                // Если сумма шансов 0, ставим равные, иначе делим на сумму
                double normalizedChance = (sum == 0) ? (1.0 / templates.Count) : (templates[i].SpawnChance / sum);

                // ВАЖНО: Копируем объект, сохраняя его тип!
                if (templates[i] is CArmoredEnemyTemplate armored)
                {
                    templates[i] = new CArmoredEnemyTemplate(armored.Name, armored.IconName, armored.BaseLife, armored.LifeModifier, armored.BaseGold, armored.GoldModifier, normalizedChance, armored.Armor);
                }
                else if (templates[i] is CHealingEnemyTemplate healing)
                {
                    templates[i] = new CHealingEnemyTemplate(healing.Name, healing.IconName, healing.BaseLife, healing.LifeModifier, healing.BaseGold, healing.GoldModifier, normalizedChance, healing.HealChance);
                }
                else
                {
                    templates[i] = new CNormalEnemyTemplate(templates[i].Name, templates[i].IconName, templates[i].BaseLife, templates[i].LifeModifier, templates[i].BaseGold, templates[i].GoldModifier, normalizedChance);
                }
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

        public IEnemy CreateEnemy(CEnemyTemplate template, int playerLevel)
        {
            BigNumber maxHP = new BigNumber(template.BaseLife.ToString());
            maxHP = maxHP * template.LifeModifier;
            maxHP = maxHP * playerLevel;

            BigNumber gold = new BigNumber(template.BaseGold.ToString());
            gold = gold * template.GoldModifier;
            gold = gold * playerLevel;

            // Проверяем реальный тип шаблона и создаем нужного врага
            if (template is CArmoredEnemyTemplate armoredTemplate)
            {
                return EnemyFactory.CreateEnemy("CArmoredEnemy", template.Name, template.IconName, maxHP, gold, armoredTemplate.Armor);
            }
            else if (template is CHealingEnemyTemplate)
            {
                return EnemyFactory.CreateEnemy("CNormalEnemy", template.Name, maxHP, gold);
            }
            else
            {
                return EnemyFactory.CreateEnemy("CNormalEnemy", template.Name, maxHP, gold);
            }
        }

        public int Count => templates.Count;
    }
}