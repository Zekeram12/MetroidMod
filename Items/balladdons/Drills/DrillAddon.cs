using Terraria.ID;
using Terraria.ModLoader;

namespace MetroidMod.Items.balladdons.Drills
{
	public class DrillAddon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Morph Ball Drill");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~60% pickaxe power\n" +
			"Can mine Meteorite\n" +
			string.Format("[c/78BE78:Requires Morph Ball to use]"));
		}
		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.maxStack = 1;
			item.value = 15000;
			item.rare = 1;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 15;
			item.useStyle = 1;
			MGlobalItem mItem = item.GetGlobalItem<MGlobalItem>();
			mItem.ballSlotType = 0;
			mItem.drillPower = 60;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ChoziteBar", 12);
            recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class DrillAddonMk2 : DrillAddon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Morph Ball Drill MK2");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~70% pickaxe power\n" +
			"Able to mine Hellstone");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.GetGlobalItem<MGlobalItem>().drillPower = 70;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddon");
			recipe.AddIngredient(ItemID.DemoniteBar, 5);
			recipe.AddIngredient(ItemID.ShadowScale, 1);
            recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
			
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddon");
			recipe.AddIngredient(ItemID.CrimtaneBar, 5);
			recipe.AddIngredient(ItemID.TissueSample, 1);
            recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class DrillAddonMk3 : DrillAddon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Morph Ball Drill MK3");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~100% pickaxe power\n" +
			"Can mine Cobalt and Palladium");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.GetGlobalItem<MGlobalItem>().drillPower = 100;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddonMk2");
			recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class DrillAddonMkHM1 : DrillAddon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Morph Ball Drill MK4");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~130% pickaxe power\n" +
			"Can mine Mythril and Orichalcum");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.GetGlobalItem<MGlobalItem>().drillPower = 130;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddonMk3");
			recipe.AddIngredient(ItemID.CobaltBar, 5);
            recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
			
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddonMk3");
			recipe.AddIngredient(ItemID.PalladiumBar, 5);
            recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class DrillAddonMkHM2 : DrillAddon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Morph Ball Drill MK5");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~165% pickaxe power\n" +
			"Can mine Adamantite and Titanium");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.GetGlobalItem<MGlobalItem>().drillPower = 165;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddonMkHM1");
			recipe.AddIngredient(ItemID.MythrilBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
			
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddonMkHM1");
			recipe.AddIngredient(ItemID.OrichalcumBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class DrillAddonMkHM3 : DrillAddon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Morph Ball Drill MK6");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~190% pickaxe power");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.GetGlobalItem<MGlobalItem>().drillPower = 190;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddonMkHM2");
			recipe.AddIngredient(ItemID.AdamantiteBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
			
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddonMkHM2");
			recipe.AddIngredient(ItemID.TitaniumBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class DrillAddon_Hallowed : DrillAddon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hallowed Morph Ball Drill");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~200% pickaxe power\n" +
			"Can mine Chlorophyte");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.GetGlobalItem<MGlobalItem>().drillPower = 200;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddonMkHM3");
			recipe.AddIngredient(ItemID.HallowedBar, 5);
			recipe.AddIngredient(ItemID.SoulofMight);
			recipe.AddIngredient(ItemID.SoulofSight);
			recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class DrillAddon_Lihzahrd : DrillAddon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lihzahrd Morph Ball Drill");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~210% pickaxe power\n" +
			"Capable of mining Lihzahrd Bricks");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.GetGlobalItem<MGlobalItem>().drillPower = 210;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddon_Hallowed");
			recipe.AddIngredient(ItemID.LihzahrdBrick, 50);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class DrillAddon_Luminite : DrillAddon
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Luminite Morph Ball Drill");
			Tooltip.SetDefault("Morph Ball Addon\n" +
			"Slot Type: Drill\n" +
			"~Left Click while morphed to drill\n" +
			"~225% pickaxe power");
		}
		public override void SetDefaults()
		{
			base.SetDefaults();
			item.GetGlobalItem<MGlobalItem>().drillPower = 225;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DrillAddon_Lihzahrd");
			recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
