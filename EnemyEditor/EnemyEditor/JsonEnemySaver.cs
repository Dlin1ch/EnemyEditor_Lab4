using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace EnemyEditor
{
    public class JsonEnemySaver : ISaveList<List<CEnemyTemplate>>
    {
        private readonly JsonSerializerOptions _options;

        public JsonEnemySaver()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new EnemyTemplateConverter() }
            };
        }

        public List<CEnemyTemplate> Load(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<CEnemyTemplate>>(json, _options) ?? new List<CEnemyTemplate>();
            }
            return new List<CEnemyTemplate>();
        }

        public void Save(List<CEnemyTemplate> data, string path)
        {
            string json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(path, json);
        }
    }
}
