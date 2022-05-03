using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MetroidModPorted.Common.Players;

namespace MetroidModPorted.Content.Items.Armors
{
	[AutoloadEquip(EquipType.Body)]
	public class ChoziteBreastplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chozite Breastplate");
		}
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
			Item.value = 5000;
			Item.defense = 6;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs ) => head.type == ModContent.ItemType<ChoziteHelmet>() && body.type == ModContent.ItemType<ChoziteBreastplate>() && legs.type == ModContent.ItemType<ChoziteGreaves>();
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "+2 defense" + "\n"
				+ "Allows somersaulting & wall jumping";
			player.statDefense += 2;
			player.GetModPlayer<MPlayer>().EnableWallJump = true;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient<Miscellaneous.ChoziteBar>(30)
				.AddTile(TileID.Anvils)
				.Register();
			/*ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ChoziteBar", 30);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();*/
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class ChoziteGreaves : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chozite Greaves");
		}
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
			Item.value = 3000;
			Item.defense = 4;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient<Miscellaneous.ChoziteBar>(25)
				.AddTile(TileID.Anvils)
				.Register();
			/*ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ChoziteBar", 25);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();*/
		}
	}
	[AutoloadEquip(EquipType.Head)]
	public class ChoziteHelmet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chozite Helmet");
		}
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
			Item.value = 3000;
			Item.defense = 5;
		}
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient<Miscellaneous.ChoziteBar>(20)
				.AddTile(TileID.Anvils)
				.Register();
			/*ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ChoziteBar", 20);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();*/
		}
	}
}
