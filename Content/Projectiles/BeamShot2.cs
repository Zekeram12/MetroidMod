using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroidMod.Content.DamageClasses;
using MetroidMod.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace MetroidMod.Content.Projectiles
{
	internal class BeamShot2 : ModProjectile
	{
		public int[] VisualWinners = [-1, -1];
		public ModBeamAddon[] beamAddons = new ModBeamAddon[BeamAddonSlotID.Count]; 
		public float beamScale = 0.75f;

		public bool canPhase = true;
		public int pierceNumber = 1;
		public override string Texture => $"{nameof(MetroidMod)}/Assets/Textures/BeamAddons/PowerBeam/Shot";
		Color color = MetroidMod.powColor;
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.scale = 0.75f;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = 1;
			Projectile.DamageType = ModContent.GetInstance<HunterDamageClass>();

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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			base.OnHitNPC(target, hit, damageDone);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (VisualWinners[0] == -1 || VisualWinners[1] == -1 || beamAddons == null){ return true; }
			ModBeamAddon beamShape = beamAddons[VisualWinners[0]];
			ModBeamAddon beamColor = beamAddons[VisualWinners[1]];
			Texture2D beamTex = (ModContent.Request<Texture2D>(beamShape.ShotTexture).Value);
			lightColor = beamColor.ShotColor;
			Main.EntitySpriteDraw(beamTex, Projectile.position, null, beamColor.ShotColor, Projectile.rotation, 
								  new Vector2(beamTex.Width / 2, beamTex.Height / 2), beamScale, SpriteEffects.None);
			return false;
		}
	}
}
