using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpBB.Server;

public class BackingFieldConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var instance = Activator.CreateInstance<T>();
        var type = typeof(T);

        using var doc = JsonDocument.ParseValue(ref reader);
        foreach (var property in doc.RootElement.EnumerateObject())
        {
            var fieldInfo = type.GetField($"_{property.Name.ToLower()}",
                BindingFlags.NonPublic | BindingFlags.Instance);
                
            if (fieldInfo != null)
            {
                var value = JsonSerializer.Deserialize(property.Value.GetRawText(), fieldInfo.FieldType);
                fieldInfo.SetValue(instance, value);
            }
        }

        return instance;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}