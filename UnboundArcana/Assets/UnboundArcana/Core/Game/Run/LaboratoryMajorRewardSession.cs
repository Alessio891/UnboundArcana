using System.Collections.Generic;
using System.Linq;
using UnboundArcana.Core.Entities;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Services;

namespace UnboundArcana.Core.Expedition
{
	public enum LaboratoryOfferStatus
	{
		Success,
		MissingSpellCaster,
		MissingSpellLoadout,
		MissingActiveSpell,
		MissingModuleCatalog,
		MissingRewardService,
		MissingModificationService,
		NoCompatibleModules
	}

	public enum LaboratorySelectionStatus
	{
		Success,
		SessionNotReady,
		AlreadySelected,
		InvalidOffer,
		OfferNoLongerCompatible,
		ModificationRejected
	}

	public readonly struct LaboratorySelectionResult
	{
		public bool Success => Status == LaboratorySelectionStatus.Success;
		public LaboratorySelectionStatus Status { get; }
		public SpellConfiguration Configuration { get; }
		public SpellModuleDefinition Module { get; }

		public LaboratorySelectionResult(LaboratorySelectionStatus status, SpellConfiguration configuration, SpellModuleDefinition module)
		{
			Status = status;
			Configuration = configuration;
			Module = module;
		}
	}

	public class LaboratoryMajorRewardSession
	{
		private const int OfferCount = 3;

		private readonly SpellCaster spellCaster;
		private readonly SpellDataCatalog moduleCatalog;
		private readonly RewardController rewardController;
		private readonly SpellModificationService modificationService;
		private readonly List<SpellModuleDefinition> offers = new();

		private bool offersGenerated;
		private bool selectionSucceeded;

		public IReadOnlyList<SpellModuleDefinition> Offers => offers;
		public SpellConfiguration ActiveConfiguration { get; private set; }
		public LaboratoryOfferStatus OfferStatus { get; private set; }
		public bool SelectionSucceeded => selectionSucceeded;

		public LaboratoryMajorRewardSession(SpellCaster spellCaster, SpellDataCatalog moduleCatalog, RewardController rewardController, SpellModificationService modificationService)
		{
			this.spellCaster = spellCaster;
			this.moduleCatalog = moduleCatalog;
			this.rewardController = rewardController;
			this.modificationService = modificationService;
		}

		public static LaboratoryMajorRewardSession CreateForPlayer(SpellCaster spellCaster)
		{
			return new LaboratoryMajorRewardSession(spellCaster, GameDatabase.Instance != null ? GameDatabase.Instance.Spells : null, GameRuntimeManager.Instance != null ? GameRuntimeManager.Instance.Rewards : null, GameRuntimeManager.Instance != null ? GameRuntimeManager.Instance.SpellModification : null);
		}

		public LaboratoryOfferStatus GenerateOffers()
		{
			if (offersGenerated)
				return OfferStatus;

			offersGenerated = true;
			OfferStatus = ResolveDependenciesAndActiveSpell();

			if (OfferStatus != LaboratoryOfferStatus.Success)
				return OfferStatus;

			List<SpellModuleDefinition> compatibleModules = moduleCatalog.modules.Where(module => modificationService.CanAddModule(ActiveConfiguration, module)).Distinct().ToList();

			if (compatibleModules.Count == 0)
			{
				OfferStatus = LaboratoryOfferStatus.NoCompatibleModules;
				return OfferStatus;
			}

			offers.AddRange(rewardController.GenerateModuleRewards(compatibleModules, OfferCount));

			if (offers.Count == 0)
				OfferStatus = LaboratoryOfferStatus.NoCompatibleModules;

			return OfferStatus;
		}

		public LaboratorySelectionResult TrySelect(SpellModuleDefinition module)
		{
			if (selectionSucceeded)
				return Result(LaboratorySelectionStatus.AlreadySelected, module);

			if (!offersGenerated || OfferStatus != LaboratoryOfferStatus.Success)
				return Result(LaboratorySelectionStatus.SessionNotReady, module);

			if (module == null || !offers.Contains(module))
				return Result(LaboratorySelectionStatus.InvalidOffer, module);

			if (!modificationService.CanAddModule(ActiveConfiguration, module))
				return Result(LaboratorySelectionStatus.OfferNoLongerCompatible, module);

			if (!modificationService.TryAddModule(ActiveConfiguration, module))
				return Result(LaboratorySelectionStatus.ModificationRejected, module);

			selectionSucceeded = true;
			return Result(LaboratorySelectionStatus.Success, module);
		}

		private LaboratoryOfferStatus ResolveDependenciesAndActiveSpell()
		{
			if (spellCaster == null)
				return LaboratoryOfferStatus.MissingSpellCaster;

			SpellLoadout loadout = spellCaster.SpellLoadout;

			if (loadout == null)
				return LaboratoryOfferStatus.MissingSpellLoadout;

			if (loadout.CurrentSpell < 0 || loadout.CurrentSpell >= loadout.Slots.Count)
				return LaboratoryOfferStatus.MissingActiveSpell;

			SpellSlot activeSpell = loadout.GetCurrentSpell();

			if (activeSpell?.Configuration == null)
				return LaboratoryOfferStatus.MissingActiveSpell;

			if (moduleCatalog?.modules == null)
				return LaboratoryOfferStatus.MissingModuleCatalog;

			if (rewardController == null)
				return LaboratoryOfferStatus.MissingRewardService;

			if (modificationService == null)
				return LaboratoryOfferStatus.MissingModificationService;

			ActiveConfiguration = activeSpell.Configuration;
			return LaboratoryOfferStatus.Success;
		}

		private LaboratorySelectionResult Result(LaboratorySelectionStatus status, SpellModuleDefinition module)
		{
			return new LaboratorySelectionResult(status, ActiveConfiguration, module);
		}
	}
}
