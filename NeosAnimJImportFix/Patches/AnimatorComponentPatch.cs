using BaseX;
using CodeX;
using FrooxEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JworkzNeosMod.Patches
{
    [HarmonyPatch(typeof(Animator), nameof(Animator.SetupFieldsByName))]
    internal static class AnimatorComponentPatch
    {
        private const int MAX_SPLIT_LENGTH = 2;

        private const int SLOT_FIELD_INDEX = 0;

        private const int SLOT_COMP_INDEX = 0;

        private const int COMP_FIELD_INDEX = 1;

        private static readonly Regex COMPONENT_FIELD_SPLIT_REGEX = new Regex(@"\.(?!.*?\.)");

        static bool Prefix(ref Task __result, ref Animator __instance, Slot root)
        {
            HashSet<Slot> ignoreSlots = new HashSet<Slot>();
            var startTaskDelegate = AccessTools.Method(typeof(Animator), "StartTask", new[] { typeof(Func<Task>) });
            var setupFieldsAsync = AccessTools.Method(typeof(Animator), "SetupFieldsAsync", new[] { typeof(Func<BaseX.AnimationTrack, IField>), typeof(HashSet<Slot>) });

            Func<BaseX.AnimationTrack, IField> setupFieldsAsyncCb = (BaseX.AnimationTrack track) =>
            {
                var slot = root.FindChild((Slot s) => s.Name == track.Node && !ignoreSlots.Contains(s));
                if (slot == null) { return null; }

                string[] compFieldNamePair = COMPONENT_FIELD_SPLIT_REGEX.Split(track.Property);

                if (compFieldNamePair.Length > MAX_SPLIT_LENGTH)
                {
                    throw new Exception("Invalid Property Path: " + track.Property);
                }

                return compFieldNamePair.Length == MAX_SPLIT_LENGTH
                    ? GetComponentField(slot, compFieldNamePair[SLOT_COMP_INDEX], compFieldNamePair[COMP_FIELD_INDEX])
                    : slot.TryGetField(compFieldNamePair[SLOT_FIELD_INDEX]);

            };

            var setupFieldsTask = (Task) setupFieldsAsync.Invoke(__instance, new object[] { setupFieldsAsyncCb, ignoreSlots });
            
            Action startTaskInit = async () => await setupFieldsTask.ConfigureAwait(false);
            __result = (Task) startTaskDelegate.Invoke(__instance, new object[] { startTaskInit });

            return false;
        }

        private static IField GetComponentField(Slot slot, string componentTypeName, string componentFieldName)
        {
            var components = slot.GetComponents<Component>((c) => c.WorkerTypeName == componentTypeName).OrderBy(c => c.UpdateOrder).ToArray();
            Component foundComponent = null;

            for (var i = 0; i < components.Count() && foundComponent == null; i++)
            {
                var component = components[i];
                if (!component.IsDriven)
                {
                    foundComponent = component;
                }
            }

            return foundComponent?.TryGetField(componentFieldName);
        }
    }
}
