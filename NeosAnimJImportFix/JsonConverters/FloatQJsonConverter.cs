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
    internal class FloatQJsonConverter : JsonConverter<floatQ>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(floatQ) == typeToConvert;

        public override floatQ Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            floatQ floatQValue;

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    floatQValue = (floatQ)JsonSerializer.Deserialize<float4>(ref reader, options);
                    break;
                default:
                    throw new JsonException();

            }

            return floatQValue;
        }

        public override void Write(Utf8JsonWriter writer, floatQ value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (float4)value, options);
        }
    }
}
