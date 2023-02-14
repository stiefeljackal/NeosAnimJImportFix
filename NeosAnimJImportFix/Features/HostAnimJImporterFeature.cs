using CodeX;
using FrooxEngine;
using FrooxEngine.LogiX.Input;
using FrooxEngine.Undo;
using JworkzNeosMod.Events;
using JworkzNeosMod.Events.Publishers;
using JworkzNeosMod.Extensions;
using JworkzNeosMod.Features.Abstract;
using JworkzNeosMod.Utility;
using NeosModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JworkzNeosMod.Features
{
    internal class HostAnimJImporterFeature : IModSubFeature
    {
        private static ComponentEventPublisher _staticBinaryCompPublisher =
            ComponentEventPublisher.RegisterComponentEventPublisher<FileMetadata>();

        private Func<ModConfiguration> _configFunc;

        private ModConfiguration Config => _configFunc();

        private float SecondsToWait => Config.GetValue(JworkzAnimJImportFixMod.KEY_IMPORT_ASSIM_DELAY_SECONDS);

        public bool IsEnabled { get; private set; }

        public HostAnimJImporterFeature(Func<ModConfiguration> configFunc)
        {
            _configFunc = configFunc;
        }

        public void Refresh(bool isMainModEnabled)
        {
            var isSubFeatureEnabledInConfig = Config.GetValue(JworkzAnimJImportFixMod.KEY_HOST_IMPORT_ENABLE) && isMainModEnabled;

            if (IsEnabled != isSubFeatureEnabledInConfig)
            {
                IsEnabled = isSubFeatureEnabledInConfig;

                if (IsEnabled) { TurnOn();  }
                else { TurnOff(); }
            }
        }

        private void TurnOff()
        {
            _staticBinaryCompPublisher.OnAttachComplete -= OnStaticBinaryComponentAttachComplete;
        }

        private void TurnOn()
        {
            _staticBinaryCompPublisher.OnAttachComplete += OnStaticBinaryComponentAttachComplete;
        }

        private void OnStaticBinaryComponentAttachComplete(object sender, ComponentOnAttachCompleteEventArgs e)
        {
            var compSlot = e.Slot;
            if (!compSlot.World.HostUser.IsLocalUser) { return; }

            var staticBinaryComp = compSlot.GetComponent<StaticBinary>();
            if (staticBinaryComp == null) { return; }

            var fileMetadataComp = compSlot.GetComponent<FileMetadata>();
            if (fileMetadataComp == null) { return; }

            compSlot.World.Coroutines.StartTask(async () =>
            {
                await new ToWorld();
                await Task.Delay(TimeSpan.FromSeconds(SecondsToWait));

                var assimUri = staticBinaryComp.URL.Value;
                var animJSlotName = fileMetadataComp.Filename.Value;

                if (!animJSlotName.EndsWith(".assim") && !animJSlotName.EndsWith(".animj")) { return; }

                var animJSlot = await ConvertAssimToAnimJ(compSlot, assimUri, animJSlotName);

                compSlot.Destroy();
                DevCreateNewForm.OpenInspector(animJSlot);
            });
        }

        private static async Task<Slot> ConvertAssimToAnimJ(Slot compSlot, Uri assimUri, string animJSlotName)
        {

            var position = compSlot.GlobalPosition;
            var rotation = compSlot.GlobalRotation;
            var scale = compSlot.GlobalScale;

            var jsonFilePathStr = await Engine.Current.AssetManager.RequestGather(assimUri, Priority.Urgent).ConfigureAwait(false);
            var allocatingUser = compSlot.GetAllocatingUser();
            var world = compSlot.World;
            
            await new ToBackground();
            var animJUri = await compSlot.Engine.LocalDB.SaveAssetAsync(
                AnimJImporterPlus.ImportFromFile(jsonFilePathStr, allocatingUser)
            );
            await new ToWorld();

            var animJSlot = compSlot.Parent.AddSlot(animJSlotName);
            animJSlot.GlobalPosition = position;
            animJSlot.GlobalRotation = rotation;
            animJSlot.GlobalScale = scale;

            var animationProvider = animJSlot.AttachComponent<StaticAnimationProvider>();
            animationProvider.URL.Value = animJUri;
            animationProvider.ForceLoad();

            return animJSlot;
        }
    }
}
