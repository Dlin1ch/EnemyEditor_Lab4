using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace EnemyEditor
{
    public class CEnemyTemplateList
    {
        // Приватный список противников
        [JsonInclude]
        private List<CEnemyTemplate> enemies;

        // Конструктор — создаёт пустой список
        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
        }

        // Добавление нового противника
        public void AddEnemy(string name, string iconName, int baseLife,
            double lifeModifier, int baseGold,
            double goldModifier, double spawnChance)
        {
            CEnemyTemplate enemy = new CEnemyTemplate(name, iconName, baseLife,
                lifeModifier, baseGold, goldModifier, spawnChance);
            enemies.Add(enemy);
        }

        // Получить противника по имени
        public CEnemyTemplate? GetEnemyByName(string name)
        {
            foreach (CEnemyTemplate enemy in enemies)
            {
                if (enemy.Name == name)
                {
                    return enemy;
                }
            }
            return null;
        }

        // Получить противника по индексу
        public CEnemyTemplate? GetEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count)
            {
                return enemies[id];
            }
            return null;
        }

        // Удалить противника по имени
        public void DeleteEnemyByName(string name)
        {
            CEnemyTemplate? enemy = GetEnemyByName(name);
            if (enemy != null)
            {
                enemies.Remove(enemy);
            }
        }

        // Удалить противника по индексу
        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count)
            {
                enemies.RemoveAt(id);
            }
        }

        // Получить список имён всех противников
        public List<string> GetListOfEnemyNames()
        {
            List<string> names = new List<string>();
            foreach (CEnemyTemplate enemy in enemies)
            {
                names.Add(enemy.Name);
            }
            return names;
        }

        // Сохранить список в JSON-файл
        public void SaveToJson(string path)
        {
            string jsonString = JsonSerializer.Serialize(enemies);
            File.WriteAllText(path, jsonString);
        }

        // Загрузить список из JSON-файла
        public void LoadFromJson(string path)
        {
            string jsonFromFile = File.ReadAllText(path);

            // Очищаем текущий список
            enemies.Clear();

            // Парсим JSON
            JsonDocument doc = JsonDocument.Parse(jsonFromFile);

            // Перебираем все элементы массива
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                string name = element.GetProperty("Name").GetString() ?? "";
                string iconName = element.GetProperty("IconName").GetString() ?? "";
                int baseLife = element.GetProperty("BaseLife").GetInt32();
                double lifeModifier = element.GetProperty("LifeModifier").GetDouble();
                int baseGold = element.GetProperty("BaseGold").GetInt32();
                double goldModifier = element.GetProperty("GoldModifier").GetDouble();
                double spawnChance = element.GetProperty("SpawnChance").GetDouble();

                // Создаём нового противника через конструктор
                CEnemyTemplate enemy = new CEnemyTemplate(name, iconName, baseLife,
                    lifeModifier, baseGold, goldModifier, spawnChance);
                enemies.Add(enemy);
            }
        }
    }
}