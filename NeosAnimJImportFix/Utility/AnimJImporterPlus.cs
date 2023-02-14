using BaseX;
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

namespace JworkzNeosMod.Utility
{
    internal static class AnimJImporterPlus
    {
        private static readonly string FILE_TYPE = "AnimJ";

        private const int MIN_BYTES_TO_REPORT = 210000;

        private const int START_AT_SECOND = 7;

        private const int REPORT_INTERNAL_IN_SECONDS = 2;

        public static AnimX ImportFromFile(string file, User allocatingUser)
        {
            return ImportFromJSON(File.ReadAllBytes(file), allocatingUser);
        }

        public static AnimX ImportFromJSON(string json, User allocatingUser)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            return ImportFromJSON(bytes, allocatingUser);

        }

        public static AnimX ImportFromJSON(byte[] bytes, User allocatingUser)
        {
            AnimX animXResult = null;

            var readerId = new FileId(FileId.UTF8_JSON_FILE_TYPE, bytes.Length);
            var fileSize = bytes.Length;
            var timer = fileSize >= MIN_BYTES_TO_REPORT ? new Timer((object _) =>
            {
                var consumedBytes = Utf8JsonFileProgressWatcher.GetConsumedBytesFromId(readerId);
                Utf8ImporterEventPublisher.RaiseOnImportProgressEvent(null, allocatingUser, readerId, FILE_TYPE, fileSize, consumedBytes);
            }, FILE_TYPE, TimeSpan.FromSeconds(START_AT_SECOND), TimeSpan.FromSeconds(REPORT_INTERNAL_IN_SECONDS)) : null;

            try
            {
                var reader = new Utf8JsonReader(bytes);

                Utf8ImporterEventPublisher.RaiseOnImportStartEvent(null, allocatingUser, readerId, FILE_TYPE, fileSize);

                var jsonObj = JsonSerializer.Deserialize<CodeX.Animation>(ref reader, new JsonSerializerOptions
                {
                    Converters = { new ModdedAnimationTrackConverter(), new ColorJsonConverter() },
                });

                Utf8ImporterEventPublisher.RaiseOnImportFinishEvent(null, allocatingUser, readerId, FILE_TYPE, fileSize);

                animXResult = AnimJImporter.CreateFrom(jsonObj);
            }
            catch (Exception e)
            {
                NeosMod.Error(e.Message);
                NeosMod.Error(e.StackTrace);
                Utf8ImporterEventPublisher.RaiseOnImportFailEvent(null, allocatingUser, readerId, FILE_TYPE, fileSize, e.Message);
                throw;
            }
            finally
            {
                timer?.Dispose();
            }

            return animXResult;
        }
    }
}
