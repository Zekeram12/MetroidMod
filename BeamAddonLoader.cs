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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetroidMod
{
	/// <summary>
	/// Manages ModBeamAddons, stacks effects of addons installed into arm cannons, and provides helpful methods to retrieve beam addon information.
	/// </summary>
	public static class BeamAddonLoader //I'll be honest I dunno what half this shit does, it's mostly copied from the modsuitaddon equivalent
	{
		/// <summary>
		/// List of all beam addons that exist.
		/// </summary>
		internal static readonly List<ModBeamAddon> addons = new();

		internal static readonly Dictionary<int, string> unloadedAddons = new();

		//The following methods are all used in order to obtain an addon's ModBeamAddon value through its other forms.
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

		/// <summary>
		/// The total number of beam addons that exist.
		/// </summary>
		public static int AddonCount => addons.Count;

		/// <summary>
		/// Gets the ModBeamAddon of an addon through its <b>Item value.</b><br/>
		/// Used to access an addon's properties for further use.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static ModBeamAddon GetAddon(Item item) =>
			addons.TryGetValue(item, out ModBeamAddon beam) ? beam : null;
		/// <summary>
		/// Gets the ModBeamAddon of an addon through its <b>index number.</b><br/>
		/// Used to access an addon's properties for further use.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static ModBeamAddon GetAddon(int type) =>
			addons.TryGetValue(type, out ModBeamAddon beam) ? beam : null;

		/// <summary>
		/// Gets the ModBeamAddon of an addon through its <b>name text.</b><br/>
		/// Used to access an addon's properties for further use.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static ModBeamAddon GetAddon(string fullName) =>
			addons.TryGetValue(fullName, out ModBeamAddon beam) ? beam : null;

		/// <summary>
		/// Gets the ModBeamAddon of an addon through <b>idfk</b><br/>
		/// Used to access an addon's properties for further use.<br/>
		/// NOTE: Can someone else check this thing and tell me how it gets the thing?   -Z
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
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

		//the following methods are used to combine the data of all beams installed in the arm cannon.
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
			ModBeamAddon[] colorOrder = [addons[BeamAddonSlotID.Primary], addons[BeamAddonSlotID.Spread], addons[BeamAddonSlotID.Ion], addons[BeamAddonSlotID.Secondary], addons[BeamAddonSlotID.Ability]]; //something something 20XX
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

		/// <summary>
		/// Combines the additional base damage values of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static int BaseDamageStacker(ModBeamAddon[] addons)
		{
			int sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].BaseDamage;
			}
			return sum;
		}
		/// <summary>
		/// Combines the damage multipliers of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static float DamageMultStacker(ModBeamAddon[] addons)
		{
			float sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].DamageMult;
			}
			return sum;
		}
		/// <summary>
		/// Combines the additional base usetime of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static int BaseSpeedStacker(ModBeamAddon[] addons)
		{
			int sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].BaseSpeed;
			}
			return sum;
		}
		/// <summary>
		/// Combines the usetime multiplier of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static float SpeedMultStacker(ModBeamAddon[] addons)
		{
			float sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].SpeedMult;
			}
			return sum;
		}
		/// <summary>
		/// Combines the additional base velocity of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static float BaseVelocityStacker(ModBeamAddon[] addons)
		{
			float sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].BaseVelocity;
			}
			return sum;
		}
		/// <summary>
		/// Combines the velocity multiplier of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static float VelocityMultStacker(ModBeamAddon[] addons)
		{
			float sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].VelocityMult;
			}
			return sum;
		}
		/// <summary>
		/// Combines the critical strike chance of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static int CritChanceStacker(ModBeamAddon[] addons)
		{
			int sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].CritChance;
			}
			return sum;
		}
		/// <summary>
		/// Combines the additional base overheat of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static int BaseOverheatStacker(ModBeamAddon[] addons)
		{
			int sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].BaseOverheat;
			}
			return sum;
		}
		/// <summary>
		/// Combines the overheat multiplier of every installed addon into a single value.
		/// </summary>
		/// <param name="addons"></param>
		/// <returns></returns>
		public static float OverheatMultStacker(ModBeamAddon[] addons)
		{
			float sum = 0;
			for (int i = 0; i < addons.Length - 1; ++i)
			{
				if (addons[i] == null) { continue; }
				sum += addons[i].OverheatMult;
			}
			return sum;
		}


		//The following methods are simply some under-the-hood stuff to make sure things actually load properly.
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
