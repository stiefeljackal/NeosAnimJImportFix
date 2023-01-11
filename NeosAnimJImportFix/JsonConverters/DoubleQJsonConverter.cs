using BaseX;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NeosModLoader;
using JworkzNeosMod.Extensions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace JworkzNeosMod.JsonConverters
{
    internal class DoubleQJsonConverter : JsonConverter<doubleQ>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(doubleQ) == typeToConvert;

        public override doubleQ Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            doubleQ doubleQValue;

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    doubleQValue = (doubleQ)JsonSerializer.Deserialize<double4>(ref reader, options);
                    break;
                default:
                    throw new JsonException();

            }

            return doubleQValue;
        }

        public override void Write(Utf8JsonWriter writer, doubleQ value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (double4)value, options);
        }
    }
}
