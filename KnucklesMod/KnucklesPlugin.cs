using BepInEx;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KnucklesMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class KnucklesPlugin : BaseUnityPlugin
    {
        public static AssetBundle knuckles_bundle;

        public static BepInEx.Configuration.ConfigEntry<bool> screamNearEnemies;
        public static BepInEx.Configuration.ConfigEntry<float> screamVolume;
        public static BepInEx.Configuration.ConfigEntry<float> enemyCheckRadius;

        private void Awake()
        {
            screamNearEnemies = Config.Bind("General", "Scream Near Enemies", true, "Whether or not Knucks screams when nearby enemies.");
            screamVolume = Config.Bind("General", "Scream Volume", 0.7f, "Quantifies the level of Knucks' fear around enemies.");
            enemyCheckRadius = Config.Bind("General", "Enemy Fear Radius", 20f, "How close an enemy needs to be for Knucks to be afraid of it.");

            // check if using LethalConfig
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("ainavt.lc.lethalconfig"))
                LethalConfiguration.setupLethalConfig();

            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            knuckles_bundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "knucklesbundle"));
            if (knuckles_bundle == null)
            {
                Logger.LogError("Failed to load custom assets.");
                return;
            }

            int iRarity = 70;

            Logger.LogInfo("Loading Knuckles Item");
            Item knuckles = knuckles_bundle.LoadAsset<Item>("Knuckles");

            Logger.LogInfo("Configuring KnucklesItem Component");
            knuckles.spawnPrefab.AddComponent(typeof(KnucklesItem));
            knuckles.spawnPrefab.GetComponent<KnucklesItem>().scream = knuckles_bundle.LoadAsset<AudioClip>("cartoon-scream");
            knuckles.spawnPrefab.GetComponent<KnucklesItem>().screamPlayer = knuckles.spawnPrefab.GetComponent<AudioSource>();
            knuckles.spawnPrefab.GetComponent<KnucklesItem>().itemProperties = knuckles;
            knuckles.spawnPrefab.GetComponent<KnucklesItem>().mainObjectRenderer = knuckles.spawnPrefab.GetComponent<MeshRenderer>();

            Logger.LogInfo("Registering Knuckles as scrap");
            LethalLib.Modules.Utilities.FixMixerGroups(knuckles.spawnPrefab);
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(knuckles.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(knuckles, iRarity, LethalLib.Modules.Levels.LevelTypes.All);

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}