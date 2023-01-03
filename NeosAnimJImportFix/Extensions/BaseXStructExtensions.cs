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

        public static color ToColorValue(this IEnumerable<float> values) =>
            values.Cast<float?>().ToColorValue();
        
        public static color ToColorValue(this IEnumerable<float?> values)
        {
            var valuesArr = values.ToArray();
            var resultArgs = DEFAULT_RGBA_ARGS.ToArray();

            for (var i = 0; i < valuesArr.Length; i++)
            {
                var value = valuesArr[i];
                if (value.HasValue)
                {
                    resultArgs[i] = value.Value;
                }
            }

            var r = resultArgs[0];
            var g = resultArgs[1];
            var b = resultArgs[2];
            var a = resultArgs[3];

            return new color(r, g, b, a);
        }
    }
}
