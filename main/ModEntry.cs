using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Integrations.GenericModConfigMenu;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using static StardewValley.Game1;

namespace WarpToFarm
{
    public sealed class ModConfig
    {
        public GeneralOptions General { get; set; } = new();
        public WarpTweaks Tweaks { get; set; } = new();

        public class GeneralOptions
        {
            public bool ModEnabled { get; set; } = true;
            public float StaminaCost { get; set; } = 100;
            public KeybindList WarpButton { get; set; } = KeybindList.Parse("F2, LeftShoulder + X");
        }

        public class WarpTweaks
        {
            public int WarpPosX { get; set; } = 64;
            public int WarpPosY { get; set; } = 15;
            public int WarpDirection { get; set; } = 1;
        }
    }

    internal sealed class ModEntry : Mod
    {
        private ModConfig config;
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            this.config = helper.ReadConfig<ModConfig>();
        }

        private void warp()
        {
            Game1.warpFarmer("Farm", this.config.Tweaks.WarpPosX,
                this.config.Tweaks.WarpPosY, this.config.Tweaks.WarpDirection);
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.displayFarmer = true;
            Game1.player.CanMove = true;
            Game1.player.temporarilyInvincible = false;
            Farmer player = Game1.player;
            player.Stamina -= this.config.General.StaminaCost;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            //IL_0079: Unknown result type (might be due to invalid IL or missing references)
            //IL_0085: Expected O, but got Unknown
            //IL_004c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0056: Expected O, but got Unknown
            if (!Context.IsPlayerFree)
            {
                return;
            }
            if (config.General.WarpButton.JustPressed())
            {
                if (Game1.player.Stamina < this.config.General.StaminaCost)
                {
                    float cost = this.config.General.StaminaCost;
                    Game1.addHUDMessage(new HUDMessage($"{cost} energy is needed to teleport.", 3));
                    return;
                }
                Game1.player.CanMove = false;
                Game1.player.temporarilyInvincible = true;
                DelayedAction.fadeAfterDelay(new afterFadeFunction(warp), 100);
            }
        }
    }
}
