﻿using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace MetroidMod.Content.Projectiles.Paralyzer
{
	public class ParalyzerShot : MProjectile
	{
		public override string Texture => $"{nameof(MetroidMod)}/Assets/Textures/BeamAddons/DiffusionBeam/Shot";
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Paralyzer Blast");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.scale = 2f;
		}

		int dustType = 64;
		Color color = MetroidMod.powColor;
		public override void AI()
		{
			base.AI();
			Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;
			Lighting.AddLight(Projectile.Center, color.R / 255f, color.G / 255f, color.B / 255f);

			if (Projectile.numUpdates == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0, 0, 100, default(Color), Projectile.scale);
				Main.dust[dust].noGravity = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			mProjectile.DustyDeath(Projectile, dustType);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			mProjectile.DrawCentered(Projectile, Main.spriteBatch);
			return false;
		}
	}
}
