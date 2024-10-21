﻿using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria;
using MetroidMod.ID;
using MetroidMod.Default;
using Terraria.Audio;
using Terraria.ID;

//gonna document as much of the code as I can to make it easy to follow
namespace MetroidMod
{
	/// <summary>
	/// The base type for all Power Beam addons.<br/><br/>
	/// ModBeamAddons automatically generate a <see cref="Terraria.ModLoader.ModItem"/> and a <see cref="Terraria.ModLoader.ModTile"/> to access the addon in-game.<br/>
	/// Textures are grabbed automatically at this filepath:<br/>
	/// <u>(name of mod)<b>/Assets/Textures/BeamAddons/</b>(name of addon file)<b>/</b>(Item for item sprite, Tile for tile sprite, Shot for shot sprite, etc.)</u><br/>
	/// but can be overriden to point to any filepath. Sounds are also stored this way, just swap Textures for Sounds.<br/><br/>
	/// Every ModBeamAddon needs an <b>AddonSlot</b>, <b>ShapePriority</b>, and <b>ColorPriority</b>.
	/// </summary>
	public abstract class ModBeamAddon : ModType
	{
		/// <summary>
		/// The numerical ID of the addon.<br/>
		/// Pretty much just like how Terraria's items all have a number ID.
		/// </summary>
		public int Type { get; private set; }
		internal void ChangeType(int type) => Type = type;

		/// <summary>
		/// The <see cref="ModItem"/> this addon controls.
		/// </summary>
		public ModItem ModItem;
		/// <summary>
		/// The <see cref="ModTile"/> this addon controls.
		/// </summary>
		public ModTile ModTile;
		/// <summary>
		/// The <see cref="Item"/> this addon controls.
		/// </summary>
		public Item Item => ModItem.Item;
		/// <summary>
		/// References the ModItem previously generated
		/// </summary>
		public int ItemType { get; internal set; }
		/// <summary>
		/// References the ModTile previously generated
		/// </summary>
		public int TileType { get; internal set; }

		/// <summary>
		/// The translations for the tooltip of this item.
		/// </summary>
		public virtual LocalizedText Tooltip => ModItem.GetLocalization(nameof(Tooltip), () => "");

		#region Appearance variables
		/// <summary>
		/// The filepath for the addon's item texture.
		/// </summary>
		public virtual string ItemTexture => $"{Mod.Name}/Assets/Textures/BeamAddons/{Name}/Item";
		/// <summary>
		/// the filepath for the addon's tile texture.
		/// </summary>
		public virtual string TileTexture => $"{Mod.Name}/Assets/Textures/BeamAddons/{Name}/Tile";
		/// <summary>
		/// The filepath for the addon's normal shot texture.
		/// </summary>
		public virtual string ShotTexture => $"{Mod.Name}/Assets/Textures/BeamAddons/{Name}/Shot";
		/// <summary>
		/// The amount of animation frames in the normal shot texture.
		/// </summary>
		public virtual int ShotFrames { get; } = 1;
		/// <summary>
		/// The filepath for the addon's shot sound effect.
		/// </summary>
		public virtual string ShotSound => $"{Mod.Name}/Assets/Sounds/BeamAddons/{Name}/Shot";
		/// <summary>
		/// The filepath for the addon's charged shot texture.
		/// </summary>
		public virtual string ChargeShotTexture => $"{Mod.Name}/Assets/Textures/BeamAddons/{Name}/ChargeShot";
		/// <summary>
		/// The amount of animation frames in the charged shot texture.
		/// </summary>
		public int ChargeShotFrames { get; } = 1;
		/// <summary>
		/// The filepath for the addon's charged shot sound effect.
		/// </summary> 
		public virtual string ChargeShotSound => $"{Mod.Name}/Assets/Sounds/BeamAddons/{Name}/ChargeShot";
		/// <summary>
		/// The filepath for the addon's shot/charged shot impact sound effect.
		/// </summary>
		public virtual string ImpactSound => $"{Mod.Name}/Assets/Sounds/BeamAddons/{Name}/Impact";
		/// <summary>
		/// The color of the addon's projectile.
		/// </summary>
		public abstract Color ShotColor { get; }
		/// <summary>
		/// The integer ID of the dust particles this addon's projectile will leave behind.
		/// <br/>Use <see cref="DustID"/> for vanilla dust and use <see cref="ModDust.Type"/> for modded ones.
		/// </summary>
		public abstract int ShotDust { get; }
		#endregion

