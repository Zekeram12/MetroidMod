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
using MetroidMod.Content.Projectiles.Paralyzer;
using MetroidMod.Default;
using MetroidMod.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
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


		//<><><><><><><><><>[[[BEAM STATS]]]<><><><><><><><><>
		/// <summary>
		/// The Power Beam's base damage, before accounting for addon multipliers.
		/// </summary>
		int baseDamage = 10;
		/// <summary>
		/// The Power Beam's total additional base damage from installed addons <br/>
		/// NOTE: baseDamageBonus is applied BEFORE damageMult, which means that it effects the base that damageMult multiplies off of.
		/// </summary>
		int baseDamageBonus = 0; //I'm not actually sure if I need any of these besides the beam's base stats, but I'll keep em in just in case
		/// <summary>
		/// The Power Beam's total damage multiplier from installed addons.
		/// </summary>
		float damageMult = 0f; //write these as percentages like you'd see them in tooltips, they'll get converted later
		/// <summary>
		/// The Power Beam's base usetime, before accounting for addon multipliers.
		/// </summary>
		int baseSpeed = 10;
		/// <summary>
		/// The Power Beam's total additional base speed from installed addons.
		/// </summary>
		int baseSpeedBonus = 0;
		/// <summary>
		/// The Power Beam's total speed multiplier from installed addons.
		/// </summary>
		float speedMult = 0f;
		/// <summary>
		/// The Power Beam's total base velocity, before accounting for addon multipliers.
		/// </summary>
		float baseVelocity = 16f;
		/// <summary>
		/// The Power Beam's total additional base velocity from installed addons.
		/// </summary>
		float baseVelocityBonus = 0f;
		/// <summary>
		/// The Power Beam's total velocity multiplier from installed addons.
		/// </summary>
		float velocityMult = 0f;
		/// <summary>
		/// The Power Beam's base critical strike chance, before accounting for addon multipliers.
		/// </summary>
		int baseCrit = 3;
		/// <summary>
		/// The Power Beam's total additional base crit chance from installed addons.
		/// </summary>
		int critBonus = 0;
		/// <summary>
		/// The Power Beam's base Overheat use, before accounting for addon multipliers.
		/// </summary>
		int baseOverheat = 4;
		/// <summary>
		/// The Power Beam's total additional base Overheat use from installed addons.
		/// </summary>
		int baseOverheatBonus = 0;
		/// <summary>
		/// The Power Beam's total overheat multiplier from installed addons.
		/// </summary>
		float overheatMult = 1f;
		/// <summary>
		/// The final overheat value, which will be calculated in UpdateInventory.<br/>
		/// It has to be out here because there's no baked-in variable like there is for damage/velocity/whatever
		/// </summary>
		int overheat = 0;
		/// <summary>
		/// The Power Beam's total base ammo count.
		/// </summary>
		int baseAmmo = 0;
		/// <summary>
		/// The Power Beam's total bonus ammo from expansions.
		/// </summary>
		int ammoBonus = 0;
		/// <summary>
		/// The total number of shots the Power Beam will fire.
		/// </summary>
		int shotCount = 1;


		//[[[MISSILE STATS]]]         Worry about this after the beams are done      -Z




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
			Item.damage = baseDamage;
			Item.width = 40;
			Item.height = 20;
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
			return Item.TryGetGlobalItem(out GlobalItem mi) && (player.whoAmI == Main.myPlayer && mp.statOverheat < mp.maxOverheat);
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

			//Adding up all the stat modifiers from installed beam addons
			baseDamageBonus = BeamAddonLoader.BaseDamageStacker(BeamAddonModifier);
			damageMult = BeamAddonLoader.DamageMultStacker(BeamAddonModifier);
			baseSpeedBonus = BeamAddonLoader.BaseSpeedStacker(BeamAddonModifier);
			speedMult = BeamAddonLoader.SpeedMultStacker(BeamAddonModifier);
			baseVelocityBonus = BeamAddonLoader.BaseVelocityStacker(BeamAddonModifier);
			velocityMult = BeamAddonLoader.VelocityMultStacker(BeamAddonModifier);
			critBonus = BeamAddonLoader.CritChanceStacker(BeamAddonModifier);
			baseOverheatBonus = BeamAddonLoader.BaseOverheatStacker(BeamAddonModifier);
			overheatMult = BeamAddonLoader.OverheatMultStacker(BeamAddonModifier);


			//apply the numbers to the weapon
			Item.damage = (int)((baseDamage + baseDamageBonus) * (damageMult/100) + 1); //Formula for power beam base damage calc. Has to convert to int to work
			Item.useTime = (int)((baseSpeed + baseSpeedBonus) * (speedMult/100) + 1); //Usetime calc. Note that the mult is being divided by 100
			Item.shootSpeed = ((baseVelocity + baseVelocityBonus) * (velocityMult/100) + 1); //Velocity calc. It adds 1 so the values can be easy to read
			Item.crit = baseCrit + critBonus;
			overheat = (int)((baseOverheat + baseOverheatBonus) * (overheatMult / 100) + 1);

		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			MPlayer mp = player.GetModPlayer<MPlayer>(); //finds the current player's MPlayer data for later modification
			
			int[] VisualDinners = BeamAddonLoader.VisualPriority(BeamAddonModifier); //VisualDinners[0] is the winning ShapePriority, VisualDinners[1] is the winning ColorPriority
			if (VisualDinners[0] == -1 || VisualDinners[1] == -1) { return false; } //If either value is -1 that either means something's fucky or there's no addons installed, meaning just go with default values
			Vector2 oPos = player.RotatedRelativePoint(player.MountedCenter, true);
			float speedX = velocity.X;
			float speedY = velocity.Y;
			BeamShot2 beam = (Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI).ModProjectile) as BeamShot2;
			beam.VisualWinners = VisualDinners;
			
			mp.statOverheat += MGlobalItem.AmmoUsage(player, overheat * mp.overheatCost);
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
