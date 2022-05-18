﻿using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MetroidModPorted.Content.MorphBallAddons
{
	public class PumpkinBomb : ModMBWeapon
	{
		public override string ItemTexture => $"{Mod.Name}/Assets/Textures/MBAddons/PumpkinBomb/PumpkinBombItem";

		public override string TileTexture => $"{Mod.Name}/Assets/Textures/MBAddons/PumpkinBomb/PumpkinBombTile";

		public override string ProjectileTexture => $"{Mod.Name}/Assets/Textures/MBAddons/PumpkinBomb/PumpkinBombProjectile";

		public override bool AddOnlyAddonItem => false;

		public override bool CanGenerateOnChozoStatue(Tile tile) => true;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pumpkin Morph Ball Bombs");
			Tooltip.SetDefault("-Right click to set off a bomb\n" +
			"Fires off Jack 'O Lanterns on detonation\n" +
			"'I took a grenade to the face, dude!'");
			ItemNameLiteral = true;
		}
		public override void SetItemDefaults(Item item)
		{
			item.damage = 75;
			item.value = Terraria.Item.buyPrice(0, 5, 0, 0);
			item.rare = ItemRarityID.Yellow;
		}

		public override void Kill(int timeLeft, ref int dustType, ref int dustType2, ref float dustScale, ref float dustScale2)
		{
			dustType = 6;
			dustType2 = 6;

			Projectile projectile = Projectile.Projectile;

			int max = 3;
			float angle = Main.rand.Next(360 / max);
			for (int i = 0; i < max; i++)
			{
				float rot = (float)Angle.ConvertToRadians(angle + ((360f / max) * i));
				Vector2 vel = rot.ToRotationVector2() * 5f;
				Projectile proj = Main.projectile[Terraria.Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vel, ProjectileID.JackOLantern, projectile.damage / max, projectile.knockBack + 3, projectile.owner)];
				proj.timeLeft = 60;
			}
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile projectile = Projectile.Projectile;

			// Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.DirectionTo(target.Center) * 8, ProjectileID.FlamingJack, (int)(damage * 1.5f), knockback + 3, projectile.owner, target.whoAmI);
		}
	}
}
