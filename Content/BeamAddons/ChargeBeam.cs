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
	//So fun fact, this is the first ModBeamAddon ever made!      -Z
	public class ChargeBeam : ModBeamAddon
	{
		public override bool AddOnlyAddonItem => false; //Idk why you'd ever want to enable this
		public override Color ShotColor => new(248, 248, 110); //Highly recommend making the shot texture greyscale for maximum effect

		public override void SetStaticDefaults()
		{
			//these values determine how the addon will interact with the dynamic visual system
			ShapePriority = 0;
			ColorPriority = 0;
			VIB = false; //General rule of thumb: only enable this if your beam is Special:tm:
			AddonSlot = BeamAddonSlotID.Charge;
		}
	}
}
