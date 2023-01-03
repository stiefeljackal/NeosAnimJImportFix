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
    internal class ColorJsonConverter : JsonConverter<color>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(color) == typeToConvert;

        public override color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            color color;

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    color = ReadObject(ref reader);
                    break;
                default:
                    throw new JsonException();

            }

            return color;
        }

        private color ReadObject(ref Utf8JsonReader reader)
        {
            var r = BaseXStructExtensions.DEFAULT_R;
            var g = BaseXStructExtensions.DEFAULT_G;
            var b = BaseXStructExtensions.DEFAULT_B;
            var a = BaseXStructExtensions.DEFAULT_A;

            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString().ToLower();
                if (reader.NextReadHasTokenType(JsonTokenType.Number))
                {
                    var propertyValue = reader.GetSingle();
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

            return new color(r, g, b, a);
        }
        
        public override void Write(Utf8JsonWriter writer, color value, JsonSerializerOptions options)
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
