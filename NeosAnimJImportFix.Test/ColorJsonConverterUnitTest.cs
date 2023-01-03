using FrooxEngine;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Xunit;
using BepuPhysics;
using HarmonyLib;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseX;
using System.Text;
using JworkzNeosMod.JsonConverters;
using JworkzNeosMod.Extensions;

namespace JworkzNeosMod.Test
{
    interface ITest
    {
        ISyncBoolMock EnabledField { get; set; }
    }
    interface ISyncBoolMock
    {
        bool IsDriven { get; set;}
    }
    public class ColorJsonConverterUnitTest
    {
        private static readonly string[] COLOR_PROP_NAMES = new string[] { "r", "g", "b", "a" };


        [Theory(DisplayName = "Should read the JSON of BaseX.color as an object type.")]
        [MemberData(nameof(GetData), parameters: 9)]
        public void ReadJsonValueOfColorAsObject(string json, color expectedColor)
        {
            var converter = new ColorJsonConverter();
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            reader.Read();

            Assert.Equal(expectedColor, converter.Read(ref reader, typeof(color), null));
        }

        public static string CreateColorValueJson(float?[] rgbaArr)
        {
            var sb = new StringBuilder("{");

            for (var i = 0; i < rgbaArr.Length; i++)
            {
                var floatNullable = rgbaArr[i];
                if (!floatNullable.HasValue) { continue;  }
                sb.Append($"\"{COLOR_PROP_NAMES[i]}\": ");
                sb.Append(floatNullable.Value);
                if (i < rgbaArr.Length - 1) { sb.Append(','); }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        public static IEnumerable<object[]> GetData(int numTests)
        {
            var floatData = new List<float?[]>()
            {
                new float?[] { 1, 2, 3, 4 },
                new float?[] { 1 },
                new float?[] { 1, 2 },
                new float?[] { 1, 2, 3 },
                new float?[] { null, 1 },
                new float?[] { null, null, 1 },
                new float?[] { null, null, null, 1 },
                new float?[] { 1, null, 2 },
                new float?[] { 1, null, null, 2 }
            }.Select(data => new object[] { CreateColorValueJson(data), data.ToColorValue() });

            return floatData.Take(numTests);
        }
    }
}
