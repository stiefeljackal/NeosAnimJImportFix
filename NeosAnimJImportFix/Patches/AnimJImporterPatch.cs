﻿using BaseX;
using CodeX;
using FrooxEngine;
using HarmonyLib;
using JworkzNeosMod.Events.Publishers;
using JworkzNeosMod.Events.Watchers;
using JworkzNeosMod.Extensions;
using JworkzNeosMod.JsonConverters;
using JworkzNeosMod.Models;
using NeosModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace JworkzNeosMod.Patches
{
    [HarmonyPatch(typeof(AnimJImporter), nameof(AnimJImporter.ImportFromJSON))]
    internal static class AnimJImporterPatch
    {
        private static readonly string FILE_TYPE = "AnimJ";

        private const int MIN_BYTES_TO_REPORT = 210000;

        private const int START_AT_SECOND = 7;

        private const int REPORT_INTERNAL_IN_SECONDS = 2;

        static bool Prefix(ref AnimX __result, string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            var readerId = new FileId(FileId.UTF8_JSON_FILE_TYPE, bytes.Length);
            var world = Engine.Current.WorldManager.FocusedWorld;
            var fileSize = bytes.Length;
            var timer = fileSize >= MIN_BYTES_TO_REPORT ? new Timer((object _) =>
            {
                NeosMod.Msg("Timer");
                var consumedBytes = Utf8JsonFileProgressWatcher.GetConsumedBytesFromId(readerId);
                Utf8ImporterEventPublisher.RaiseOnImportProgressEvent(null, world, readerId, FILE_TYPE, fileSize, consumedBytes);
            }, FILE_TYPE, TimeSpan.FromSeconds(START_AT_SECOND), TimeSpan.FromSeconds(REPORT_INTERNAL_IN_SECONDS)) : null;

            try
            {
                var reader = new Utf8JsonReader(bytes);

                Utf8ImporterEventPublisher.RaiseOnImportStartEvent(null, world, readerId, FILE_TYPE, fileSize);

                var jsonObj = JsonSerializer.Deserialize<CodeX.Animation>(ref reader, new JsonSerializerOptions
                {
                    Converters = { new ModdedAnimationTrackConverter(), new ColorJsonConverter() },
                });

                Utf8ImporterEventPublisher.RaiseOnImportFinishEvent(null, world, readerId, FILE_TYPE, fileSize);

                __result = AnimJImporter.CreateFrom(jsonObj);
            }
            catch (Exception e)
            {
                __result = null;
                NeosMod.Error(e.Message);
                Utf8ImporterEventPublisher.RaiseOnImportFailEvent(null, world, readerId, FILE_TYPE, fileSize, e.Message);
                throw;
            }
            finally
            {
                timer?.Dispose();
            }

            return false;
        }
    }

    
}
