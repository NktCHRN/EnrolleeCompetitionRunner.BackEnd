using System.Text.Json.Serialization;
using System.Text.Json;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Common.Converters;
public class ShortOrStringPrimitiveConverter : JsonConverter<short>
{
    public override short Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        (reader.TokenType switch
        {
            JsonTokenType.String => HandleString(reader.GetString()),
            JsonTokenType.Number when reader.TryGetInt16(out var i) => i,
            //Add other cases as needed:
            //JsonTokenType.Number when reader.TryGetInt64(out var l) => l,
            //JsonTokenType.Number when reader.TryGetDouble(out var d) => d,
            //JsonTokenType.True => true,
            //JsonTokenType.False => false,
            _ => throw new JsonException(), // StartObject, StartArray, Null    
        })!;

    private static short HandleString(string? value)
    {
        if (short.TryParse(value, out var i))
            return i;

        return 0;
    }

    public override void Write(Utf8JsonWriter writer, short value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value, value.GetType() /*, options */); // Passing options when ObjectPrimitiveConverter has been added to options.Converters will cause a stack overflow
}