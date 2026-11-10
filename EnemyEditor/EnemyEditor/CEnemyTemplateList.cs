using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace EnemyEditor
{
    public class CEnemyTemplateList
    {
        [System.Text.Json.Serialization.JsonInclude]
        private List<CEnemyTemplate> enemies;

        // Используем интерфейс для сохранения/загрузки
        private readonly ISaveList<List<CEnemyTemplate>> _serializer;

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
            _serializer = new JsonEnemySaver(); // Создаем реализацию интерфейса
        }

        // Добавление уже готового объекта (обычного, бронированного или лечащегося)
        public void AddEnemy(CEnemyTemplate enemy)
        {
            enemies.Add(enemy);
        }

        public CEnemyTemplate? GetEnemyByName(string name)
        {
            return enemies.Find(e => e.Name == name);
        }

        public CEnemyTemplate? GetEnemyByIndex(int id)
        {
            return (id >= 0 && id < enemies.Count) ? enemies[id] : null;
        }

        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count) enemies.RemoveAt(id);
        }

        public List<string> GetListOfEnemyNames()
        {
            return enemies.ConvertAll(e => e.Name);
        }

        public int Count
        {
            get { return enemies.Count; }
        }

        // Сохранение и загрузка через интерфейс (полиморфизм!)
        public void SaveToJson(string path)
        {
            _serializer.Save(enemies, path);
        }

        public void LoadFromJson(string path)
        {
            // 1. Загружаем список из файла
            List<CEnemyTemplate> loadedEnemies = _serializer.Load(path);

            // 2. Добавляем каждого загруженного врага в текущий список
            foreach (CEnemyTemplate enemy in loadedEnemies)
            {
                enemies.Add(enemy);
            }
        }
    }
}