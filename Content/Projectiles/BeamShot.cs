﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroidMod.Content.DamageClasses;
using MetroidMod.ID;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MetroidMod.Content.Projectiles
{
	internal class BeamShot : MProjectile
	{
		/// <summary>
		/// Stores the winning slot numbers from the Visual Priority check.
		/// </summary>
		public int[] VisualWinners = [-1, -1, 0, 0];
		public ModBeamAddon[] beamAddons = new ModBeamAddon[BeamAddonSlotID.Count]; 
		public float beamScale = 0.75f;
		public int beamDust = DustID.YellowTorch;
		public string textureMod;
		public string nameChanger;

		public bool canPhase = false;
		/// <summary>
		/// The amount of tile interactions the shot can perform before dying.
		/// </summary>
		public int TileInteract = 0;
		/// <summary>
		/// The amount of entity interactions the shot can perform before dying.
		/// </summary>
		public int EntityInteract = 0;
		public int ShotNumber = 0;
		public bool charged = false;
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
		//public override bool PreAI()
		//{
		//	return true;
		//}
		public override void AI() //TODO: make a whole-ass thing
		{
			Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;
			Lighting.AddLight(Projectile.Center, color.R / 255f, color.G / 255f, color.B / 255f);

			//Put the dustline shit here later

			if (Projectile.numUpdates == 0)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, beamDust, 0, 0, 100, default(Color), Projectile.scale);
				Main.dust[dust].noGravity = true;
			}
		}
		//public override void PostAI()
		//{
		//	base.PostAI();
		//}
		public override void OnKill(int timeLeft)
		{
			Vector2 pos = Projectile.position;
			
			//Copied from MProjectile.DustyDeath()
			int freq = 20;
			bool noGravity = true;
			for (int i = 0; i < freq; i++)
			{
				int dust = Dust.NewDust(pos, Projectile.width, Projectile.height, beamDust, 0, 0, 100, color, Projectile.scale * beamScale);
				Main.dust[dust].velocity = new Vector2((Main.rand.Next(freq) - (freq / 2)) * 0.125f, (Main.rand.Next(freq) - (freq / 2)) * 0.125f);
				Main.dust[dust].noGravity = noGravity;
			}
			if (VisualWinners[0] != -1)
			{
				//TODO: Add an exception-catcher for if there's no asset found, make it default to the normal impact sound          -Z
				if (VisualWinners[3] == 1)
				{ SoundStyle sound = new(beamAddons[VisualWinners[1]].ImpactSound); SoundEngine.PlaySound(sound, Projectile.Center); }
				else { SoundStyle sound = new(beamAddons[VisualWinners[0]].ImpactSound); SoundEngine.PlaySound(sound, Projectile.Center); }
			}
		}

		//public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		//{
		//	//inject onhitnpc code here
		//	base.OnHitNPC(target, hit, damageDone);
		//}
		//public override void OnHitPlayer(Player target, Player.HurtInfo info)
		//{
		//	//Could do some cool shit here.
		//	base.OnHitPlayer(target, info);
		//}

		public override bool PreDraw(ref Color lightColor)
		{
			if (VisualWinners[0] == -1 || VisualWinners[1] == -1 || beamAddons == null){ return true; }
			ModBeamAddon beamShape = beamAddons[VisualWinners[0]];
			ModBeamAddon beamColor = beamAddons[VisualWinners[1]];
			Texture2D beamTex;
			MetroidMod.Instance.Logger.Info(" Projectile renderin' time.\nTexture path: " + beamShape.ShotTexture);
			if (charged) { beamTex = (ModContent.Request<Texture2D>(beamShape.ChargeShotTexture).Value); }
			else { beamTex = (ModContent.Request<Texture2D>(beamShape.ShotTexture).Value); }
			lightColor = beamColor.ShotColor;
			beamDust = beamColor.ShotDust;
			Main.EntitySpriteDraw(beamTex, Projectile.Center - Main.screenPosition, null, beamColor.ShotColor, Projectile.rotation, 
								  new Vector2(beamTex.Width / 2, beamTex.Height / 2), beamScale, SpriteEffects.None);
			return false;
		}
	}
}
