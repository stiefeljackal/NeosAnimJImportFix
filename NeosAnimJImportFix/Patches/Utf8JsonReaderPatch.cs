using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using FrooxEngine;
using HarmonyLib;
using JworkzNeosMod.Events.Watchers;
using JworkzNeosMod.Models;
using JworkzNeosMod.Utility;
using NeosModLoader;

namespace JworkzNeosMod.Patches
{
    [HarmonyPatch(typeof(Utf8JsonReader), nameof(Utf8JsonReader.Read))]
    internal static class Utf8JsonReaderPatch
    {
        private const int MIN_BYTES_TO_CONSUME = 7000;

        private const int MIN_BYTES_TO_REPORT = 100000;

        private delegate object Utf8JsonReaderAccessor(Utf8JsonReader reader);
        private static readonly Utf8JsonReaderAccessor JsonReaderBufferLengthInfo =
            DelegateCreator.CreateAccessor<Utf8JsonReaderAccessor>("_buffer", "Length");

        static void Postfix(ref Utf8JsonReader __instance)
        {
            if (__instance.BytesConsumed < MIN_BYTES_TO_CONSUME) { return; }

            var bufferLength = (int)JsonReaderBufferLengthInfo(__instance);

            if (bufferLength < MIN_BYTES_TO_REPORT) { return; }

            var fileId = new FileId(FileId.UTF8_JSON_FILE_TYPE, bufferLength);
            if (Utf8JsonFileProgressWatcher.IsWatchingFile(fileId))
            {
                Utf8JsonFileProgressWatcher.UpdateCacheValue(fileId, (int)__instance.BytesConsumed);
            }
        }

    }
}
