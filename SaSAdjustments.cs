using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.NetLauncher.Common;
using System.Runtime.CompilerServices;

namespace SaltAndSacrificeAdjustments
{
    // this is your plugin class which contains your entry point
    // this will be the first code that runs for your plugin
    [BepInEx.BepInPlugin("com.aurocosh.salt_and_sacrifice_adjustments", "Salt and Sacrifice Adjustments", "1.0")]
    public class SaSAdjustments : BasePlugin
    {
        public static ConfigEntry<float> HealthRegenRate;
        public static ConfigEntry<float> StaminaRegenRate;
        public static ConfigEntry<float> ManaRegenRate;
        public static ConfigEntry<float> RageRegenRate;

        public static ConfigEntry<bool> HealthPotionRegenEnabled;
        public static ConfigEntry<int> HealthPotionRegenDelay;

        public static ConfigEntry<bool> FocusPotionRegenEnabled;
        public static ConfigEntry<int> FocusPotionRegenDelay;

        public static ConfigEntry<bool> RangedAmmoRegenEnabled;
        public static ConfigEntry<int> RangedAmmoRegenDelay;

        public static ConfigEntry<bool> GrayPearlRegenEnabled;
        public static ConfigEntry<int> GrayPearlRegenDelay;
        public static ConfigEntry<int> GrayPearlRegenLimit;

        public static ConfigEntry<bool> MouseCursorInversionDisabled;
        public static ConfigEntry<bool> DoNotDropSaltOnDeath;

        public static ConfigEntry<float> PlayerDamageMultiplier;
        public static ConfigEntry<float> PlayerDefenseMultiplier;

        public static ConfigEntry<bool> DebugInfoEnabled;

        public static ManualLogSource MainLog;

        // Load is called by the BepInEx preloader when it creates an instance of your plugin
        // Optimisation is disabled for this function as it can cause issues with patching if it is optimised.
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public override void Load()
        {
            // Player main stat regen
            HealthRegenRate = Config.Bind(
                "Player stat regen",
                "Health regeneration rate",
                0.5f,
                new ConfigDescription("Determines the health regeneration rate.",
                new AcceptableValueRange<float>(0.0f, 20.0f)));
            StaminaRegenRate = Config.Bind(
                "Player stat regen",
                "Stamina regeneration rate",
                8.0f,
                new ConfigDescription("Determines the stamina regeneration rate. Does not affect natural stamina regen and unlike natural regen this regen is applied constantly.",
                new AcceptableValueRange<float>(0.0f, 20.0f)));
            ManaRegenRate = Config.Bind(
                "Player stat regen",
                "Mana regeneration rate",
                0.5f,
                new ConfigDescription("Determines the mana regeneration rate.",
                new AcceptableValueRange<float>(0.0f, 10.0f)));
            RageRegenRate = Config.Bind(
                "Player stat regen",
                "Rage regeneration rate",
                0.5f,
                new ConfigDescription("Determines the rage regeneration rate.",
                new AcceptableValueRange<float>(0.0f, 10.0f)));

            // Health potion regen
            HealthPotionRegenEnabled = Config.Bind(
                "Health potion regen",
                "Enable health potion regen",
                true,
                new ConfigDescription("Enables health potion regeneration."));
            HealthPotionRegenDelay = Config.Bind(
                "Health potion regen",
                "Health potion regen delay",
                300,
                new ConfigDescription("Determines the delay for the health potion regeneration. This value is in seconds.",
                new AcceptableValueRange<int>(10, 36000)));

            // Focus potion regen
            FocusPotionRegenEnabled = Config.Bind(
                "Focus potion regen",
                "Enable focus potion regen",
                true,
                new ConfigDescription("Enables focus potion regeneration."));
            FocusPotionRegenDelay = Config.Bind(
                "Focus potion regen",
                "Focus potion regen delay",
                300,
                new ConfigDescription("Determines the delay for the focus potion regeneration. This value is in seconds.",
                new AcceptableValueRange<int>(10, 36000)));

            // Ranged ammo regen
            RangedAmmoRegenEnabled = Config.Bind(
                "Ammo regen",
                "Enable ranged ammo regen",
                true,
                new ConfigDescription("Enables ranged ammo regeneration."));
            RangedAmmoRegenDelay = Config.Bind(
                "Ammo regen",
                "Ranged ammo regen delay",
                30,
                new ConfigDescription("Determines the delay for the health potion regeneration. This value is in seconds.",
                new AcceptableValueRange<int>(10, 36000)));

            // Gray pearls
            GrayPearlRegenEnabled = Config.Bind(
                "Gray pearl regen",
                "Enable gray pearl regen",
                true,
                new ConfigDescription("Enables gray pearl regeneration."));
            GrayPearlRegenDelay = Config.Bind(
                "Gray pearl regen",
                "Gray pearl regen delay",
                300, // TODO change
                new ConfigDescription("Determines the delay for the gray pearl regeneration. This value is in seconds.",
                new AcceptableValueRange<int>(10, 36000)));
            GrayPearlRegenLimit = Config.Bind(
                "Gray pearl regen",
                "Gray pearl regen limit",
                20, // TODO change
                new ConfigDescription("Determines the limit for the gray pearl regeneration. Regeneration will stop if you have that many Gray pearls or more.",
                new AcceptableValueRange<int>(1, 100)));

            // General
            MouseCursorInversionDisabled = Config.Bind(
                "General",
                "Disable mouse cursor inversion",
                false,
                new ConfigDescription("Disables inversion of mouse cursor when the cursor is on the left part of the screen."));
            DoNotDropSaltOnDeath = Config.Bind(
                "General",
                "Do not drop salt on death",
                false,
                new ConfigDescription("Will disable losing salt on death."));

            // Damage
            PlayerDamageMultiplier = Config.Bind(
                "Damage",
                "Player damage multiplier",
                1f,
                new ConfigDescription("Multiplies all player damage by this value. Value larger then 1 will increase your damage, and values between 0 and 1 will decrease your damage.",
                new AcceptableValueRange<float>(0.0f, 50.0f)));

            PlayerDefenseMultiplier = Config.Bind(
                "Damage",
                "Player defense multiplier",
                1f,
                new ConfigDescription("Multiplies all player defense by this value. Value larger then 1 will increase your defense, and values between 0 and 1 will decrease your defense.",
                new AcceptableValueRange<float>(0.0f, 50.0f)));

            // Debug
            DebugInfoEnabled = Config.Bind(
                "Debug",
                "Enable debug information",
                false,
                new ConfigDescription("Shows or hides debug information in game. Debug information includes delays in seconds until next item regen."));

            MainLog = Log;

            // this adds your class and method patches into the game code
            var harmony = new HarmonyLib.Harmony("salt_and_sacrifice_adjustments");
            harmony.PatchAll();


            Log.LogInfo("Mod initialized");
        }
    }
}
