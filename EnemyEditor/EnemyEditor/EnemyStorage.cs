using Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class EnemyStorage
    {
        // Список шаблонов противников (из Лабы 1)
        private List<CEnemyTemplate> templates;

        // Генератор случайных чисел
        private Random random;

        // Конструктор
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

            // Перебираем всех противников и добавляем в наш список
            List<string> names = list.GetListOfEnemyNames();
            for (int i = 0; i < names.Count; i++)
            {
                CEnemyTemplate? template = list.GetEnemyByIndex(i);
                if (template != null)
                {
                    templates.Add(template);
                }
            }

            // Нормализуем шансы
            NormalizeChances();
        }

        // Загрузить шаблоны из стандартной папки ClickerData на рабочем столе
        public void LoadFromDefaultPath()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string jsonPath = System.IO.Path.Combine(desktopPath, "ClickerData", "enemies.json");

            if (!System.IO.File.Exists(jsonPath))
            {
                throw new System.IO.FileNotFoundException($"Файл не найден: {jsonPath}");
            }

            LoadFromJson(jsonPath);
        }

        // Нормализовать шансы появления (сумма = 1)
        private void NormalizeChances()
        {
            if (templates.Count == 0)
                return;

            double sum = 0;
            for (int i = 0; i < templates.Count; i++)
            {
                sum += templates[i].SpawnChance;
            }

            // Если сумма = 0 — устанавливаем равные шансы
            if (sum == 0)
            {
                double equalChance = 1.0 / templates.Count;
                for (int i = 0; i < templates.Count; i++)
                {
                    templates[i] = new CEnemyTemplate(
                        templates[i].Name,
                        templates[i].IconName,
                        templates[i].BaseLife,
                        templates[i].LifeModifier,
                        templates[i].BaseGold,
                        templates[i].GoldModifier,
                        equalChance
                    );
                }
                return;
            }

            // Нормализуем
            for (int i = 0; i < templates.Count; i++)
            {
                double normalizedChance = templates[i].SpawnChance / sum;
                templates[i] = new CEnemyTemplate(
                    templates[i].Name,
                    templates[i].IconName,
                    templates[i].BaseLife,
                    templates[i].LifeModifier,
                    templates[i].BaseGold,
                    templates[i].GoldModifier,
                    normalizedChance
                );
            }
        }

        // Выбрать случайный шаблон по нормализованным шансам
        public CEnemyTemplate? GetRandomTemplate()
        {
            if (templates.Count == 0)
                return null;

            double chance = random.NextDouble();
            double sum = 0;

            for (int i = 0; i < templates.Count; i++)
            {
                sum += templates[i].SpawnChance;
                if (sum >= chance)
                {
                    return templates[i];
                }
            }

            // Если не нашли — возвращаем последний
            return templates[templates.Count - 1];
        }

        // Создать противника на сцене из шаблона с учётом уровня игрока
        public Enemy CreateEnemy(CEnemyTemplate template, int playerLevel)
        {
            // Здоровье = BaseLife * LifeModifier * уровень
            BigNumber maxHP = new BigNumber(template.BaseLife.ToString());
            maxHP = maxHP * template.LifeModifier;
            maxHP = maxHP * playerLevel;

            // Награда = BaseGold * GoldModifier * уровень
            BigNumber gold = new BigNumber(template.BaseGold.ToString());
            gold = gold * template.GoldModifier;
            gold = gold * playerLevel;

            return new Enemy(template.Name, maxHP, gold, template.IconName);
        }

        // Получить список всех шаблонов
        public List<CEnemyTemplate> GetTemplates()
        {
            return templates;
        }

        // Получить количество шаблонов
        public int Count
        {
            get { return templates.Count; }
        }
    }
}
