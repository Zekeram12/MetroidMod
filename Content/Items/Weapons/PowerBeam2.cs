using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MetroidMod.Common.GlobalItems;
using MetroidMod.Common.Players;
using MetroidMod.Content.DamageClasses;
using MetroidMod.Content.Projectiles;
using MetroidMod.Content.Projectiles.missiles;
using MetroidMod.Content.Projectiles.Paralyzer;
using MetroidMod.Default;
using MetroidMod.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MetroidMod.Content.Items.Weapons
{
	internal class PowerBeam2 : ModItem
	{
		#region Beam and Missile addon storage

		//[Power Beam addons]
		/// <summary>
		/// The array in which active beam addons are stored.
		/// </summary>
		private Item[] beamAddons;
		/// <summary>
		/// Used to access the contents of the beam addon array.<br/>
		/// Needed because quote: "Something something data security"
		/// </summary>
		public Item[] BeamAddonAccess
		{
			get {
				if (beamAddons == null) //This is a failsafe; if the array comes up null, reset the array
				{
					beamAddons = new Item[BeamAddonSlotID.Count]; //iterate through all slots of the array
					for (int i = 0; i < beamAddons.Length; ++i)
					{
						beamAddons[i] = new Item();
						beamAddons[i].TurnToAir();
					}
				}
				return beamAddons;
			}
		}
		/// <summary>
		/// Used to access the stats of beam addons to modify the Power Beam.
		/// </summary>
		public ModBeamAddon[] BeamAddonModifier
		{
			get {
				ModBeamAddon[] addons = BeamAddonAccess //Creates a version of BeamAddonAccess that can be fed into the visual priority system
				.Select(i => BeamAddonLoader.GetAddon(i))
				.Where(i => i != null)
				.ToArray();
				return addons;
			}
		}

		/// <summary>
		/// The array in which secondary charge addons are stored.<br/>
		/// </summary>
		private Item[] chargeQuickSwap;
		/// <summary>
		/// Used to access the contents of the beam array array.
		/// </summary>
		public Item[] ChargeQuickSwapAccess
		{
			get {
				if (chargeQuickSwap == null)
				{
					chargeQuickSwap = new Item[1]; //Array is dynamic, so reset to one Item long and turn that Item into air
					chargeQuickSwap[0] = new Item();
					chargeQuickSwap[0].TurnToAir();
				}
				return chargeQuickSwap;
			}
		}
		/// <summary>
		/// Used to access the stats of the quick swap addons to modify the Power Beam.
		/// </summary>
		public ModBeamAddon[] ChargeQuickSwapModifier
		{
			get {
				ModBeamAddon[] addons = ChargeQuickSwapAccess //Creates a version of BeamAddonAccess that can be fed into the visual priority system
				.Select(i => BeamAddonLoader.GetAddon(i))
				//.Where(i => i != null)
				.ToArray();
				return addons;
			}
		}

		//[Missile Launcher addons]
		/// <summary>
		/// The array in which active missile addons are stored.
		/// </summary>
		private Item[] missileAddons;
		/// <summary>
		/// Used to access the contents of the missile addon array.<br/>
		/// Needed because quote: "Something something data security"
		/// </summary>
		public Item[] MissileAddonAccess
		{
			get {
				if(missileAddons == null) //see BeamAddonAccess above
				{
					missileAddons = new Item[MissileAddonSlotID.Count];
					for (int i = 0; i < missileAddons.Length; ++i)
					{
						missileAddons[i] = new Item();
						missileAddons[i].TurnToAir();
					}
				}
				return missileAddons;
			}
		}
		public ModMissileAddon[] MissileAddonModifier
		{
			get {
				ModMissileAddon[] addons = MissileAddonAccess
					.Select(i => MissileAddonLoader.GetAddon(i))
					.ToArray();
				return addons;
			}
		}

		/// <summary>
		/// The array in which secondary charge combos are stored.
		/// </summary>
		private Item[] comboQuickChange;
		/// <summary>
		/// Used to access the contents of the combo quick change array.
		/// </summary>
		public Item[] ComboQuickChangeAccess
		{
			get { 
				if(comboQuickChange == null) //See BeamArrayAccess above
				{
					comboQuickChange = new Item[1];
					comboQuickChange[0] = new Item();
					comboQuickChange[0].TurnToAir();
				}
				return comboQuickChange;
			}
		}
		public ModMissileAddon[] ComboQuickChangeModifier
		{
			get {
				ModMissileAddon[] addons = ComboQuickChangeAccess
					.Select(i => MissileAddonLoader.GetAddon(i))
					.ToArray();
				return addons;
			}
		}
		#endregion


		#region Power Beam stats
		/// <summary>
		/// The Power Beam's base damage, before accounting for addon multipliers.
		/// </summary>
		int BeamBaseDamage = 10;
		/// <summary>
		/// The Power Beam's total additional base damage from installed addons <br/>
		/// NOTE: baseDamageBonus is applied BEFORE damageMult, which means that it effects the base that damageMult multiplies off of.
		/// </summary>
		int BeamBaseDamageBonus = 0; //I'm not actually sure if I need any of these besides the beam's base stats, but I'll keep em in just in case
		/// <summary>
		/// The Power Beam's total damage multiplier from installed addons.
		/// </summary>
		float BeamDamageMult = 0f; //write these as percentages like you'd see them in tooltips, they'll get converted later
		/// <summary>
		/// The Power Beam's base usetime, before accounting for addon multipliers.
		/// </summary>
		int BeamBaseSpeed = 8;
		/// <summary>
		/// The Power Beam's total additional base speed from installed addons.
		/// </summary>
		int BeamBaseSpeedBonus = 0;
		/// <summary>
		/// The Power Beam's total speed multiplier from installed addons.
		/// </summary>
		float BeamSpeedMult = 0f;
		/// <summary>
		/// The Power Beam's total base velocity, before accounting for addon multipliers.
		/// </summary>
		float BeamBaseVelocity = 24f;
		/// <summary>
		/// The Power Beam's total additional base velocity from installed addons.
		/// </summary>
		float BeamBaseVelocityBonus = 0f;
		/// <summary>
		/// The Power Beam's total velocity multiplier from installed addons.
		/// </summary>
		float BeamVelocityMult = 0f;
		/// <summary>
		/// The Power Beam's base critical strike chance, before accounting for addon multipliers.
		/// </summary>
		int BeamBaseCrit = 3;
		/// <summary>
		/// The Power Beam's total additional base crit chance from installed addons.
		/// </summary>
		int BeamCritBonus = 0;
		/// <summary>
		/// The Power Beam's base Overheat use, before accounting for addon multipliers.
		/// </summary>
		int BaseOverheat = 4;
		/// <summary>
		/// The Power Beam's total additional base Overheat use from installed addons.
		/// </summary>
		int BaseOverheatBonus = 0;
		/// <summary>
		/// The Power Beam's total overheat multiplier from installed addons.
		/// </summary>
		float OverheatMult = 0f;
		/// <summary>
		/// The final overheat value, which will be calculated in UpdateInventory.<br/>
		/// It has to be out here because there's no baked-in variable like there is for damage/velocity/whatever
		/// </summary>
		int Overheat = 0;
		/// <summary>
		/// The total number of shots the Power Beam will fire.
		/// </summary>
		int ShotCount = 0;
		#endregion


		#region Missile Launcher stats
		/// <summary>
		/// The Missile Launcher's base damage, before accounting for addons.
		/// </summary>
		int MissileBaseDamage = 32;
		/// <summary>
		/// The Missile Launcher's total damage multiplier from addons.
		/// </summary>
		float MissileDamageMult = 0f;
		/// <summary>
		/// The Missile Launcher's base usetime, before accounting for addons.
		/// </summary>
		int MissileBaseSpeed = 18;
		/// <summary>
		/// The Missile Launcher's total speed multiplier from addons.
		/// </summary>
		float MissileSpeedMult = 0f;
		/// <summary>
		/// The Missile Launcher's base velocity, before accounting for addons.
		/// </summary>
		int MissileBaseVelocity = 50;
		/// <summary>
		/// The Missile Launcher's total velocity multiplier from addons.
		/// </summary>
		float MissileVelocityMult = 0f;
		/// <summary>
		/// The Missile Launcher's base critical strike chance, before accounting for addons.
		/// </summary>
		int MissileBaseCrit = 3;
		/// <summary>
		/// The Missile Launcher's base Charge Combo cost, before accounting for addons.
		/// </summary>
		int BaseComboCost = 10;
		// I was gonna have just as many stats as the PB in here but
		// there's really only one missile addon that affects your base projectile
		// it really didn't end up being necessary I think
		#endregion

		/// <summary>
		/// The sound effect a normal shot will use.
		/// </summary>
		public SoundStyle? ShotSound;
		/// <summary>
		/// The sound effect a charged shot will use.
		/// </summary>
		public SoundStyle? ChargeShotSound;

		public override void SetStaticDefaults()
		{
			//Below is how display text worked before localization hjsons
			//Their introduction made these obsolete but I'm keeping this here for posterity :)        -Z
			/* DisplayName.SetDefault("Power Beam");
			   Tooltip.SetDefault("Select this item in your hotbar and open your inventory to open the Beam Addon UI");*/
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults() //obviously stats are set here
		{
			MGlobalItem ac = Item.GetGlobalItem<MGlobalItem>();
			Item.width = 40;
			Item.height = 20;
			Item.DamageType = ModContent.GetInstance<HunterDamageClass>();
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.value = 6969;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = ShotSound;
			ac.showChargeBar = true; //lets the charge UI show up
			ac.showOnHand = true;

			if (ac.isBeam) //Power Beam default stats
			{
				Item.damage = BeamBaseDamage;
				Item.useTime = BeamBaseSpeed;
				Item.useAnimation = BeamBaseSpeed;
				Item.shoot = ModContent.ProjectileType<BeamShot2>(); //Most of the cool shit happens on the projectile itself
				Item.shootSpeed = BeamBaseVelocity;
				Item.crit = BeamBaseCrit;
			}
			else //Missile Launcher default stats
			{
				Item.damage = MissileBaseDamage;
				Item.useTime = MissileBaseSpeed;
				Item.useAnimation = MissileBaseSpeed;
				Item.shoot = ModContent.ProjectileType<MissileShot>(); //Most of the cool shit happens on the projectile itself
				Item.shootSpeed = MissileBaseVelocity;
				Item.crit = MissileBaseCrit;
			}
			
		}
		public override void UseStyle(Player player, Rectangle heldItemFrame) //makes the player's arm rotate with the arm cannon
		{
			Item.TryGetGlobalItem(out MGlobalItem mi);
			float armRot = player.itemRotation - (float)(Math.PI / 2) * player.direction;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
			Vector2 origin = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armRot);
			origin.Y -= heldItemFrame.Height / 2f;
			player.itemLocation = origin + player.itemRotation.ToRotationVector2() * (mi.isBeam ? -16 : -14) * player.direction;
		}

		public override bool CanUseItem(Player player) //lets things properly restrict your ability to use the weapon
		{
			MPlayer mp = player.GetModPlayer<MPlayer>();
			return (player.whoAmI == Main.myPlayer && mp.statOverheat < mp.maxOverheat);
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

			#region Power Beam stat calculation
			//Adding up all the stat modifiers from installed beam addons
			BeamBaseDamageBonus = BeamAddonLoader.BaseDamageStacker(BeamAddonModifier);
			BeamDamageMult = BeamAddonLoader.DamageMultStacker(BeamAddonModifier);
			BeamBaseSpeedBonus = BeamAddonLoader.BaseSpeedStacker(BeamAddonModifier);
			BeamSpeedMult = BeamAddonLoader.SpeedMultStacker(BeamAddonModifier);
			BeamBaseVelocityBonus = BeamAddonLoader.BaseVelocityStacker(BeamAddonModifier);
			BeamVelocityMult = BeamAddonLoader.VelocityMultStacker(BeamAddonModifier);
			BeamCritBonus = BeamAddonLoader.CritChanceStacker(BeamAddonModifier);
			BaseOverheatBonus = BeamAddonLoader.BaseOverheatStacker(BeamAddonModifier);
			OverheatMult = BeamAddonLoader.OverheatMultStacker(BeamAddonModifier);
			#endregion


			//apply the numbers to the weapon
			if (ac.isBeam) //apply to power beam
			{
				Item.damage = (int)((BeamBaseDamage + BeamBaseDamageBonus) * ((BeamDamageMult / 100) + 1)); //Formula for power beam base damage calc. Has to convert to int to work
				Item.useTime = (int)((BeamBaseSpeed + BeamBaseSpeedBonus) * ((BeamSpeedMult / 100) + 1)); //Usetime calc. Note that the mult is being divided by 100
				Item.shootSpeed = ((BeamBaseVelocity + BeamBaseVelocityBonus) * ((BeamVelocityMult / 100) + 1)); //Velocity calc. It adds 1 so the values can be easy to read
				Item.crit = BeamBaseCrit + BeamCritBonus;
				Overheat = (int)((BaseOverheat + BaseOverheatBonus) * ((OverheatMult / 100) + 1));
			}
			else //apply to missile
			{
				Item.damage = 69;
			}

		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			MPlayer mp = player.GetModPlayer<MPlayer>(); //finds the current player's MPlayer data for later modification
			
			int[] VisualDinners = BeamAddonLoader.VisualPriority(BeamAddonModifier); //VisualDinners[0] is the winning ShapePriority, VisualDinners[1] is the winning ColorPriority
			//if (VisualDinners[0] == -1 || VisualDinners[1] == -1) { return false; } //If either value is -1 that either means something's fucky or there's no addons installed, meaning just go with default values
			Vector2 oPos = player.RotatedRelativePoint(player.MountedCenter, true);
			float speedX = velocity.X;
			float speedY = velocity.Y;
			BeamShot2 beam = (Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI).ModProjectile) as BeamShot2;
			beam.VisualWinners = VisualDinners;
			beam.beamAddons = BeamAddonModifier;
			
			//mp.statOverheat += MGlobalItem.AmmoUsage(player, overheat * mp.overheatCost);
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient<Miscellaneous.ChoziteBar>(8)
				.AddIngredient<Tiles.MissileExpansion>(1)
				.AddIngredient<Miscellaneous.EnergyShard>(3)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
