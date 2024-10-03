using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using System.Drawing.Text;
using MetroidMod.ID;

namespace MetroidMod
{

	public static class BeamAddonLoader //I'll be honest I dunno what half this shit does, it's mostly copied from the modsuitaddon equivalent
	{
		/// <summary>
		/// List of all beam addons that exist.
		/// </summary>
		internal static readonly List<ModBeamAddon> addons = new();

		internal static readonly Dictionary<int, string> unloadedAddons = new();

		internal static bool TryGetValue(this IList<ModBeamAddon> list, int type, out ModBeamAddon beam) =>
			list.TryGetValue(i => i.Type == type, out beam);
		internal static bool TryGetValue(this IList<ModBeamAddon> list, string fullName, out ModBeamAddon beam) =>
			list.TryGetValue(i => i.FullName == fullName, out beam);
		internal static bool TryGetValue(this IList<ModBeamAddon> list, Item item, out ModBeamAddon beam) =>
			list.TryGetValue(i => i.ItemType == item.type, out beam);
		public static bool TryGetAddon(Item item, out ModBeamAddon beam) =>
			addons.TryGetValue(item, out beam);
		public static bool TryGetAddon(int type, out ModBeamAddon beam) =>
			addons.TryGetValue(type, out beam);
		public static bool TryGetAddon(string fullName, out ModBeamAddon beam) =>
			addons.TryGetValue(fullName, out beam);
		public static bool TryGetAddon<T>(out ModBeamAddon beam) =>
			addons.TryGetValue(i => i is T, out beam);

		public static int AddonCount => addons.Count;

		public static ModBeamAddon GetAddon(Item item) =>
			addons.TryGetValue(item, out ModBeamAddon beam) ? beam : null;

		public static ModBeamAddon GetAddon(int type) =>
			addons.TryGetValue(type, out ModBeamAddon beam) ? beam : null;

		public static ModBeamAddon GetAddon(string fullName) =>
			addons.TryGetValue(fullName, out ModBeamAddon beam) ? beam : null;

		public static ModBeamAddon GetAddon<T>() where T : ModBeamAddon =>
			addons.TryGetValue(i => i is T, out ModBeamAddon beam) ?beam : null;

		public static bool IsABeamTile(Tile tile)
		{
			foreach (ModBeamAddon addon in addons)
			{
				if (tile.TileType == addon.TileType) { return true; }
			}
			return false;
		}

		/// <summary>
		/// Checks the piority values of the loaded addons and determines what the projectile should look like.<br/>
		/// Method checks for VIB, then ShapePriority, then ColorPriority.<br/>
		/// Unique combination graphics should be checked for within the beam with the highest ShapePriority in the combination<br/>
		/// (i.e. Fusion's DNA-esque Plasma+Wave would be stored and checked for in Plasma)
		/// </summary>
		/// <param name="slot1"></param>
		/// <param name="slot2"></param>
		/// <param name="slot3"></param>
		/// <param name="slot4"></param>
		/// <param name="slot5"></param>
		/// <returns></returns>
		public static int[] VisualPriority(ModBeamAddon[] addons)
		{
			//let it be known there was originally gonna be a third array here for the VIB(e) check results called vibRibbon        -Z
			int[] shapeOfPew = new int[addons.Length]; //store all the ShapePriority check results (I couldn't come up with a secondary joke (mostly because I didn't feel like trying))
			int[] fuckYouIceBeam = new int[addons.Length]; //store all the ColorPriority check results here at Big Zek Hell's Arrays
			int[] winners = new int[2];

			for (int i = 0; i < addons.Length - 1; ++i) //Check all addon slots for if VIB is true
			{
				if (addons[i] == null || addons[i].VIB == false) { continue; }
				if (addons[i].VIB == true) { winners = [i, i]; return winners; }
			}


			int highestShapePriorityIndex = -1;  //Compare ShapePriority values of all installed beams, determine which is the highest
			int highestShapePriority = -1;
			for (int i = 0; i < addons.Length - 1; i++)
			{
				if (addons[i]?.ShapePriority >= highestShapePriority)
				{
					highestShapePriorityIndex = i;
					highestShapePriority = addons[i].ShapePriority;
				}
			}

			for (int i = 0; i < addons.Length - 1; ++i) //Compare ColorPriority values of all installed beams, determine which is the highest
			{
				if (addons[i] == null) { fuckYouIceBeam[i] = -1; continue; }
				fuckYouIceBeam[i] = addons[i].ColorPriority;
			}
			ModBeamAddon[] colorOrder = [addons[BeamAddonSlotID.Charge], addons[BeamAddonSlotID.PrimaryA], addons[BeamAddonSlotID.Utility], addons[BeamAddonSlotID.PrimaryB], addons[BeamAddonSlotID.Secondary]]; //something something 20XX
			int highestColorPriorityIndex = -1;
			int highestColorPriority = -1;
			for (int i = 0; i < colorOrder.Length; i++)
			{
				if (colorOrder[i]?.ColorPriority >= highestColorPriority)
				{
					highestColorPriorityIndex = i;
					highestColorPriority = addons[i].ColorPriority;
				}
			}
			winners = [highestShapePriorityIndex, highestColorPriorityIndex]; //If there are no winners it should turn up -1, -1
			return winners;
		}

		internal static void ReloadTypes(TagCompound unloadedTag)
		{
			unloadedAddons.Clear();
			Dictionary<string, object> unloaded = new(unloadedTag);
			foreach ((string name, object type) in unloaded)
			{
				unloadedAddons[(int)type] = name;
			}

			HashSet<int> reserveTypes = new();
			foreach ((int type, string name) in unloadedAddons)
			{
				if (addons.TryGetValue(name, out ModBeamAddon beam))
				{
					beam.ChangeType(type);
					reserveTypes.Add(type);
				}
			}

			int freeType = 3;
			foreach (ModBeamAddon beam in addons)
			{
				if (reserveTypes.Contains(beam.Type)) { continue; }

				while (reserveTypes.Contains(freeType)) { freeType++; }

				beam.ChangeType(freeType);
				freeType++;
			}
		}

		internal static void Unload()
		{
			addons.Clear();
			unloadedAddons.Clear();
		}
	}
}
