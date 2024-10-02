using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroidMod.Common.GlobalItems;
using MetroidMod.Common.Players;
using MetroidMod.Content.DamageClasses;
using MetroidMod.Content.Projectiles;
using MetroidMod.Content.Projectiles.Paralyzer;
using MetroidMod.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MetroidMod.Content.Items.Weapons
{
	internal class PowerBeam2 : ModItem
	{
		//<><><><><><><><><>[[[ADDON STORAGE]]]<><><><><><><><><>

		//[Power Beam addons]
		/// <summary>
		/// The array in which active beam addons are stored.
		/// </summary>
		private Item[] BeamAddons;
		/// <summary>
		/// Used to access the contents of the beam addon array.<br/>
		/// Needed because quote: "Something something data security"
		/// </summary>
		public Item[] BeamAddonAccess
		{
			get {
				if (BeamAddons == null) //This is a failsafe; if the array comes up null, reset the array
				{
					BeamAddons = new Item[BeamAddonSlotID.Count]; //iterate through all slots of the array
					for (int i = 0; i < BeamAddons.Length; ++i)
					{
						BeamAddons[i] = new Item();
						BeamAddons[i].TurnToAir();
					}
				}
				return BeamAddons;
			}
		}
		/// <summary>
		/// The array in which secondary charge addons are stored.<br/>
		/// TODO: Give this a better name later       -Z
		/// </summary>
		private Item[] BeamArray;
		/// <summary>
		/// Used to access the contents of the beam array array.<br/>
		/// ...Yeah do you see why I need to come up with a better name now?  -Z
		/// </summary>
		public Item[] BeamArrayAccess
		{
			get {
				if (BeamArray == null)
				{
					BeamArray = new Item[1]; //Array is dynamic, so reset to one Item long and turn that Item into air
					BeamArray[0] = new Item();
					BeamArray[0].TurnToAir();
				}
				return BeamArray;
			}
		}

		//[Missile Launcher addons]
		/// <summary>
		/// The array in which active missile addons are stored.
		/// </summary>
		private Item[] MissileAddons;
		/// <summary>
		/// Used to access the contents of the missile addon array.<br/>
		/// Needed because quote: "Something something data security"
		/// </summary>
		public Item[] MissileAddonAccess
		{
			get {
				if(MissileAddons == null) //see BeamAddonAccess above
				{
					MissileAddons = new Item[MissileAddonSlotID.Count];
					for (int i = 0; i < MissileAddons.Length; ++i)
					{
						MissileAddons[i] = new Item();
						MissileAddons[i].TurnToAir();
					}
				}
				return MissileAddons;
			}
		}
		/// <summary>
		/// The array in which secondary charge combos are stored. <br/>
		/// TODO: Give this a better name           -Z
		/// </summary>
		private Item[] MissileArray;
		/// <summary>
		/// Used to access the contents of the missile array array.<br/>
		/// ...Yeah do you see why I need to come up with a better name now?  -Z
		/// </summary>
		public Item[] MissileArrayAccess
		{
			get { 
				if(MissileArray == null) //See BeamArrayAccess above
				{
					MissileArray = new Item[1];
					MissileArray[0] = new Item();
					MissileArray[0].TurnToAir();
				}
				return MissileArray;
			}
		}


		//<><><><><><><><><>[[[BEAM STATS]]]<><><><><><><><><>
		/// <summary>
		/// The Power Beam's base damage, before accounting for addon multipliers.
		/// </summary>
		int baseDamage = 10;
		/// <summary>
		/// The Power Beam's total additional base damage from installed addons <br/>
		/// NOTE: baseDamageBonus is applied BEFORE damageMult, which means that it effects the base that damageMult multiplies off of.
		/// </summary>
		int baseDamageBonus = 0;
		/// <summary>
		/// The Power Beam's total damage multiplier from installed addons.
		/// </summary>
		float damageMult = 1f;
		/// <summary>
		/// The Power Beam's base usetime, before accounting for addon multipliers.
		/// </summary>
		int baseSpeed = 10;
		/// <summary>
		/// The Power Beam's total speed multiplier from installed addons.
		/// </summary>
		float speedMult = 1f;
		/// <summary>
		/// The Power Beam's total base velocity, before accounting for addon multipliers.
		/// </summary>
		float baseVelocity = 16f;
		/// <summary>
		/// The Power Beam's total velocity multiplier from installed addons.
		/// </summary>
		float velocityMultiplier = 1f;
		/// <summary>
		/// The Power Beam's base critical strike chance, before accounting for addon multipliers.
		/// </summary>
		int baseCrit = 3;
		/// <summary>
		/// The Power Beam's total crit chance multiplier from installed addons.
		/// NOTE: I'll be honest I dunno if this is how that works yet. I'll figure it out later.    -Z
		/// </summary>
		float critMultiplier = 1f;
		/// <summary>
		/// The Power Beam's base Overheat use, before accounting for addon multipliers.
		/// </summary>
		int baseOverheat = 4;
		/// <summary>
		/// The Power Beam's total overheat multiplier from installed addons.
		/// </summary>
		float overheatMultiplier = 1f;
		//NOTE: I'll figure out UA shit later      -Z


		//[[[MISSILE STATS]]]         Worry about this after the beams are done      -Z




		public override void SetStaticDefaults()
		{
			//Below is how display text worked before localization hjsons
			//Their introduction made these obsolete but I'm keeping this here for posterity :)        -Z
			/* DisplayName.SetDefault("Power Beam");
			   Tooltip.SetDefault("Select this item in your hotbar and open your inventory to open the Beam Addon UI");*/
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = baseDamage;
			Item.width = 16;
			Item.height = 12;
			Item.DamageType = ModContent.GetInstance<HunterDamageClass>();
			Item.useTime = baseSpeed;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.value = 6969;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = Sounds.Items.Weapons.PowerBeamSound;
			Item.shoot = ModContent.ProjectileType<BeamShot2>(); //Most of the cool shit happens on the projectile itself
			Item.shootSpeed = baseVelocity;
			Item.crit = baseCrit;
		}
		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			Item.TryGetGlobalItem(out MGlobalItem mi);
			float armRot = player.itemRotation - (float)(Math.PI / 2) * player.direction;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
			Vector2 origin = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armRot);
			origin.Y -= heldItemFrame.Height / 2f;
			player.itemLocation = origin + player.itemRotation.ToRotationVector2() * (mi.isBeam ? -16 : -14) * player.direction;
		}

		public override bool PreDrawInWorld(SpriteBatch sb, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (Item == null || !Item.TryGetGlobalItem(out MGlobalItem mi)) { return true; }
			Texture2D tex = Terraria.GameContent.TextureAssets.Item[Type].Value;
				tex = mi.itemTexture;
			float num5 = Item.height - tex.Height;
			float num6 = Item.width / 2 - tex.Width / 2;
			sb.Draw(tex, new Vector2(Item.position.X - Main.screenPosition.X + (tex.Width / 2) + num6, Item.position.Y - Main.screenPosition.Y + (tex.Height / 2) + num5 + 2f),
			new Rectangle?(new Rectangle(0, 0, tex.Width, tex.Height)), alphaColor, rotation, new Vector2(tex.Width / 2, (tex.Height / 2)), scale, SpriteEffects.None, 0f);
			return false;
		}

		//"This is where the fun begins" -Anakin Skywalker
		public override void UpdateInventory(Player p)
		{
			MPlayer mp = p.GetModPlayer<MPlayer>(); //finds the current player's MPlayer data for later modification
			if (Item == null || !Item.TryGetGlobalItem(out MGlobalItem ac)) { return; }

		}
	}
}
