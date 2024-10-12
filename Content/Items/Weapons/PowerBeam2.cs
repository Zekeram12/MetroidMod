using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MetroidMod.Common.GlobalItems;
using MetroidMod.Common.Players;
using MetroidMod.Common.Systems;
using MetroidMod.Content.DamageClasses;
using MetroidMod.Content.Projectiles;
using MetroidMod.Content.Projectiles.missiles;
using MetroidMod.Content.Projectiles.Paralyzer;
using MetroidMod.Default;
using MetroidMod.ID;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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
			set { beamAddons = value; }
		}
		/// <summary>
		/// Used to access the stats of beam addons to modify the Power Beam.
		/// </summary>

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
					chargeQuickSwap = new Item[8]; //Array is dynamic, so reset to one Item long and turn that Item into air
					for(int i = 0;i < chargeQuickSwap.Length; ++i)
					{
						chargeQuickSwap[i] = new Item();
						chargeQuickSwap[i].TurnToAir();
					}
				}
				return chargeQuickSwap;
			}
			set { chargeQuickSwap = value; }
		}
		/// <summary>
		/// Used to access the stats of the quick swap addons to modify the Power Beam.
		/// </summary>

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
			set { missileAddons = value; }
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
					comboQuickChange = new Item[8];
					for (int i = 0;i < comboQuickChange.Length; ++i)
					{
						comboQuickChange[i] = new Item();
						comboQuickChange[i].TurnToAir();
					}
				}
				return comboQuickChange;
			}
			set { comboQuickChange = value; }
		}

		/// <summary>
		/// Keeps track of the addons that were selected by the <b>Visual Priority System</b>.
		/// <br/><br/><i>(VisualWinners was taken by <see cref="BeamShot2"/>)</i>
		/// </summary>
		public int[] VisualDinners;

		public SoundStyle beamSound = Sounds.Items.Weapons.PowerBeamSound;
		public SoundStyle missileSound = Sounds.Items.Weapons.MissileSound;
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
		int BeamBaseSpeed = 14;
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
			if (VisualDinners == null)
			{
				VisualDinners = new int[2]; //it's a surprise tool that'll help us later
				VisualDinners = [-1, -1];
			}
			Item.width = 40;
			Item.height = 20;
			Item.DamageType = ModContent.GetInstance<HunterDamageClass>();
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.value = 6969;
			Item.rare = ItemRarityID.Green;

			if (ac.isBeam) //Power Beam default stats
			{
				Item.damage = BeamBaseDamage;
				Item.useTime = BeamBaseSpeed;
				Item.useAnimation = BeamBaseSpeed;
				Item.UseSound = beamSound;
				Item.shoot = ModContent.ProjectileType<BeamShot2>(); //Most of the cool shit happens on the projectile itself
				Item.shootSpeed = BeamBaseVelocity;
				Item.crit = BeamBaseCrit;
			}
			else //Missile Launcher default stats
			{
				Item.damage = MissileBaseDamage;
				Item.useTime = MissileBaseSpeed;
				Item.useAnimation = MissileBaseSpeed;
				Item.UseSound = missileSound;
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
		#region Item visual methods
		private void SetTexture(MGlobalItem ac)
		{
			if (!ac.isBeam)
			{
				ac.itemTexture = ModContent.Request<Texture2D>(Texture + "Missile").Value;
			}
			else { ac.itemTexture = ac.itemTexture = ModContent.Request<Texture2D>(Texture).Value; }
		}

		public override bool PreDrawInWorld(SpriteBatch sb, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (Item == null || !Item.TryGetGlobalItem(out MGlobalItem ac)) { return true; }
			Texture2D tex = Terraria.GameContent.TextureAssets.Item[Type].Value;
			SetTexture(ac);
			if (ac.itemTexture != null)
			{
				tex = ac.itemTexture;
			}
			float num5 = Item.height - tex.Height;
			float num6 = Item.width / 2 - tex.Width / 2;
			sb.Draw(tex, new Vector2(Item.position.X - Main.screenPosition.X + (tex.Width / 2) + num6, Item.position.Y - Main.screenPosition.Y + (tex.Height / 2) + num5 + 2f),
			new Rectangle?(new Rectangle(0, 0, tex.Width, tex.Height)), alphaColor, rotation, new Vector2(tex.Width / 2, (tex.Height / 2)), scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (Item == null || !Item.TryGetGlobalItem(out MGlobalItem ac)) { return true; }
			Texture2D tex = Terraria.GameContent.TextureAssets.Item[Type].Value;
			SetTexture(ac);
			if (ac.itemTexture != null)
			{
				tex = ac.itemTexture;
			}
			spriteBatch.Draw(tex, new Vector2(position.X + 2f, position.Y), new Rectangle?(new Rectangle(0, 0, tex.Width, tex.Height)), drawColor, 0f, origin, scale + 0.2f, SpriteEffects.None, 0f);
			return false;
		}
		#endregion

		//"This is where the fun begins" -Anakin Skywalker
		#region The juicy stuff
		public override void UpdateInventory(Player p)
		{
			MPlayer mp = p.GetModPlayer<MPlayer>(); //finds the current player's MPlayer data for later modification
			if (Item == null || !Item.TryGetGlobalItem(out MGlobalItem ac)) { return; }

			#region Power Beam stat calculation
			//Adding up all the stat modifiers from installed beam addons
			BeamBaseDamageBonus = BeamAddonLoader.BaseDamageStacker(BeamAddonAccess);
			BeamDamageMult = BeamAddonLoader.DamageMultStacker(BeamAddonAccess);
			BeamBaseSpeedBonus = BeamAddonLoader.BaseSpeedStacker(BeamAddonAccess);
			BeamSpeedMult = BeamAddonLoader.SpeedMultStacker(BeamAddonAccess);
			BeamBaseVelocityBonus = BeamAddonLoader.BaseVelocityStacker(BeamAddonAccess);
			BeamVelocityMult = BeamAddonLoader.VelocityMultStacker(BeamAddonAccess);
			BeamCritBonus = BeamAddonLoader.CritChanceStacker(BeamAddonAccess);
			BaseOverheatBonus = BeamAddonLoader.BaseOverheatStacker(BeamAddonAccess);
			OverheatMult = BeamAddonLoader.OverheatMultStacker(BeamAddonAccess);
			#endregion


			//apply the numbers to the weapon
			if (ac.isBeam) //apply to power beam
			{
				Item.damage = (int)((BeamBaseDamage + BeamBaseDamageBonus) * ((BeamDamageMult / 100) + 1)); //Formula for power beam base damage calc. Has to convert to int to work
				Item.useAnimation = Item.useTime = (int)((BeamBaseSpeed + BeamBaseSpeedBonus) * ((BeamSpeedMult / 100) + 1)); //Usetime calc. Note that the mult is being divided by 100
				Item.shootSpeed = ((BeamBaseVelocity + BeamBaseVelocityBonus) * ((BeamVelocityMult / 100) + 1)); //Velocity calc. It adds 1 so the values can be easy to read
				Item.crit = BeamBaseCrit + BeamCritBonus;
				Overheat = (int)((BaseOverheat + BaseOverheatBonus) * ((OverheatMult / 100) + 1));
				Item.UseSound = beamSound;
			}
			else //apply to missile
			{
				Item.damage = 69;
				Item.UseSound = missileSound;
			}

		}

		public void ArrayUpdate()
		{
			VisualDinners = BeamAddonLoader.VisualPriority(beamAddons);
			if (VisualDinners[0] != -1)
			{
				beamSound = new SoundStyle(BeamAddonLoader.GetAddon(beamAddons[VisualDinners[0]]).ShotSound);
			}
			else
			{
				beamSound = Sounds.Items.Weapons.PowerBeamSound;
			}
			if (missileAddons != null && !missileAddons[MissileAddonSlotID.Primary].IsAir)
			{
				missileSound = new SoundStyle(MissileAddonLoader.GetAddon(missileAddons[MissileAddonSlotID.Primary]).ShotSound);
			}
			else
			{
				missileSound = Sounds.Items.Weapons.MissileSound;
			}
		}

		public override void HoldItem(Player player)
		{
			MPlayer mp = player.GetModPlayer<MPlayer>(); //finds the current player's MPlayer data for later modification
			MGlobalItem ac = Item.GetGlobalItem<MGlobalItem>();
			ac.showChargeBar = true; //lets the charge UI show up
			ac.showOnHand = true;
			if (MSystem.ACSwitch.JustPressed)
			{
				ac.isBeam = !ac.isBeam;
				SoundEngine.PlaySound(new SoundStyle("MetroidMod/Assets/Sounds/ArmCannon/WeaponSwitch"));
			}
		}

		

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			MPlayer mp = player.GetModPlayer<MPlayer>(); //finds the current player's MPlayer data for later modification
			MGlobalItem ac = Item.GetGlobalItem<MGlobalItem>();
			if (ac.isBeam)
			{
				//VisualDinners[0] is the winning ShapePriority, VisualDinners[1] is the winning ColorPriority
				//if (VisualDinners[0] == -1 || VisualDinners[1] == -1) { return false; } //If either value is -1 that either means something's fucky or there's no addons installed, meaning just go with default values
				Vector2 oPos = player.RotatedRelativePoint(player.MountedCenter, true);
				float speedX = velocity.X;
				float speedY = velocity.Y;
				BeamShot2 beam = (Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI).ModProjectile) as BeamShot2;
				beam.VisualWinners = VisualDinners;

				beam.beamAddons = BeamAddonAccess
					.Select(i => BeamAddonLoader.GetAddon(i))
					.ToArray();

				if (VisualDinners[0] != -1)
				{
					Item.UseSound = new SoundStyle(beam.beamAddons[VisualDinners[0]].ShotSound);
				}
				else
				{
					Item.UseSound = Sounds.Items.Weapons.PowerBeamSound;
				}
				mp.statOverheat += MGlobalItem.AmmoUsage(player, Overheat * mp.overheatCost);
				mp.overheatDelay = (int)Math.Max(Item.useTime - 10, 2);
			}

			return false;
		}
		#endregion
		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient<Miscellaneous.ChoziteBar>(8)
				.AddIngredient<Tiles.MissileExpansion>(1)
				.AddIngredient<Miscellaneous.EnergyShard>(3)
				.AddTile(TileID.Anvils)
				.Register();
		}

		#region Data Preservation
		public override ModItem Clone(Item newEntity)
		{
			//Make sure the clone has all the same addons as the original
			PowerBeam2 clone = (PowerBeam2)base.Clone(newEntity);
			clone.beamAddons = new Item[BeamAddonSlotID.Count];
			clone.chargeQuickSwap = new Item[8];
			clone.missileAddons = new Item[MissileAddonSlotID.Count];
			clone.comboQuickChange = new Item[8];

			#region Beam Addon cloning
			for (int i = 0; i < BeamAddonSlotID.Count; ++i)
			{
				if (beamAddons == null || beamAddons[i] == null)
				{
					clone.beamAddons[i] = new Item();
					clone.beamAddons[i].TurnToAir();
				}
				else { clone.beamAddons[i] = beamAddons[i]; }
			}
			#endregion
			#region Charge Quick-Swap cloning
				for (int i = 0; i < (8); ++i)
				{
					if (chargeQuickSwap == null || chargeQuickSwap[i] == null)
					{
						clone.chargeQuickSwap[i] = new Item();
						clone.chargeQuickSwap[i].TurnToAir();
					}
					else { clone.chargeQuickSwap[i] = chargeQuickSwap[i]; }
				}
			#endregion
			#region Missile Addon cloning
			for (int i = 0; i < MissileAddonSlotID.Count; ++i)
			{
				if (missileAddons == null || missileAddons[i] == null)
				{
					clone.missileAddons[i] = new Item();
					clone.missileAddons[i].TurnToAir();
				}
				else { clone.missileAddons[i] = missileAddons[i]; }
			}
			#endregion
			#region Charge Combo Quick-Swap cloning
				for (int i = 0; (i < 8); ++i)
				{
					if (comboQuickChange == null || comboQuickChange[i] == null)
					{
						clone.comboQuickChange[i] = new Item();
						clone.comboQuickChange[i].TurnToAir();
					}
					else { clone.comboQuickChange[i] = comboQuickChange[i]; }
				}
			#endregion
			return clone;
		}

		public override void OnResearched(bool fullyResearched)
		{
			//If the player researches the arm cannon, puke out all the addons

			foreach (Item item in beamAddons)
			{
				if (item == null || item.IsAir) { continue; }
				IEntitySource itemSource_OpenItem = Main.LocalPlayer.GetSource_OpenItem(Type);
				Main.LocalPlayer.QuickSpawnItem(itemSource_OpenItem, item, item.stack);
			} //beam addons

			foreach (Item item in chargeQuickSwap)
			{
				if (item == null || item.IsAir) { continue; }
				IEntitySource itemSource_OpenItem = Main.LocalPlayer.GetSource_OpenItem(Type);
				Main.LocalPlayer.QuickSpawnItem(itemSource_OpenItem, item, item.stack);
			} //charge quick swap

			foreach (Item item in missileAddons)
			{
				if (item == null || item.IsAir) { continue; }
				IEntitySource itemSource_OpenItem = Main.LocalPlayer.GetSource_OpenItem(Type);
				Main.LocalPlayer.QuickSpawnItem(itemSource_OpenItem, item, item.stack);
			} //missile addons

			foreach (Item item in comboQuickChange)
			{
				if (item == null || item.IsAir) { continue; }
				IEntitySource itemSource_OpenItem = Main.LocalPlayer.GetSource_OpenItem(Type);
				Main.LocalPlayer.QuickSpawnItem(itemSource_OpenItem, item, item.stack);
			} //charge combo quick swap
		}


		public override void SaveData(TagCompound tag)
		{
			#region addons
			//Normal beam addons
			for (int i = 0; i < BeamAddonSlotID.Count; ++i)
			{
				//Failsafe check
				if (beamAddons[i] == null)
				{
					beamAddons[i] = new Item();
				}
				tag.Add("Beam Addon - Slot " + (i + 1), ItemIO.Save(beamAddons[i]));

			}
			//Charge Quick-Swap
			for (int i = 0; i < 8; ++i)
			{
				//Failsafe check
				if (chargeQuickSwap[i] == null)
				{
					chargeQuickSwap[i] = new Item();
				}
				tag.Add("Primary Quick-Swap - Slot " + (i + 1), ItemIO.Save(chargeQuickSwap[i]));
			}
			//Normal missile addons
			for (int i = 0; i < MissileAddonSlotID.Count; ++i)
			{
				//Failsafe check
				if (missileAddons[i] == null)
				{
					missileAddons[i] = new Item();
				}
				tag.Add("Missile Addon - Slot " + (i + 1), ItemIO.Save(missileAddons[i]));
			}
			//Combo Quick-Swap
			for (int i = 0; i < 8; ++i)
			{
				//Failsafe check
				if (comboQuickChange[i] == null)
				{
					comboQuickChange[i] = new Item();
				}
				tag.Add("Charge Combo Quick-Swap - Slot " + (i + 1), ItemIO.Save(comboQuickChange[i]));
			}

			//priority winners
			for (int i = 0; i < 2; ++i)
			{
				tag.Add("Beam Visual Settings - Data Point " + (i + 1), VisualDinners[i]);
			}
			#endregion
			//ammo
			if (Item.TryGetGlobalItem(out MGlobalItem ac))
			{
				tag.Add("Maximum UA", ac.maxUA);
				tag.Add("Current UA", ac.statUA);
				tag.Add("Maximum Missiles", ac.maxMissiles);
				tag.Add("Current Missiles", ac.statMissiles);
			}
		}

		public override void LoadData(TagCompound tag)
		{
			try
			{
				#region addons
				beamAddons = new Item[BeamAddonSlotID.Count];
				for (int i = 0; i < beamAddons.Length; i++)
				{
					Item item = tag.Get<Item>("Beam Addon - Slot " + (i + 1));
					beamAddons[i] = item;
				}
				chargeQuickSwap = new Item[8];
				for (int i = 0; i < 8; i++)
				{
					Item item = tag.Get<Item>("Primary Quick-Swap - Slot " + (i + 1));
					chargeQuickSwap[i] = item;
				}
				missileAddons = new Item[MissileAddonSlotID.Count];
				for (int i = 0; i < missileAddons.Length; i++)
				{
					Item item = tag.Get<Item>("Missile Addon - Slot " + (i + 1));
					missileAddons[i] = item;
				}
				comboQuickChange = new Item[8];
				for (int i = 0; i < 8; i++)
				{
					Item item = tag.Get<Item>("Charge Combo Quick-Swap - Slot " + (i + 1));
					comboQuickChange[i] = item;
				}

				for (int i = 0; i < 2; i++)
				{
					VisualDinners[i] = tag.Get<int>("Beam Visual Settings - Data Point " + (i + 1));
				}
				#endregion
				MGlobalItem ac = Item.GetGlobalItem<MGlobalItem>();
				ac.maxUA = tag.GetInt("Maximum UA");
				ac.statUA = tag.GetFloat("Current UA");
				ac.maxMissiles = tag.GetInt("Maximum Missiles");
				ac.statMissiles = tag.GetInt("Current Missiles");
			}
			catch { }
		}
		#endregion
	}
}
