using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnemyEditor
{
    public class EnemyTemplateConverter : JsonConverter<CEnemyTemplate>
    {
        public override CEnemyTemplate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                // Ищем поле $type. Добавили знак ? к строке, так как GetString() может вернуть null
                string? type = root.TryGetProperty("$type", out var typeProp)
                    ? typeProp.GetString()
                    : "CNormalEnemyTemplate";

                // Если type всё еще null (теоретически), ставим по умолчанию
                type ??= "CNormalEnemyTemplate";

                string json = root.GetRawText();

                // Добавили знак ! после Deserialize, чтобы убрать предупреждение CS8603
                switch (type)
                {
                    case "CNormalEnemyTemplate":
                        return JsonSerializer.Deserialize<CNormalEnemyTemplate>(json, options)!;
                    case "CArmoredEnemyTemplate":
                        return JsonSerializer.Deserialize<CArmoredEnemyTemplate>(json, options)!;
                    case "CHealingEnemyTemplate":
                        return JsonSerializer.Deserialize<CHealingEnemyTemplate>(json, options)!;
                    default:
                        throw new NotSupportedException($"Неизвестный тип: {type}");
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, CEnemyTemplate value, JsonSerializerOptions options)
        {
            string type = value.GetType().Name;

            // Сериализуем объект в строку
            string json = JsonSerializer.Serialize(value, value.GetType(), options);

            // Парсим строку, чтобы добавить поле $type
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                writer.WriteStartObject();
                writer.WriteString("$type", type);

                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    prop.WriteTo(writer);
                }
                writer.WriteEndObject();
            }
        }
    }
}