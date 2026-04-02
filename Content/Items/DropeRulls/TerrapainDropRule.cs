using System;
using System.Collections.Generic;
using Terrapain.Common.System;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Terrapain.Content.Items.ItemDropRules;

public class TerrapainDropRull : IItemDropRule
{
	public struct Parameters
	{
		/// <summary>
		/// ignores the rull if classic mode on
		/// </summary>
		public bool VanillaDrop;
		public int ChanceNumerator;
		public int ChanceDenominator;
		public int MinimumItemDropsCount;
		public int MaximumItemDropsCount;
		public int MinimumStackPerChunkBase;
		public int MaximumStackPerChunkBase;
		public int BonusMinDropsPerChunkPerPlayer;
		public int BonusMaxDropsPerChunkPerPlayer;
        public int Boss;
        public bool MultiplyByDifficulty;
        public bool GaranteedInSuicide;
		public float GetPersonalDropRate() => (float)ChanceNumerator / (float)ChanceDenominator;
	}

	public int itemId;
	
	//TML: Turned from a field into an autoproperty.
	public Parameters parameters { get; private init; }

	public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

	public TerrapainDropRull(int itemId, Parameters parameters)
	{
		if (parameters.MinimumItemDropsCount > parameters.MaximumItemDropsCount) {
			throw new ArgumentException($"{nameof(parameters.MinimumItemDropsCount)} must be lesser or equal to {nameof(parameters.MaximumItemDropsCount)}.", nameof(parameters));
		}

		if (parameters.MinimumStackPerChunkBase > parameters.MaximumStackPerChunkBase) {
			throw new ArgumentException($"{nameof(parameters.MinimumStackPerChunkBase)} must be lesser or equal to {nameof(parameters.MaximumStackPerChunkBase)}.", nameof(parameters));
		}

		if (parameters.BonusMinDropsPerChunkPerPlayer > parameters.BonusMaxDropsPerChunkPerPlayer) {
			throw new ArgumentException($"{nameof(parameters.BonusMinDropsPerChunkPerPlayer)} must be lesser or equal to {nameof(parameters.BonusMaxDropsPerChunkPerPlayer)}.", nameof(parameters));
		}

		ChainedRules = new List<IItemDropRuleChainAttempt>();
		this.parameters = parameters;
		this.itemId = itemId;
	}

	public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
	{
		ItemDropAttemptResult result;
		if (info.player.RollLuck(parameters.ChanceDenominator) < parameters.ChanceNumerator || (parameters.GaranteedInSuicide && BossDownedSystem.bossBagsSuicide[parameters.Boss] > 0)) 
        {
            int multiplyer = parameters.VanillaDrop? 0 : 1;
            if (parameters.MultiplyByDifficulty && BossDownedSystem.bossBagsTorture[parameters.Boss] > 0)
            {
                multiplyer = parameters.VanillaDrop ? 1 : 2;
            }
            if (parameters.MultiplyByDifficulty && BossDownedSystem.bossBagsSuicide[parameters.Boss] > 0)
            {
                multiplyer = parameters.VanillaDrop ? 2 : 3;
            }
			int num = info.rng.Next(parameters.MinimumItemDropsCount * multiplyer, parameters.MaximumItemDropsCount * multiplyer + 1);
			int activePlayersCount = Main.CurrentFrameFlags.ActivePlayersCount;
			int minValue = parameters.MinimumStackPerChunkBase + activePlayersCount * parameters.BonusMinDropsPerChunkPerPlayer;
			int num2 = parameters.MaximumStackPerChunkBase + activePlayersCount * parameters.BonusMaxDropsPerChunkPerPlayer;


			for (int i = 0; i < num; i++) {
				CommonCode.DropItem(info, itemId, info.rng.Next(minValue, num2 + 1), scattered: true);
			}

			result = default(ItemDropAttemptResult);
			result.State = ItemDropAttemptResultState.Success;
			return result;
		}

		result = default(ItemDropAttemptResult);
		result.State = ItemDropAttemptResultState.FailedRandomRoll;
		return result;
	}

	public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
	{
		float personalDropRate = parameters.GetPersonalDropRate();
		float dropRate = personalDropRate * ratesInfo.parentDroprateChance;
		drops.Add(new DropRateInfo(itemId, parameters.MinimumItemDropsCount * (parameters.MinimumStackPerChunkBase + parameters.BonusMinDropsPerChunkPerPlayer), parameters.MaximumItemDropsCount * (parameters.MaximumStackPerChunkBase + parameters.BonusMaxDropsPerChunkPerPlayer), dropRate, ratesInfo.conditions));
		Chains.ReportDroprates(ChainedRules, personalDropRate, drops, ratesInfo);
	}

	public bool CanDrop(DropAttemptInfo info) => true;
}
