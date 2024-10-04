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
		public override bool AddOnlyAddonItem => false;

		public override Color ShotColor => new(0, 0, 255);

		public override void SetStaticDefaults()
		{
			ShapePriority = 1;
			ColorPriority = 3;
			AddonSlot = BeamAddonSlotID.Secondary;
			BaseDamage = bd;
			BaseSpeed = bs;
		}

		public override void SetItemDefaults(Item item)
		{
			Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(0, 6, 9, 0);
			Item.rare = ItemRarityID.Cyan;
		}
	}
}