		#region Visual Priority System variables
		/// <summary>
		/// Determines the level of priority of the addon's <b>shot texture</b>.<br />
		/// 0 is the lowest, 5 is the highest<br />
		/// If the addon has the <i>highest shape priority currently installed</i>, its shot graphics will be used.<br />
		/// In the case of a tie, graphics are decided by slot priority.<br/>
		/// Slot shape priority highest to lowest: Secondary(4), Spread(3), Ion(2), Ability(1), Primary(0)
		/// </summary>
		public abstract int ShapePriority { get; }

		/// <summary>
		/// Determines the level of priority of the addon's <b>shot color</b>.<br />
		/// 0 is the lowest, 5 is the highest<br />
		/// If the addon has the <i>highest color priority currently installed</i>, its shot color will be used.<br />
		/// In the case of a tie, color is decided by slot priority.<br />
		/// Slot color priority highest to lowest: Ability(1), Secondary(4), Ion(2), Spread(3), Primary(0)
		/// </summary>
		public abstract int ColorPriority { get; }
		/// <summary>
		/// If true, this addon's sounds will be applied to the shot so long as it has color priority.
		/// </summary>
		public virtual bool SoundOverride { get; } = false;

		/// <summary>
		/// If true, addon's visuals <b>completely override the priority system.</b>
		/// Intended for use on Special Beams, like Hyper and Phazon.<br/>
		/// Checks each addon in sequential order; 1, 2, yadda yadda.<br/>
		/// Defaults to <b>false.</b><br/>
		/// <i>(stands for Very Important Beam)</i>
		/// </summary>
		public virtual bool VIB { get; } = false;
		#endregion

		#region Addon stat variables
		/// <summary>
		/// The slot in the Addon UI that this addon uses.<br/><br/>
		/// See <see cref="BeamAddonSlotID"/> for details on the different slots.
		/// </summary> 
		public virtual int AddonSlot { get; set; } = BeamAddonSlotID.None;

		//These stats are plugged into the WEAPON, not the projectile.
		/// <summary>
		/// The base damage value this addon adds.<br/>
		/// NOTE: Not to be confused with DamageMult, which is applied after this variable.
		/// </summary>
		public virtual int BaseDamage { get; set; } = 0;
		/// <summary>
		/// The damage multiplier value this addon adds.<br/>
		/// NOTE: Input the value as you would see it on the item's tooltip. It will be converted later.<br/>
		/// (i.e. if the addon should have a 50% damage increase, put 50f instead of 1.5f)
		/// </summary>
		public virtual float DamageMult { get; set; } = 0f;
		/// <summary>
		/// The base usetime value this addon adds.<br/>
		/// NOTE: Not to be confused with SpeedMult, which is applied after this variable.
		/// </summary>
		public virtual int BaseSpeed { get; set; } = 0;
		/// <summary>
		/// The usetime multiplier value this addon adds.<br/>
		/// NOTE: Input the value as you would see it on the item's tooltip. It will be converted later.<br/>
		/// (i.e. if the addon should have a 50% speed increase, put 50f instead of 1.5f)
		/// </summary>
		public virtual float SpeedMult { get; set; } = 0f;
		/// <summary>
		/// The base velocity value this addon adds.<br/>
		/// NOTE: Not to be confused with VelocityMult, which is applied after this variable.
		/// </summary>
		public virtual float BaseVelocity { get; set; } = 0f;
		/// <summary>
		/// The velocity multiplier value this addon adds.<br/>
		/// NOTE: Input the value as you would see it on the item's tooltip. It will be converted later.<br/>
		/// (i.e. if the addon should have a 50% speed increase, put 50f instead of 1.5f)
		/// </summary>
		public virtual float VelocityMult { get; set; } = 0f;
		/// <summary>
		/// The critical strike chance this addon adds.<br/>
		/// NOTE: due to how crits work this one does NOT have a respective Mult value.
		/// </summary>
		public virtual int CritChance { get; set; } = 0;
		/// <summary>
		/// The base overheat value this addon adds.<br/>
		/// NOTE: Not to be confused with OverheatMult, which is applied after this variable.
		/// </summary>
		public virtual int BaseOverheat { get; set; } = 0;
		/// <summary>
		/// The overheat multiplier value this addon adds.<br/>
		/// NOTE: Input the value as you would see it on the item's tooltip. It will be converted later.<br/>
		/// (i.e. if the addon should have a -50% overheat multiplier, put -50f instead of 0.5f)
		/// </summary>
		public virtual float OverheatMult { get; set; } = 0f;
		/// <summary>
		/// The amount of extra projectiles this addon will make the player fire.
		/// </summary>
		public virtual int AddShots { get; set; } = 0;


