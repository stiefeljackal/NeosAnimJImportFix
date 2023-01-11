using BaseX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JworkzNeosMod.Extensions
{
    internal static class BaseXStructExtensions
    {
        public const float DEFAULT_R = 0f;
        public const float DEFAULT_G = 0f;
        public const float DEFAULT_B = 0f;
        public const float DEFAULT_A = 1f;

        public static readonly IEnumerable<float> DEFAULT_RGBA_ARGS =
            new float[] { DEFAULT_R, DEFAULT_G, DEFAULT_B, DEFAULT_A }.AsEnumerable();

        public static readonly IEnumerable<float> DEFAULT_FLOAT_ARGS =
            new float[] { 0f, 0f, 0f, 0f };

        public static readonly IEnumerable<double> DEFAULT_DOUBLE_ARGS =
            new double[] { 0d, 0d, 0d, 0d };

        public static color ToColorValue(this IEnumerable<float> values) =>
            values.Cast<float?>().ToColorValue();

        public static color ToColorValue(this IEnumerable<float?> values) =>
            new color(values.ToFloat4Value(DEFAULT_RGBA_ARGS.Cast<float?>()));

        public static floatQ ToFloatQValue(this float4 value) =>
            new floatQ(value.x, value.y, value.z, value.w);

        public static floatQ ToFloatQValue(this IEnumerable<float?> values) =>
            values.ToFloat4Value(DEFAULT_FLOAT_ARGS.Cast<float?>()).ToFloatQValue();

        public static float4 ToFloat4Value(this IEnumerable<float?> values, IEnumerable<float?> defaultValues)
        {
            if (defaultValues == null) { defaultValues = DEFAULT_FLOAT_ARGS.Cast<float?>(); }

            var resultArgs = GetConstructorArgs(values, defaultValues).ToArray();

            var x = resultArgs[0].Value;
            var y = resultArgs[1].Value;
            var z = resultArgs[2].Value;
            var w = resultArgs[3].Value;

            return new float4(x, y, z, w);
        }

        public static doubleQ ToDoubleQValue(this double4 value) =>
            new doubleQ(value.x, value.y, value.z, value.w);

        public static doubleQ ToDoubleQValue(this IEnumerable<double?> values) =>
            values.ToDouble4Value(DEFAULT_DOUBLE_ARGS.Cast<double?>()).ToDoubleQValue();

        public static double4 ToDouble4Value(this IEnumerable<double?> values, IEnumerable<double?> defaultValues)
        {
            if (defaultValues == null) { defaultValues = DEFAULT_DOUBLE_ARGS.Cast<double?>(); }

            var resultArgs = GetConstructorArgs(values, defaultValues).ToArray();

            var x = resultArgs[0].Value;
            var y = resultArgs[1].Value;
            var z = resultArgs[2].Value;
            var w = resultArgs[3].Value;

            return new double4(x, y, z, w);
        }

        private static IEnumerable<T?> GetConstructorArgs<T>(this IEnumerable<T?> values, IEnumerable<T?> defaultValues) where T : struct
        {
            var valuesArr = values.ToArray();
            var defaultArgs = defaultValues.ToArray();

            for (var i = 0; i < defaultArgs.Length; i++)
            {
                var value = i < valuesArr.Length ? valuesArr[i] : null;
                yield return value.HasValue ? value.Value : defaultArgs[i].Value;
            }
        }
    }
}
