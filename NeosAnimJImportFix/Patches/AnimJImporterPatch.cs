using BaseX;
using CodeX;
using FrooxEngine;
using HarmonyLib;
using JworkzNeosMod.Events.Publishers;
using JworkzNeosMod.Events.Watchers;
using JworkzNeosMod.Extensions;
using JworkzNeosMod.JsonConverters;
using JworkzNeosMod.Models;
using JworkzNeosMod.Utility;
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
        static bool Prefix(ref AnimX __result, string json)
        {
            var world = Engine.Current.WorldManager.FocusedWorld;
            var localUser = world.LocalUser;

            __result = AnimJImporterPlus.ImportFromJSON(json, localUser);

            return false;
        }
    }

    
}