		//These stats get plugged into the PROJECTILE, not the weapon.
		/// <summary>
		/// The buff that this addon will inflict on hit.
		/// </summary>
		public virtual BuffID InflictsBuff { get; set; } = null;
		/// <summary>
		/// The amount of extra tiles this addon allows the beam to interact with before being destroyed.
		/// <br/><br/>Example: The amount of tiles the Wave Beam allows the shot to phase through.
		/// </summary>
		public virtual int TileInteract { get; set; } = 0;
		/// <summary>
		/// The amount of extra NPCs this addon allows the beam to hit before being destroyed.
		/// </summary>
		public virtual int NPCInteract { get; set; } = 0;
		/// <summary>
		/// If true, this addon will continue to perform its actions for as long as Fire is held.
		/// <br/>Only mess with this if you know what you're doing.
		/// <br/><br/>Defaults to <b>false</b>.
		/// </summary>
		public virtual bool HoldFire { get; set; } = false;
		/// <summary>
		/// If true, you cannot use the Charge Beam while this addon is active, regardless of whether or not it's in the array.
		/// <br/><br/>Defaults to <b>whatever value HoldFire is set to</b>.
		/// </summary>
		public virtual bool OverrideCharge => HoldFire;
		#endregion



		public bool ItemNameLiteral { get; set; } = true;
		/// <summary>
		/// Makes the addon in question only add the item and tile, not the beam properties.<br/>
		/// Good for... something, I think   -Z
		/// </summary>
		public abstract bool AddOnlyAddonItem { get; }

		public override sealed void SetupContent()
		{
			SetStaticDefaults();
			ModItem.SetStaticDefaults();
		}

		public override void Load()
		{
			ModItem = new BeamAddonItem(this);
			ModTile = new BeamAddonTile(this);
			if (ModItem == null) { throw new Exception("WTF happened here? BeamAddonItem is null!"); }
			if (ModTile == null) { throw new Exception("WTF happened here? BeamAddonTile is null!"); }
			Mod.AddContent(ModItem);
			Mod.AddContent(ModTile);

		}

		public override void Unload()
		{
			ModItem.Unload();
			ModTile.Unload();
			ModItem = null;
			ModTile = null;
			base.Unload();
		}

		protected sealed override void Register()
		{
			if (!AddOnlyAddonItem && BeamAddonLoader.AddonCount <= 127)
			{
				Type = BeamAddonLoader.AddonCount;
				if (Type > 127)
				{
					throw new Exception("Beam Addons Limit Reached. (Max: 128)");
				}
				BeamAddonLoader.addons.Add(this);
			}
			MetroidMod.Instance.Logger.Info("Register new Beam Addon: " + FullName + ", OnlyAddonItem: " + AddOnlyAddonItem);
		}

		public override void SetStaticDefaults()
		{
			Main.tileSpelunker[TileType] = true;
			Main.tileOreFinderPriority[Type] = 806;
			base.SetStaticDefaults();
		}

		/// <inheritdoc cref="ModItem.SetDefaults()"/>
		public virtual void SetItemDefaults(Item item) { }

		/// <inheritdoc cref="ModItem.AddRecipes"/>
		public virtual void AddRecipes() { }

		/// <summary>
		/// Allows VIB addons to completely commandeer the shot-firing process.
		/// <br/><br/>TODO: make this optional
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public virtual void VIBOverride(Item[] addons) { }

		public virtual void ModifyPreShot(Projectile shot) { }
		public virtual void ModifyShotSpread(Projectile shot) { }
		public virtual void ModifyShotAI(Projectile shot) { }
		public virtual void ModifyShotHitTile(Projectile shot) { }
		public virtual void ModifyShotHitEntity(Projectile shot) { }
		public virtual void ModifyShotHitPlayer(Projectile shot) { }
		public virtual void ModifyShotKill(Projectile shot) { }

		public virtual bool ShowTileHover(Player player) => player.InInteractionRange(Player.tileTargetX, Player.tileTargetY, default);
		/// <inheritdoc cref="ModTile.CanKillTile(int, int, ref bool)"/>
		public virtual bool CanKillTile(int i, int j) { return true; }
		/// <inheritdoc cref="ModMBAddon.CanExplodeTile(int, int)"/>
		public virtual bool CanExplodeTile(int i, int j) { return true; }
	}
}
