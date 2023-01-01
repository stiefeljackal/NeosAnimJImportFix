using BaseX;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NeosModLoader;
using JworkzNeosMod.Extensions;

namespace JworkzNeosMod.JsonConverters
{
    internal class Color32JsonConverter : JsonConverter<color32>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(color32) == typeToConvert;

        public override color32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            byte r = 0;
            byte g = 0;
            byte b = 0;
            var a = byte.MaxValue;

            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString().ToLower();
                if (reader.NextReadHasTokenType(JsonTokenType.Number))
                {
                    var propertyValue = reader.GetByte();
                    switch (propertyName)
                    {
                        case "r":
                            r = propertyValue;
                            break;
                        case "g":
                            g = propertyValue;
                            break;
                        case "b":
                            b = propertyValue;
                            break;
                        case "a":
                            a = propertyValue;
                            break;
                        default:
                            throw new JsonException();
                    }
                }
            }
            if (!reader.CurrentReadHasTokenType(JsonTokenType.EndObject))
            {
                throw new JsonException();
            }

            return new color32(r, g, b, a);
        }

        public override void Write(Utf8JsonWriter writer, color32 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("r", value.r);
            writer.WriteNumber("g", value.g);
            writer.WriteNumber("b", value.b);
            writer.WriteNumber("a", value.a);
            writer.WriteEndObject();
        }
    }
}
