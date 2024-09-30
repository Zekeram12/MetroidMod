using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace MetroidMod.Content.Projectiles
{
	internal class BeamShot2 : ModProjectile
	{
		Color color = MetroidMod.powColor;
		public float beamScale = 0.75f;
		public override string Texture => $"{nameof(MetroidMod)}/Assets/Textures/Beams/Power/Shot";
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.scale = 0.75f;
		}
		public override void AI() //TODO: make a whole-ass thing
		{
			Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;
			Lighting.AddLight(Projectile.Center, color.R / 255f, color.G / 255f, color.B / 255f);

			if (Projectile.numUpdates == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 64, 0, 0, 100, default(Color), Projectile.scale);
				Main.dust[dust].noGravity = true;
			}
		}
		public override void OnKill(int timeLeft)
		{
			Vector2 pos = Projectile.position;
			int freq = 20;
			int dustType = 64;
			bool noGravity = true;
			for (int i = 0; i < freq; i++)
			{
				int dust = Dust.NewDust(pos, Projectile.width, Projectile.height, dustType, 0, 0, 100, color, Projectile.scale * beamScale);
				Main.dust[dust].velocity = new Vector2((Main.rand.Next(freq) - (freq / 2)) * 0.125f, (Main.rand.Next(freq) - (freq / 2)) * 0.125f);
				Main.dust[dust].noGravity = noGravity;
			}
			SoundStyle sound = new($"{MetroidMod.Instance.Name}/Assets/Sounds/BeamImpactSound");
			SoundEngine.PlaySound(sound, Projectile.Center);
		}
	}
}
