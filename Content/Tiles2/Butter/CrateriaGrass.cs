using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;

namespace MetroidMod.Content.Tiles2.Butter
{
	internal class CrateriaGrass : GenericTile
	{
		public override Color MapColor => new(49, 71, 42);
		// TODO: assign appropiate values for these!
		public override SoundStyle HitSound => SoundID.Tink;
		public override int DustType => DustID.Firework_Red;
	}
}