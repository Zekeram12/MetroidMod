using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;

namespace MetroidMod
{
	public static class BeamAddonLoader
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
