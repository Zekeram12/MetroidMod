using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MetroidMod.ID;

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
	}
}
