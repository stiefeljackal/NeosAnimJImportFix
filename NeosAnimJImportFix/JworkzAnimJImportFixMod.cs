using CodeX;
using NeosModLoader;
using HarmonyLib;
using FrooxEngine;
using BaseX;
using System.Text.Json;
using System.Text.Json.Serialization;
using JworkzNeosMod.Watchers;
using System.Collections.Generic;
using JworkzNeosMod.Features.Abstract;
using JworkzNeosMod.Features;
using System.Linq;
using JworkzNeosMod.Events.Publishers;

namespace JworkzNeosMod
{
    public class JworkzAnimJImportFixMod : NeosMod
    {
        public const float DEFAULT_IMPORT_ASSIM_DELAY_SECONDS = 0.25f;

        public override string Name => nameof(JworkzAnimJImportFixMod);
        public override string Author => "Stiefel Jackal";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/stiefeljackal/NeosAnimJImportFixMod";

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> KEY_ENABLE =
            new ModConfigurationKey<bool>("enabled", $"Enables the {nameof(JworkzAnimJImportFixMod)} mod", () => true);

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<bool> KEY_HOST_IMPORT_ENABLE =
            new ModConfigurationKey<bool>("enabledHostAnimJImporter", $"Enables the 'Allow Host to Import AnimJ' subfeature: the host with this mod can import AnimJ with the extension 'assim'.", () => false);

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<float> KEY_IMPORT_ASSIM_DELAY_SECONDS =
            new ModConfigurationKey<float>("applyUpdates", $"Number of seconds to wait until the mod attempts to import the 'assim' file as AnimJ.", () => DEFAULT_IMPORT_ASSIM_DELAY_SECONDS);

        private static ModConfiguration _config;

        private static WorldManager WorldManager => Engine.Current.WorldManager;

        private static IModSubFeature[] _subFeatures = new IModSubFeature[]
        {
            new HostAnimJImporterFeature(() => _config)
        };
        
        private Harmony _harmony;

        public bool IsEnabled { get; private set; }

        public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder) =>
            builder.Version(Version).AutoSave(false);

        public override void OnEngineInit()
        {
            ImporterProgressIndicatorWatcher.Init();
            _harmony = new Harmony($"jworkz.sjackal.{Name}");
            _config = GetConfiguration();
            _config.OnThisConfigurationChanged += OnConfigurationChanged;

            Engine.Current.OnReady += OnCurrentNeosEngineReady;
        }

        private void RefreshMod()
        {
            var isEnabledInConfig = _config.GetValue(KEY_ENABLE);

            if (IsEnabled != isEnabledInConfig)
            {
                IsEnabled = isEnabledInConfig;

                if (IsEnabled) { TurnModOn(); }
                else { TurnModOff(); }
            }

            foreach(var subFeature in _subFeatures)
            {
                subFeature.Refresh(IsEnabled);
            }
        }

        private void TurnModOn()
        {
            _harmony.PatchAll();

            WorldManager.Worlds?.Do(OnWorldAdded);
            WorldManager.WorldAdded += OnWorldAdded;
            WorldManager.WorldRemoved+= OnWorldRemoved;
        }

        private void TurnModOff()
        {
            _harmony.UnpatchAll(_harmony.Id);

            WorldManager.WorldAdded -= OnWorldAdded;
            WorldManager.WorldRemoved -= OnWorldRemoved;
            WorldManager.Worlds?.Do(OnWorldRemoved);
        }

        private void OnConfigurationChanged(ConfigurationChangedEvent @event) => RefreshMod();

        private void OnCurrentNeosEngineReady() => RefreshMod();

        private void OnWorldAdded(World world) => world.ComponentAdded += OnComponentAdded;

        private void OnWorldRemoved(World world) => world.ComponentAdded -= OnComponentAdded;

        private void OnComponentAdded(Slot _, Component component) =>
            ComponentEventPublisher.RaiseOnAttachCompleteEvent(component);
    }
}
