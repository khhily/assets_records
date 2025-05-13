using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssetsRecords.Service.Converters;

public class DateTimeNullableConverter : JsonConverter<DateTime?>
{
    private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        string? dateString = reader.GetString();
        return dateString != null 
            ? DateTime.Parse(dateString) 
            : null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Value.ToString(DateTimeFormat));
        }
    }
}