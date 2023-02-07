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

        private static ModConfiguration _config;

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

            RefreshMod();
        }

        private void RefreshMod()
        {
            var isEnabled = _config.GetValue(KEY_ENABLE);

            if (isEnabled) { TurnModOn(); }
            else { TurnModOff(); }
        }

        private void TurnModOn() => _harmony.PatchAll();

        private void TurnModOff() => _harmony.UnpatchAll(_harmony.Id);

        private void OnConfigurationChanged(ConfigurationChangedEvent @event) => RefreshMod();

    }
}
