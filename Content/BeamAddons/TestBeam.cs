using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MetroidMod.ID;
using Terraria;
using Terraria.ID;

namespace MetroidMod.Content.BeamAddons
{
	internal class TestBeam : ModBeamAddon
	{
		int bd = 50;
		int bs = -5;

		public override int ShapePriority => 1;
		public override int ColorPriority => 3;
		public override bool AddOnlyAddonItem => false;

		public override string ImpactSound => $"{Mod.Name}/Assets/Sounds/BeamAddons/{Name}/Impact"; //This is the filepath for a custom beam sound if you don't like the default

		public override Color ShotColor => new(0, 0, 255, 1f);
		public override int ShotDust => 33;

		public override void SetStaticDefaults()
		{
			AddonSlot = BeamAddonSlotID.Secondary;
			BaseDamage = bd;
			BaseSpeed = bs;
		}

		public override void SetItemDefaults(Item item)
		{
			item.width = 16;
			item.height = 16;
			item.value = Item.buyPrice(0, 6, 9, 0);
			item.rare = ItemRarityID.Cyan;
		}
	}
}
