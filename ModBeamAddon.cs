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

//gonna document as much of the code as I can to make it easy to follow
namespace MetroidMod
{
	/// <summary>
	/// The base type for all Power Beam addons.
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

		/// <summary>
		/// The filepath for the addon's item texture.
		/// </summary>
		public abstract string ItemTexture { get; }
		/// <summary>
		/// the filepath for the addon's tile texture.
		/// </summary>
		public abstract string TileTexture { get; }
		/// <summary>
		/// The filepath for the addon's normal shot texture.
		/// </summary>
		public abstract string ShotTexture { get; }
		/// <summary>
		/// The amount of animation frames in the normal shot texture.
		/// </summary>
		public int ShotFrames { get; } = 1;
		/// <summary>
		/// The filepath for the addon's shot sound effect.
		/// </summary>
		public abstract string ShotSound { get; }
		/// <summary>
		/// The filepath for the addon's charged shot texture.
		/// </summary>
		public abstract string ChargeShotTexture { get; }
		/// <summary>
		/// The amount of animation frames in the charged shot texture.
		/// </summary>
		public int ChargeShotFrames { get; } = 1;
		/// <summary>
		/// The filepath for the addon's charged shot sound effect.
		/// </summary>
		public abstract string ChargeShotSound { get; }
		/// <summary>
		/// The color of the addon's projectile.
		/// </summary>
		public abstract Color ShotColor { get; }

		/// <summary>
		/// Determines the level of priority of the addon's shot graphics.<br />
		/// 0 is the lowest, 5 is the highest<br />
		/// If the addon has the highest shape priority currently installed, its shot graphics will be used.<br />
		/// In the case of a tie, graphics are decided by slot priority.<br />
		/// Slot shape priority highest to lowest: Primary B, Primary A, Utility, Secondary, Charge
		/// </summary>
		public int ShapePriority;
		/// <summary>
		/// Determines the level of priority of the addon's shot color.<br />
		/// 0 is the lowest, 5 is the highest<br />
		/// If the addon has the highest color priority installed, its shot color will be used.<br />
		/// In the case of a tie, color is decided by slot priority.<br />
		/// Slot color priority highest to lowest: Utility, Primary B, Secondary, Primary A, Charge
		/// </summary>
		public int ColorPriority;
		/// <summary>
		/// If true, addon's visuals completely override the priority system.<br/>
		/// Intended for use on Special Beams, like Hyper and Phazon<br/>
		/// (stands for Very Important Beam)
		/// </summary>
		public bool VIB = false;

		public virtual int AddonSlot { get; set; } = BeamAddonSlotID.None;

		public bool ItemNameLiteral { get; set; } = true;
		public abstract bool AddOnlyAddonItem { get; }


		protected sealed override void Register()
		{
			throw new NotImplementedException();
		}
	}
}
