using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;

namespace KnucklesMod
{
    class LethalConfiguration
    {
        public static void setupLethalConfig()
        {
            var screamCheckBox = new BoolCheckBoxConfigItem(KnucklesPlugin.screamNearEnemies, requiresRestart: false);
            var screamVolumeSlider = new FloatSliderConfigItem(KnucklesPlugin.screamVolume, new FloatSliderOptions
            {
                Min = 0f,
                Max = 1f,
                RequiresRestart = false
            });

            var fearSlider = new FloatSliderConfigItem(KnucklesPlugin.enemyCheckRadius, new FloatSliderOptions
            {
                Min = 0f,
                Max = 30f,
                RequiresRestart = false
            });

            LethalConfigManager.AddConfigItem(screamCheckBox);
            LethalConfigManager.AddConfigItem(screamVolumeSlider);
            LethalConfigManager.AddConfigItem(fearSlider);
        }
    }
}
