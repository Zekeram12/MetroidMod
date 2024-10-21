using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MetroidMod.ID;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using MetroidMod.Common.GlobalItems;
using MetroidMod.Common.Players;
using MetroidMod.Content.Items.Weapons;
using MetroidMod.Content.Projectiles;
using rail;

namespace MetroidMod.Content.BeamAddons
{
	//So fun fact, this is the first ModBeamAddon ever made!      -Z
	public class ChargeBeam : ModBeamAddon
	{
		public override bool AddOnlyAddonItem => false; //Idk why you'd ever want to enable this
		public override Color ShotColor => new(248, 248, 110); //Highly recommend making the shot texture greyscale for maximum effect
		public override int ShotDust => 64;

		public override string ShotSound => $"{Mod.Name}/Assets/Sounds/ArmCannon/Shot";

		public override string ImpactSound => $"{Mod.Name}/Assets/Sounds/ArmCannon/BeamImpactSound";

		public override int ShapePriority => 0;
		public override int ColorPriority => 0;

		public float chargeMultiplier = 3f;

		public override void SetStaticDefaults()
		{
			//these values determine how the addon will interact with the dynamic visual system
			AddonSlot = BeamAddonSlotID.Primary;

			BaseDamage = 5;
			DamageMult = 5f;
			BaseOverheat = 5;
		}

		public override void SetItemDefaults(Item item)
		{
			item.rare = ItemRarityID.Blue;
		}
	}
}

