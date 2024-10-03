using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria;
using MetroidMod.Content.SuitAddons;

namespace MetroidMod.Default
{
	[Autoload(false)] //Need this for this blank sucker otherwise Mr. Template will turn into a real item
	internal class BeamAddonItem : ModItem
	{
		public ModBeamAddon modBeamAddon;

		public override string Texture => modBeamAddon.ItemTexture;
		public override string Name => modBeamAddon.Name + "Addon";
		public override LocalizedText Tooltip => modBeamAddon.Tooltip ?? base.Tooltip;

		public BeamAddonItem(ModBeamAddon modBeamAddon)
		{
			this.modBeamAddon = modBeamAddon;
		}

		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.maxStack = 1;
			Item.width = Main.netMode == NetmodeID.Server ? 32 : ModContent.Request<Texture2D>(Texture).Value.Width;
			Item.height = Main.netMode == NetmodeID.Server ? 32 : ModContent.Request<Texture2D>(Texture).Value.Height;
			modBeamAddon.SetItemDefaults(Item);
			modBeamAddon.ItemType = Type;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.vanity = false;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = modBeamAddon.TileType;
		}

		public override void HoldItem(Player player)
		{
			if (modBeamAddon.ShowTileHover(player))
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = Type;
			}
		}

		public override void AddRecipes()
		{
			modBeamAddon.AddRecipes();
		}
	}
}
