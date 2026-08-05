using System.Collections.Generic;
using NUnit.Framework;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Expedition;
using UnboundArcana.Player;
using UnboundArcana.Spells.Behaviors.Beam;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Modules.SizeModifier;
using UnboundArcana.Spells.Services;
using UnityEngine;

namespace UnboundArcana.Tests
{
	public class LaboratoryMajorRewardPresenterTests
	{
		private readonly List<Object> createdObjects = new();
		private GameObject casterObject;
		private GameObject inputObject;
		private GameObject presenterObject;
		private PlayerInput playerInput;
		private LaboratoryMajorRewardPresenter presenter;
		private SpellDataCatalog catalog;
		private RewardController rewards;
		private SpellModificationService modification;
		private SpellCaster spellCaster;

		[SetUp]
		public void SetUp()
		{
			casterObject = new GameObject("SpellCaster");
			spellCaster = casterObject.AddComponent<SpellCaster>();
			inputObject = new GameObject("PlayerInput");
			playerInput = inputObject.AddComponent<PlayerInput>();
			presenterObject = new GameObject("LaboratoryMajorRewardPresenter");
			presenter = presenterObject.AddComponent<LaboratoryMajorRewardPresenter>();
			catalog = Create<SpellDataCatalog>();
			catalog.modules = new List<SpellModuleDefinition>();
			ModuleRewardTable table = Create<ModuleRewardTable>();
			rewards = new RewardController(new ModuleRewardService(table));
			modification = new SpellModificationService(new GameEventBus());
			BeamBehaviorDefinition behavior = Create<BeamBehaviorDefinition>();
			SpellDefinition definition = Create<SpellDefinition>();
			definition.behavior = behavior;
			spellCaster.InitializeLoadout(new List<SpellDefinition> { definition });
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(presenterObject);
			Object.DestroyImmediate(inputObject);
			Object.DestroyImmediate(casterObject);
			foreach (Object createdObject in createdObjects) { Object.DestroyImmediate(createdObject); }
		}

		[Test]
		public void FailedSelectionKeepsPresenterOpenAndInputBlocked()
		{
			SpellModuleDefinition offered = Create<SizeModifierModuleDefinition>();
			SpellModuleDefinition invalid = Create<SizeModifierModuleDefinition>();
			catalog.modules.Add(offered);
			LaboratoryMajorRewardSession session = CreateSession();

			Assert.That(presenter.Open(session, playerInput), Is.EqualTo(LaboratoryOfferStatus.Success));
			LaboratorySelectionResult result = presenter.SelectOffer(invalid);

			Assert.That(result.Status, Is.EqualTo(LaboratorySelectionStatus.InvalidOffer));
			Assert.That(presenter.IsOpen, Is.True);
			Assert.That(playerInput.InputEnabled, Is.False);
			Assert.That(presenter.FailureMessage, Is.Not.Empty);
		}

		[Test]
		public void SuccessfulSelectionSignalsOnceAndRejectsAdditionalSelections()
		{
			SpellModuleDefinition offered = Create<SizeModifierModuleDefinition>();
			catalog.modules.Add(offered);
			LaboratoryMajorRewardSession session = CreateSession();
			int successCount = 0;
			presenter.SelectionSucceeded += _ => successCount++;
			presenter.Open(session, playerInput);

			LaboratorySelectionResult first = presenter.SelectOffer(session.Offers[0]);
			LaboratorySelectionResult second = presenter.SelectOffer(session.Offers[0]);

			Assert.That(first.Success, Is.True);
			Assert.That(second.Status, Is.EqualTo(LaboratorySelectionStatus.AlreadySelected));
			Assert.That(successCount, Is.EqualTo(1));
			Assert.That(playerInput.InputEnabled, Is.False);
		}

		private LaboratoryMajorRewardSession CreateSession()
		{
			return new LaboratoryMajorRewardSession(spellCaster, catalog, rewards, modification);
		}

		private T Create<T>() where T : ScriptableObject
		{
			T instance = ScriptableObject.CreateInstance<T>();
			createdObjects.Add(instance);
			return instance;
		}
	}
}
