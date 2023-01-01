using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BaseX;
using CodeX;
using FrooxEngine;
using HarmonyLib;
using JworkzNeosMod.Extensions;
using NeosModLoader;

namespace JworkzNeosMod.JsonConverters
{
    internal class ModdedAnimationTrackConverter : JsonConverter<CodeX.AnimationTrack>
    {
        private static readonly JsonSerializerOptions TRACK_SERIALIZER_OPTS;
        static ModdedAnimationTrackConverter()
        {
            TRACK_SERIALIZER_OPTS = new JsonSerializerOptions();

            var converters = TRACK_SERIALIZER_OPTS.Converters;
            converters.Add(new ColorJsonConverter());
            converters.Add(new Color32JsonConverter());

        }

        public override bool CanConvert(Type typeToConvert) => typeof(CodeX.AnimationTrack).IsAssignableFrom(typeToConvert);
        public override CodeX.AnimationTrack Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!reader.CurrentReadHasTokenType(JsonTokenType.StartObject) ||
                !reader.NextReadHasStringValue(JsonTokenType.PropertyName, "trackType") ||
                !reader.NextReadHasTokenType(JsonTokenType.String))
            {
                throw new JsonException();
            }

            var trackType = (TrackType)Enum.Parse(typeof(TrackType), reader.GetString());

            if (!reader.NextReadHasStringValue(JsonTokenType.PropertyName, "valueType") ||
                !reader.NextReadHasTokenType(JsonTokenType.String))
            {
                throw new JsonException();
            }
            Type valueType = AnimX.ElementNameToType(reader.GetString());
            Type returnType;

            switch (trackType)
            {
                case TrackType.Raw:
                    returnType = typeof(CodeX.RawAnimationTrack<>).MakeGenericType(valueType);
                    break;
                case TrackType.Discrete:
                    returnType = typeof(CodeX.DiscreteAnimationTrack<>).MakeGenericType(valueType);
                    break;
                case TrackType.Curve:
                    returnType = typeof(CodeX.CurveAnimationTrack<>).MakeGenericType(valueType);
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (!reader.NextReadHasStringValue(JsonTokenType.PropertyName, "data") ||
                !reader.NextReadHasTokenType(JsonTokenType.StartObject))
            {
                throw new JsonException();
            }

            var result = (CodeX.AnimationTrack)JsonSerializer.Deserialize(ref reader, returnType, TRACK_SERIALIZER_OPTS);
            if (!reader.NextReadHasTokenType(JsonTokenType.EndObject))
            {
                throw new JsonException();
            }
            return result;
        }

        public override void Write(Utf8JsonWriter writer, CodeX.AnimationTrack value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("trackType", value.TrackType.ToString());
            writer.WriteString("valueType", value.ValueType.GetNiceName());
            writer.WritePropertyName("data");
            JsonSerializer.Serialize(writer, value, value.GetType());
            writer.WriteEndObject();
        }
    }
}
