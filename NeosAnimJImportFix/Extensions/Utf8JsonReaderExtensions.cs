using NeosModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JworkzNeosMod.Extensions
{
    internal static class Utf8JsonReaderExtensions
    {
        internal static bool NextReadHasTokenType(this ref Utf8JsonReader reader, JsonTokenType tokenType) =>
            reader.Read() && reader.CurrentReadHasTokenType(tokenType);

        internal static bool CurrentReadHasTokenType(this ref Utf8JsonReader reader, JsonTokenType tokenType) =>
            reader.TokenType == tokenType;

        internal static bool NextReadHasStringValue(this ref Utf8JsonReader reader, JsonTokenType tokenType, string value) =>
            reader.Read() && reader.CurrentReadHasStringValue(tokenType, value);

        internal static bool CurrentReadHasStringValue(this ref Utf8JsonReader reader, JsonTokenType tokenType, string value) =>
            reader.CurrentReadHasTokenType(tokenType) && reader.GetString() == value;
    }
}
