﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using MetroidModPorted.Common.Players;

namespace MetroidModPorted.Common.ItemDropRules.Conditions
{
	public class ReserveHearts : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation)
			{
				MPlayer mp = info.player.GetModPlayer<MPlayer>();
				return mp.reserveTanks > 0 && mp.reserveHearts < mp.reserveTanks && info.player.statLife >= info.player.statLifeMax2;
			}
			return false;
		}

		public bool CanShowItemDropInUI() => false;

		public string GetConditionDescription() => "No.";
	}
}