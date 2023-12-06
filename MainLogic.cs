using BepInEx.Configuration;
using HarmonyLib;
using LootHero.loot;
using Microsoft.Xna.Framework;
using ProjectMage.character;
using ProjectMage.config;
using ProjectMage.gamestate;
using ProjectMage.player;
using System;
using System.Linq;

namespace SaltAndSacrificeAdjustments
{
    [HarmonyPatch]
    internal class MainLogic
    {
        private static float _healthPotionRegenDelay = 0;
        private static float _focusPotionRegenDelay = 0;
        private static float _rangedAmmoRegenDelay = 0;
        private static float _grayPearlRegenDelay = 0;

        private static void ResetRegenDelay()
        {
            _healthPotionRegenDelay = SaSAdjustments.HealthPotionRegenDelay.Value;
            _focusPotionRegenDelay = SaSAdjustments.FocusPotionRegenDelay.Value;
            _rangedAmmoRegenDelay = SaSAdjustments.RangedAmmoRegenDelay.Value;
            _grayPearlRegenDelay = SaSAdjustments.GrayPearlRegenDelay.Value;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerMgr), "Draw")]
        public static void DrawPluginText()
        {
            if (SaSAdjustments.DebugInfoEnabled.Value)
            {
                Menumancer.hud.Text.DrawText(
                    new System.Text.StringBuilder($"Health potion: {(int)_healthPotionRegenDelay}, Focus potion: {(int)_focusPotionRegenDelay}, Arrows: {(int)_rangedAmmoRegenDelay}, Gray pearl: {(int)_grayPearlRegenDelay}, Game state: {GameState.state}"),
                    new Common.Vector2(200, 350), Common.Color.White, 1.0f, 0);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerEquipment), "ReplenishConsumables")]
        public static void AfterReplenishConsumables(bool useStockpile)
        {
            ResetRegenDelay();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerMgr), "Init")]
        public static void AfterPlayerMgrInit()
        {
            ResetRegenDelay();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerMgr), "Update")]
        public static void MyCustomUpdate(float frameTime, float realTime)
        {
            if (GameState.state != 1)
                return;

            Player mainPlayer = GetMainPlayer();
            RegeneratePlayer(mainPlayer, frameTime);

            if(IsLocalCoopMode())
            {
                Player localCoopPlayer = GetLocalCoopPlayer();
                RegeneratePlayer(localCoopPlayer, frameTime);
            }
        }

        public static void RegeneratePlayer(Player player, float frameTime)
        {
            if (player.charIdx < 0 || !player.active)
                return;

            Character playerCharacter = CharMgr.character[player.charIdx];
            if (playerCharacter.hp <= 0)
                return;

            float staminaRegenRate = SaSAdjustments.StaminaRegenRate.Value;
            if (staminaRegenRate > 0)
            {
                playerCharacter.stamina += staminaRegenRate * frameTime;
                playerCharacter.stamina = Math.Min(playerCharacter.stamina, GetMaxStamina(player.stats, true));
            }

            float healthRegenRate = SaSAdjustments.HealthRegenRate.Value;
            if (healthRegenRate > 0)
            {
                playerCharacter.hp += healthRegenRate * frameTime;
                playerCharacter.hp = Math.Min(playerCharacter.hp, GetMaxHP(player.stats, true));
            }

            float manaRegenRate = SaSAdjustments.ManaRegenRate.Value;
            if (manaRegenRate > 0)
            {
                playerCharacter.mp += manaRegenRate * frameTime;
                playerCharacter.mp = Math.Min(playerCharacter.mp, GetMaxMP(player.stats, true));
            }

            float rageRegenRate = SaSAdjustments.RageRegenRate.Value;
            if (rageRegenRate > 0)
            {
                playerCharacter.rage += rageRegenRate * frameTime;
                playerCharacter.rage = Math.Min(playerCharacter.rage, GetMaxRage(player.stats, true));
            }

            if (SaSAdjustments.FocusPotionRegenEnabled.Value)
            {
                _focusPotionRegenDelay -= frameTime;
                if (_focusPotionRegenDelay <= 0)
                {
                    _focusPotionRegenDelay = SaSAdjustments.FocusPotionRegenDelay.Value;
                    AddItem(player, "focus_potion", 1, false);
                }
            }

            if (SaSAdjustments.HealthPotionRegenEnabled.Value)
            {
                _healthPotionRegenDelay -= frameTime;
                if (_healthPotionRegenDelay <= 0)
                {
                    _healthPotionRegenDelay = SaSAdjustments.HealthPotionRegenDelay.Value;
                    AddItem(player, "health_potion", 1, false);
                }
            }

            if (SaSAdjustments.RangedAmmoRegenEnabled.Value)
            {
                _rangedAmmoRegenDelay -= frameTime;
                if (_rangedAmmoRegenDelay <= 0)
                {
                    _rangedAmmoRegenDelay = SaSAdjustments.RangedAmmoRegenDelay.Value;
                    AddItem(player, "arrow", 1, true);
                }
            }

            if (SaSAdjustments.GrayPearlRegenEnabled.Value)
            {
                _grayPearlRegenDelay -= frameTime;
                if (_grayPearlRegenDelay <= 0)
                {
                    _grayPearlRegenDelay = SaSAdjustments.GrayPearlRegenDelay.Value;
                    int pearlCount = CountGrayPearls(player.equipment);
                    if (pearlCount < SaSAdjustments.GrayPearlRegenLimit.Value)
                        AddItem(player, "gray_pearl", 1, true);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerEquipment), "PopulateHVals")]
        public static void PopulateHVals(float[] hVals, int equipType, bool runicArt = false, bool scaled = true)
        {
            if (hVals.Count() == 6 && (equipType == 4 || equipType == 5 || equipType == 6))
            {
                for (int i = 0; i < 6; i++)
                {
                    hVals[i] *= SaSAdjustments.PlayerDamageMultiplier.Value;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerEquipment), "GetDefense")]
        public static void GetDefense(int damageType, bool scaled, ref float __result)
        {
            //SaSAdjustments.MainLog.LogInfo($"damageType: {damageType}, scaled: {scaled}, result: {__result}");
            __result *= SaSAdjustments.PlayerDefenseMultiplier.Value;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerStats), "CreateXPPile")]
        private static bool CreateXPPilePrefix(Character character)
        {
            return !SaSAdjustments.DoNotDropSaltOnDeath.Value;
        }

        private static int CountGrayPearls(PlayerEquipment playerEquipment)
        {
            int grayPearlCount = 0;
            for (int i = 0; i < playerEquipment.invItem.Count; i++)
            {
                PlayerItem playerItem = playerEquipment.invItem[i];
                LootDef lootDef = LootCatalog.lootDef[playerItem.lootIdx];
                if (lootDef.type == 4 && lootDef.flags != null)
                {
                    if (lootDef.flags.Contains(5) && playerItem.count > 0)
                        grayPearlCount += playerItem.count;
                }
            }
            return grayPearlCount;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlayerMgr), "GetMainPlayer")]
        internal static Player GetMainPlayer()
        {
            throw new NotImplementedException("Gets replaced with PlayerMgr.GetMainPlayer()");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlayerStats), "GetMaxStamina")]
        public static float GetMaxStamina(PlayerStats __instance, bool scaled = true)
        {
            throw new NotImplementedException("Gets replaced with PlayerStats.GetMaxStamina()");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlayerStats), "GetMaxHP")]
        public static float GetMaxHP(PlayerStats __instance, bool scaled = true)
        {
            throw new NotImplementedException("Gets replaced with PlayerStats.GetMaxHP()");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlayerStats), "GetMaxMP")]
        public static float GetMaxMP(PlayerStats __instance, bool scaled = true)
        {
            throw new NotImplementedException("Gets replaced with PlayerStats.GetMaxMP()");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlayerStats), "GetMaxRage")]
        public static float GetMaxRage(PlayerStats __instance, bool scaled = true)
        {
            throw new NotImplementedException("Gets replaced with PlayerStats.GetMaxRage()");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Player), "AddItem", new Type[] { typeof(string), typeof(int), typeof(bool) })]
        public static int AddItem(Player __instance, string loot, int count, bool dontList = false)
        {
            throw new NotImplementedException("Gets replaced with Player.AddItem(string,int,bool)");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlayerMgr), "IsLocalCoopMode")]
        public static bool IsLocalCoopMode()
        {
            throw new NotImplementedException("Gets replaced with PlayerMgr.IsLocalCoopMode()");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlayerMgr), "GetLocalCoopPlayer")]
        public static Player GetLocalCoopPlayer()
        {
            throw new NotImplementedException("Gets replaced with PlayerMgr.GetLocalCoopPlayer()");
        }
    }
}
