using Clicker;
using System.IO;
using System.Text.Json;

namespace EnemyEditor
{
    public class JsonPlayerSaver : ISaveList<Player>
    {
        private readonly JsonSerializerOptions _options;

        public JsonPlayerSaver()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }

        public Player Load(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<Player>(json, _options) ?? new Player();
            }
            return new Player();
        }

        public void Save(Player data, string path)
        {
            string json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(path, json);
        }
    }
}