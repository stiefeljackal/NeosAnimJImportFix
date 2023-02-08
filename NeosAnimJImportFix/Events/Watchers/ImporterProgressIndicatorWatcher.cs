using BaseX;
using FrooxEngine;
using JworkzNeosMod.Events;
using JworkzNeosMod.Events.Publishers;
using JworkzNeosMod.Events.Watchers;
using JworkzNeosMod.Models;
using NeosModLoader;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static NeosAssets.Common;

namespace JworkzNeosMod.Watchers
{
    internal static class ImporterProgressIndicatorWatcher
    {
        internal static ConcurrentDictionary<FileId, RefID> IdToIndicatorDictionary = new ConcurrentDictionary<FileId, RefID>();

        internal static ConcurrentDictionary<RefID, FileId> IndicatorToIdDictionary = new ConcurrentDictionary<RefID, FileId>();

        public static void Init()
        {
            Utf8ImporterEventPublisher.OnImportStart += SpawnIndicator;
            Utf8ImporterEventPublisher.OnImportProgress += UpdateIndicator;
            Utf8ImporterEventPublisher.OnImportFinish += CompleteIndicator;
            Utf8ImporterEventPublisher.OnImportFail += FailIndicator;
        }

        public static bool IsWatchingFile(FileId fileId) => IdToIndicatorDictionary.ContainsKey(fileId);

        private static void SpawnIndicator(object _, Utf8ImportStartEventArgs args)
        {
            var world = args.World;
            var user = args.User;
            var id = args.Id;
            var fileTypeName = args.FileTypeName;

            Utf8JsonFileProgressWatcher.StartCache(id);

            world.RunSynchronously(() =>
            {
                if (!Utf8JsonFileProgressWatcher.IsWatchingFile(id)) { return; }

                var indicatorSlot = user.LocalUserSpace.AddSlot($"Import {fileTypeName} Indicator");
                indicatorSlot.PersistentSelf = false;
                NeosLogoMenuProgress indicator = indicatorSlot.AttachComponent<NeosLogoMenuProgress>();
                indicatorSlot.AttachComponent<DestroyOnUserLeave>().TargetUser.Target = world.LocalUser;

                var localUserGlobalPos = user.LocalUserSpace.GlobalPosition;
                user.GetPointInFrontOfUser(out float3 spawnPoint, out floatQ rotation);

                indicator.Spawn(spawnPoint, 0.05f, true);
                indicator.UpdateProgress(0.0f, $"Importing {fileTypeName}", string.Empty);

                IdToIndicatorDictionary.TryAdd(id, indicator.ReferenceID);
                IndicatorToIdDictionary.TryAdd(indicator.ReferenceID, id);

                indicatorSlot.OnPrepareDestroy += OnSlotPrepareDestroy;
            }, true);
        }

        private static void UpdateIndicator(object _, Utf8ImportProgressEventArgs args)
        {
            var indicator = GetProgressIndicator(args.Id, args.World);
            indicator?.UpdateProgress(args.Percent, $"Importing {args.FileTypeName}", $"{args.ReadSize} / {args.ByteSize} ({args.Percent}%)");
        }

        private static void CompleteIndicator(object _, Utf8ImportFinishEventArgs args)
        {
            ResolveIndicator(args.Id, args.World, args.FileTypeName);
        }

        private static void FailIndicator(object _, Utf8ImportFailEventArgs args)
        {
            ResolveIndicator(args.Id, args.World, args.FileTypeName, true, args.ErrorMessage);
        }

        private static void ResolveIndicator(FileId fileId, World world, string fileTypeName, bool isError = false, string message = "")
        {
            var indicator = GetProgressIndicator(fileId, world);
            Utf8JsonFileProgressWatcher.StopCache(fileId);

            if (indicator == null) { return; }

            if (!isError)
            {
                indicator.ProgressDone($"{fileTypeName} has been imported!");
            }
            else
            {
                indicator.ProgressFail($"Failed to import {fileTypeName}: {message}");
            }

            RemoveIndicatorCache(indicator.ReferenceID);
        }

        private static NeosLogoMenuProgress GetProgressIndicator(FileId id, World world)
        {
            var hasRefId = IdToIndicatorDictionary.TryGetValue(id, out var refId);
            return hasRefId ? world.ReferenceController.GetObjectOrNull(refId) as NeosLogoMenuProgress : null;
        }

        private static void OnSlotPrepareDestroy(Slot slot)
        {
            var indicator = slot.GetComponent<NeosLogoMenuProgress>();
            RemoveIndicatorCache(indicator.ReferenceID);
        }

        private static void RemoveIndicatorCache(RefID refId)
        {
            IndicatorToIdDictionary.TryRemove(refId, out FileId id);
            IdToIndicatorDictionary.TryRemove(id, out RefID _);
        }
    }
}
