using BaseX;
using CodeX;
using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
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

        private static readonly Regex COMPONENT_FULLNAME_METADATA_REGEX = new Regex(@"(?:,\s?mscorlib)(?:,\sVersion=(?:\d\.?)+)?(?:,\sCulture=\w+)?(:?,\sPublicKeyToken=\w+)?");

        static bool Prefix(ref Task __result, Animator __instance, Slot root)
        {
            HashSet<Slot> ignoreSlots = new HashSet<Slot>();

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
            __result = __instance.StartTask(() => StartSetupFieldsAsync(ref __instance, ignoreSlots, setupFieldsAsyncCb));

            return false;
        }

        private static Task StartSetupFieldsAsync(ref Animator animator, HashSet<Slot> ignoreSlots, Func<BaseX.AnimationTrack, IField> callback)
        {
            var setupFieldsAsync = AccessTools.Method(typeof(Animator), "SetupFieldsAsync", new[] { typeof(Func<BaseX.AnimationTrack, IField>), typeof(HashSet<Slot>) });
            return (Task) setupFieldsAsync.Invoke(animator, new object[] { callback, ignoreSlots });
        }

        private static IField GetComponentField(Slot slot, string componentTypeName, string componentFieldName)
        {
            var components = slot.GetComponents<Component>((c) =>
                c.WorkerTypeName == componentTypeName ||
                c.WorkerType.GetNiceName() == componentTypeName ||
                COMPONENT_FULLNAME_METADATA_REGEX.Replace(c.WorkerTypeName, "") == componentTypeName
            ).OrderBy(c => c.UpdateOrder).ToArray();
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
