using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using MetroidMod.Content.Items.Weapons;

namespace MetroidMod.Content.Projectiles.VoltDriver
{
	public class VoltDriverShot : MProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Volt Driver Shot");
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			
			string S  = PowerBeam.SetCondition();
			base.SetDefaults();
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.scale = 0.75f;
			Projectile.damage = 15;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
			//Projectile.extraUpdates = 3;
			if (S.Contains("green"))
			{
				Projectile.penetrate = 6;
			}
			if (S.Contains("nova"))
			{
				Projectile.penetrate = 8;
			}
			if (S.Contains("solar"))
			{
				Projectile.penetrate = 12;
			}
		}

		public override void AI()
		{
			
			string S  = PowerBeam.SetCondition();
			if (S.Contains("wave") || S.Contains("nebula"))
			{
				Projectile.tileCollide = false;
				mProjectile.WaveBehavior(Projectile);
			}
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
			Color color = MetroidMod.powColor;
			Lighting.AddLight(Projectile.Center, color.R / 255f, color.G / 255f, color.B / 255f);

			if (Projectile.numUpdates == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 269, 0, 0, 100, default(Color), Projectile.scale);
				Main.dust[dust].noGravity = true;
			}
			Projectile.frame++;
			if (Projectile.frame > 3)
			{
				Projectile.frame = 0;
			}
		}
		public override void Kill(int timeLeft)
		{
			mProjectile.DustyDeath(Projectile, 269);
			SoundEngine.PlaySound(Sounds.Items.Weapons.VoltDriverImpactSound, Projectile.position);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			mProjectile.DrawCentered(Projectile, Main.spriteBatch);
			return false;
		}
	}
}
