using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JworkzNeosMod.Extensions
{
    internal static class GuidExtensions
    {
        public static Guid CreateWeakFileGuid(this byte[] bytes)
        {
            return bytes?.Length > 0 ? new Guid(new byte[] {
                bytes[0],
                bytes[1 % bytes.Length],
                bytes[2 % bytes.Length],
                bytes[3 % bytes.Length],
                bytes[4 % bytes.Length],
                bytes[5 % bytes.Length],
                bytes[6 % bytes.Length],
                bytes[7 % bytes.Length],
                bytes[Math.Max((bytes.Length - 1) % bytes.Length, 0)],
                bytes[Math.Max((bytes.Length - 2) % bytes.Length, 0)],
                bytes[Math.Max((bytes.Length - 3) % bytes.Length, 0)],
                bytes[Math.Max((bytes.Length - 4) % bytes.Length, 0)],
                bytes[Math.Max((bytes.Length - 5) % bytes.Length, 0)],
                bytes[Math.Max((bytes.Length - 6) % bytes.Length, 0)],
                bytes[Math.Max((bytes.Length - 7) % bytes.Length, 0)],
                bytes[Math.Max((bytes.Length - 8) % bytes.Length, 0)],
            }) : Guid.Empty;
        }
    }
}
