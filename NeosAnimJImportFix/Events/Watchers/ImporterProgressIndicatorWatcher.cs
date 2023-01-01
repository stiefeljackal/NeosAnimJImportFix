using BaseX;
using FrooxEngine;
using JworkzNeosMod.Events;
using JworkzNeosMod.Events.Publishers;
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
            var indicatorSlot = world.LocalUserSpace.AddSlot($"Import {args.FileTypeName} Indicator");
            indicatorSlot.PersistentSelf = false;
            NeosLogoMenuProgress indicator = indicatorSlot.AttachComponent<NeosLogoMenuProgress>();
            indicatorSlot.AttachComponent<DestroyOnUserLeave>().TargetUser.Target = world.LocalUser;

            var localUser = world.LocalUser;
            var localUserGlobalPos = localUser.LocalUserSpace.GlobalPosition;
            localUser.GetPointInFrontOfUser(out float3 spawnPoint, out floatQ rotation);

            indicator.Spawn(spawnPoint, 0.05f, true);
            indicator.UpdateProgress(0.0f, $"Importing {args.FileTypeName}", string.Empty);

            IdToIndicatorDictionary.TryAdd(args.Id, indicator.ReferenceID);
            IndicatorToIdDictionary.TryAdd(indicator.ReferenceID, args.Id);

            indicatorSlot.OnPrepareDestroy += OnSlotPrepareDestroy;
        }

        private static void UpdateIndicator(object _, Utf8ImportProgressEventArgs args)
        {
            var indicator = GetProgressIndicator(args.Id, args.World);
            indicator?.UpdateProgress(args.Percent, $"Importing {args.FileTypeName}", $"{args.ReadSize} / {args.ByteSize} ({args.Percent}%)");
        }

        private static void CompleteIndicator(object _, Utf8ImportFinishEventArgs args)
        {
            var indicator = GetProgressIndicator(args.Id, args.World);
            indicator?.ProgressDone($"{args.FileTypeName} has been imported!");
            RemoveIndicatorCache(indicator.ReferenceID);
        }

        private static void FailIndicator(object _, Utf8ImportFailEventArgs args)
        {
            var indicator = GetProgressIndicator(args.Id, args.World);
            indicator?.ProgressFail($"Failed to import {args.FileTypeName}: {args.ErrorMessage}");
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
