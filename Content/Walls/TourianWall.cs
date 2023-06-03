﻿using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MetroidMod.Content.Walls
{
	public class TourianWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = 87;

			AddMapEntry(new Color(39, 48, 63));
		}
	}
}
