using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.UI;
using MetroidMod.ID;
using Microsoft.Xna.Framework;
using Terraria;
using MetroidMod.Content.Items.Weapons;
using Terraria.GameContent.UI.Elements;

namespace MetroidMod.Common.UI
{
	/// <summary>
	/// The UI for the arm cannon
	/// <br/> jazz this desc up later
	/// </summary>
	internal class ArmCannonUI : UIState
	{
		public static bool Visible => Main.playerInventory && Main.LocalPlayer.chest == -1 && (Main.LocalPlayer.inventory[Main.LocalPlayer.MetroidPlayer().selectedItem].type == ModContent.ItemType<PowerBeam2>());

		public override void OnInitialize()
		{
			UIPanel panel = new UIPanel();
			panel.Width.Set(600, 0);
			panel.Height.Set(300, 0);


		}
	}
}
