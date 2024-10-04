using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria;
using MetroidMod.ID;
using MetroidMod.Default;

//gonna document as much of the code as I can to make it easy to follow
namespace MetroidMod
{
	/// <summary>
	/// The base type for all Power Beam addons.<br/>
	/// ModBeamAddons automatically generate a <see cref="Terraria.ModLoader.ModItem"/> and a <see cref="Terraria.ModLoader.ModTile"/> to access the addon in-game.<br/>
	/// Textures are grabbed automatically at this filepath:<br/>
	/// <u>(name of mod)<b>/Assets/Textures/BeamAddons/</b>(name of addon file)<b>/</b>(Item for item sprite, Tile for tile sprite, Shot for shot sprite, etc.)</u><br/>
	/// but can be overriden to point to any filepath. Sounds are also stored this way, just swap Textures for Sounds.<br/>
	/// Every
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

		//Appearance variables
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
		public int ShotFrames { get; } = 1;
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

		//Visual Priority System variables
		/// <summary>
		/// Determines the level of priority of the addon's <b>shot texture</b>.<br />
		/// 0 is the lowest, 5 is the highest<br />
		/// If the addon has the <i>highest shape priority currently installed</i>, its shot graphics will be used.<br />
		/// In the case of a tie, graphics are decided by slot priority.<br/>
		/// Slot shape priority highest to lowest: Secondary(4), Spread(3), Ion(2), Ability(1), Primary(0)
		/// </summary>
		public int ShapePriority;
		/// <summary>
		/// Determines the level of priority of the addon's <b>shot color</b>.<br />
		/// 0 is the lowest, 5 is the highest<br />
		/// If the addon has the <i>highest color priority currently installed</i>, its shot color will be used.<br />
		/// In the case of a tie, color is decided by slot priority.<br />
		/// Slot color priority highest to lowest: Ability(1), Secondary(4), Ion(2), Spread(3), Primary(0)
		/// </summary>
		public int ColorPriority;
		/// <summary>
		/// If true, addon's visuals <b>completely override the priority system.</b><br/>
		/// Intended for use on Special Beams, like Hyper and Phazon<br/>
		/// Checks each addon in sequential order; 1, 2, yadda yadda.<br/>
		/// <i>(stands for Very Important Beam)</i>
		/// </summary>
		public bool VIB = false;

		//Addon stat variables
		/// <summary>
		/// The slot in the Addon UI that this addon uses.<br/><br/>
		/// General rule of thumb for what to put where:<br/>
		/// <u><see cref="BeamAddonSlotID.Primary"/></u> is the <b>base</b> upon which other addons modify, and can be stored in <b>Quick-Swap</b> to change weapons on the fly. Things like the Charge Beam go here.<br/>
		/// <u><see cref="BeamAddonSlotID.Ability"/></u> is for addons that <b>apply after-effects</b> to your beam shot, like the Ice Beam.<br/>
		/// <u><see cref="BeamAddonSlotID.Ion"/></u> is for addons that affect how the beam <b>interacts with terrain</b>, like the Wave Beam.<br/>
		/// <u><see cref="BeamAddonSlotID.Spread"/></u> is for addons that affect <b>how your projectiles come out</b>, like the Spazer Beam.<br/>
		/// <u><see cref="BeamAddonSlotID.Secondary"/></u> is for addons that affect how the beam <b>interacts with enemies</b>, like the Plasma Beam.<br/>
		/// <u><see cref="BeamAddonSlotID.Ammo"/></u> is <i>exclusively</i> for ammunition, like UA Expansions. <b>This slot does not get checked with the others.</b>
		/// </summary> 
		public virtual int AddonSlot { get; set; } = BeamAddonSlotID.None;
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
		public virtual float VelocityMult {  get; set; } = 0f;
		/// <summary>
		/// The critical strike chance this addon adds.<br/>
		/// NOTE: due to how crits work this one does NOT have a respective Mult value.
		/// </summary>
		public virtual int CritChance { get; set; } = 0;
		/// <summary>
		/// The base overheat value this addon adds.<br/>
		/// NOTE: Not to be confused with OverheatMult, which is applied after this variable.
		/// </summary>
		public virtual int BaseOverheat { get; set;} = 0;
		/// <summary>
		/// The overheat multiplier value this addon adds.<br/>
		/// NOTE: Input the value as you would see it on the item's tooltip. It will be converted later.<br/>
		/// (i.e. if the addon should have a -50% overheat multiplier, put -50f instead of 0.5f)
		/// </summary>
		public virtual float OverheatMult { get; set; } = 0f;




		public bool ItemNameLiteral { get; set; } = true;
		/// <summary>
		/// Makes the addon in question only add the item and tile, not the beam properties.<br/>
		/// Good for... something, I think   -Z
		/// </summary>
		public abstract bool AddOnlyAddonItem { get; }

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
			if (!AddOnlyAddonItem || BeamAddonLoader.AddonCount <= 127)
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
		public virtual bool ShowTileHover(Player player) => player.InInteractionRange(Player.tileTargetX, Player.tileTargetY, default);
		/// <inheritdoc cref="ModTile.CanKillTile(int, int, ref bool)"/>
		public virtual bool CanKillTile(int i, int j) { return true; }
		/// <inheritdoc cref="ModMBAddon.CanExplodeTile(int, int)"/>
		public virtual bool CanExplodeTile(int i, int j) { return true; }
	}
}
