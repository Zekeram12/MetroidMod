using MetroidMod.Common.GlobalItems;
using Terraria.ID;
using Terraria.ModLoader;

namespace MetroidMod.Content.Items.Addons
{
	public class PlasmaBeamGreenAddon : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plasma Beam (Green)");
			/* Tooltip.SetDefault(string.Format("[c/9696FF:Power Beam Addon]\n") +
				string.Format("[c/FF9696:Power Beam Addon V2]\n") +
				"Slot Type: Primary B\n" +
				"Shots pierce enemies\n" +
				string.Format("[c/78BE78:+100% damage]\n") +
				string.Format("[c/BE7878:+75% overheat use]\n") +
				string.Format("[c/BE7878:-15% speed]")); */
			//ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PlasmaBeamRedAddon>(); //Leftover from when it was an alt
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.width = 10;
			Item.height = 14;
			Item.maxStack = 1;
			Item.value = 70000;
			Item.rare = ItemRarityID.Pink;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Content.Tiles.ItemTile.Beam.PlasmaBeamGreenTile>();
			MGlobalItem mItem = Item.GetGlobalItem<MGlobalItem>();
			mItem.addonSlotType = 4;
			mItem.addonDmg = Common.Configs.MConfigItems.Instance.damagePlasmaBeamGreen;
			mItem.addonHeat = Common.Configs.MConfigItems.Instance.overheatPlasmaBeamGreen;
			mItem.addonSpeed = Common.Configs.MConfigItems.Instance.speedPlasmaBeamGreen;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Miscellaneous.UnknownPlasmaBeam>(1)
				.AddIngredient(ItemID.HallowedBar, 10)
				.AddIngredient(ItemID.SoulofSight, 5)
				.AddIngredient(ItemID.Emerald, 1)
				.AddTile(TileID.MythrilAnvil)
				.Register();

			CreateRecipe()
				.AddIngredient<Miscellaneous.UnknownPlasmaBeam>(1)
				.AddIngredient(ItemID.HallowedBar, 10)
				.AddIngredient(ItemID.SoulofMight, 5)
				.AddIngredient(ItemID.Emerald, 1)
				.AddTile(TileID.MythrilAnvil)
				.Register();

			CreateRecipe()
				.AddIngredient<Miscellaneous.UnknownPlasmaBeam>(1)
				.AddIngredient(ItemID.HallowedBar, 10)
				.AddIngredient(ItemID.SoulofFright, 5)
				.AddIngredient(ItemID.Emerald, 1)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
