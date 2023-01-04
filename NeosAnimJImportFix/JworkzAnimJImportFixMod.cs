using CodeX;
using NeosModLoader;
using HarmonyLib;
using FrooxEngine;
using BaseX;
using System.Text.Json;
using System.Text.Json.Serialization;
using JworkzNeosMod.Watchers;

namespace JworkzNeosMod
{
    public class JworkzAnimJImportFixMod : NeosMod
    {
        public override string Name => nameof(JworkzAnimJImportFixMod);
        public override string Author => "Stiefel Jackal";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/stiefeljackal/NeosAnimJImportFixMod";

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> KEY_ENABLE =
            new ModConfigurationKey<bool>("enabled", $"Enables the {nameof(JworkzAnimJImportFixMod)} mod", () => true);

        private static ModConfiguration Config;

        private Harmony harmony;

        public bool IsEnabled { get; private set; }

        public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder) =>
            builder.Version(Version).AutoSave(false);

        public override void OnEngineInit()
        {
            ImporterProgressIndicatorWatcher.Init();
            harmony = new Harmony($"jworkz.sjackal.{Name}");
            Config = GetConfiguration();
            Config.OnThisConfigurationChanged += OnConfigurationChanged;

            RefreshMod();
        }

        private void RefreshMod()
        {
            var isEnabled = Config.GetValue(KEY_ENABLE);

            if (isEnabled) { TurnModOn(); }
            else { TurnModOff(); }
        }

        private void TurnModOn() => harmony.PatchAll();

        private void TurnModOff() => harmony.UnpatchAll(harmony.Id);

        private void OnConfigurationChanged(ConfigurationChangedEvent @event) => RefreshMod();

    }
}
