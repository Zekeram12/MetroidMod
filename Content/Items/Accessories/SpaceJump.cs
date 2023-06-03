using Terraria;
using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MetroidMod.Common.Players;

namespace MetroidMod.Content.Items.Accessories
{
	public class SpaceJump : ModItem
	{
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Space Jump");
			//Tooltip.SetDefault("'Somersault continuously in the air!'\n" + 
			/*"Allows somersaulting\n" + 
			"Allows the user to jump up to 10 times in a row\n" + 
			"Jumps recharge mid-air\n" +
			"Increases jump height and prevents fall damage");*/

			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.value = 40000;
			Item.rare = ItemRarityID.Lime;
			Item.accessory = true;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Content.Tiles.ItemTile.SpaceJumpTile>();
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient<SpaceJumpBoots>(1)
				.AddIngredient(ItemID.HallowedBar, 10)
				.AddIngredient(ItemID.SoulofFlight, 10)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			MPlayer mp = player.GetModPlayer<MPlayer>();
			mp.spaceJump = true;
			mp.hiJumpBoost = true;
			player.noFallDmg = true;
		}
	}
}
