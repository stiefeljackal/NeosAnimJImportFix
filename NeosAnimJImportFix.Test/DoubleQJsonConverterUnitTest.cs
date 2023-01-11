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
    public class DoubleQJsonConverterUnitTest
    {
        private static readonly string[] FLOATQ_PROP_NAMES = new string[] { "x", "y", "z", "w" };


        [Theory(DisplayName = "Should read the JSON of BaseX.doubleQ as an object type.")]
        [MemberData(nameof(GetData), parameters: 9)]
        public void ReadJsonValueOfDoubleQAsObject(string json, doubleQ expectedDoubleQ)
        {
            var converter = new DoubleQJsonConverter();
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            reader.Read();

            Assert.Equal(expectedDoubleQ, converter.Read(ref reader, typeof(doubleQ), null));
        }

        public static string CreateDoubleQValueJson(double?[] rgbaArr)
        {
            var sb = new StringBuilder("{");

            for (var i = 0; i < rgbaArr.Length; i++)
            {
                var floatNullable = rgbaArr[i];
                if (!floatNullable.HasValue) { continue;  }
                sb.Append($"\"{FLOATQ_PROP_NAMES[i]}\": ");
                sb.Append(floatNullable.Value);
                if (i < rgbaArr.Length - 1) { sb.Append(','); }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        public static IEnumerable<object[]> GetData(int numTests)
        {
            var floatData = new List<double?[]>()
            {
                new double?[] { 1, 2, 3, 4 },
                new double?[] { 1 },
                new double?[] { 1, 2 },
                new double?[] { 1, 2, 3 },
                new double?[] { null, 1 },
                new double?[] { null, null, 1 },
                new double?[] { null, null, null, 1 },
                new double?[] { 1, null, 2 },
                new double?[] { 1, null, null, 2 }
            }.Select(data => new object[] { CreateDoubleQValueJson(data), data.ToDoubleQValue() });

            return floatData.Take(numTests);
        }
    }
}
