using System.Collections.Generic;
using NUnit.Framework;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Expedition;
using UnboundArcana.Spells.Behaviors.Beam;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Modules.ProjectileSpeed;
using UnboundArcana.Spells.Modules.SizeModifier;
using UnboundArcana.Spells.Services;
using UnityEngine;

namespace UnboundArcana.Tests
{
	public class LaboratoryMajorRewardSessionTests
	{
		private readonly List<Object> createdObjects = new();
		private GameObject casterObject;
		private SpellCaster spellCaster;
		private SpellDataCatalog catalog;
		private RewardController rewards;
		private SpellModificationService modification;

		[SetUp]
		public void SetUp()
		{
			casterObject = new GameObject("SpellCaster");
			spellCaster = casterObject.AddComponent<SpellCaster>();
			catalog = Create<SpellDataCatalog>();
			catalog.modules = new List<SpellModuleDefinition>();
			ModuleRewardTable table = Create<ModuleRewardTable>();
			rewards = new RewardController(new ModuleRewardService(table));
			modification = new SpellModificationService(new GameEventBus());
			InitializeActiveSpell();
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(casterObject);

			foreach (Object createdObject in createdObjects)
				Object.DestroyImmediate(createdObject);
		}

		[Test]
		public void GenerateOffersReturnsThreeDistinctCompatibleModules()
		{
			SizeModifierModuleDefinition first = AddCompatibleModule();
			SizeModifierModuleDefinition second = AddCompatibleModule();
			SizeModifierModuleDefinition third = AddCompatibleModule();
			catalog.modules.Add(first);
			catalog.modules.Add(Create<ProjectileSpeedModuleDefinition>());

			LaboratoryMajorRewardSession session = CreateSession();

			Assert.That(session.GenerateOffers(), Is.EqualTo(LaboratoryOfferStatus.Success));
			Assert.That(session.Offers, Has.Count.EqualTo(3));
			Assert.That(new HashSet<SpellModuleDefinition>(session.Offers), Has.Count.EqualTo(3));
			Assert.That(session.Offers, Does.Contain(first));
			Assert.That(session.Offers, Does.Contain(second));
			Assert.That(session.Offers, Does.Contain(third));
		}

		[Test]
		public void GenerateOffersReturnsAllWhenFewerThanThreeAreCompatible()
		{
			AddCompatibleModule();
			AddCompatibleModule();

			LaboratoryMajorRewardSession session = CreateSession();

			Assert.That(session.GenerateOffers(), Is.EqualTo(LaboratoryOfferStatus.Success));
			Assert.That(session.Offers, Has.Count.EqualTo(2));
		}

		[Test]
		public void GenerateOffersFailsGracefullyWhenNoneAreCompatible()
		{
			catalog.modules.Add(Create<ProjectileSpeedModuleDefinition>());
			LaboratoryMajorRewardSession session = CreateSession();
			int originalModuleCount = session.ActiveConfiguration?.modules.Count ?? 0;

			Assert.That(session.GenerateOffers(), Is.EqualTo(LaboratoryOfferStatus.NoCompatibleModules));
			Assert.That(session.Offers, Is.Empty);
			Assert.That(session.TrySelect(catalog.modules[0]).Status, Is.EqualTo(LaboratorySelectionStatus.SessionNotReady));
			Assert.That(spellCaster.SpellLoadout.GetCurrentSpell().Configuration.modules, Has.Count.EqualTo(originalModuleCount));
		}

		[Test]
		public void SuccessfulSelectionAppliesImmediatelyAndPreventsSecondSuccess()
		{
			AddCompatibleModule();
			AddCompatibleModule();
			LaboratoryMajorRewardSession session = CreateSession();
			session.GenerateOffers();
			SpellModuleDefinition selected = session.Offers[0];
			SpellModuleDefinition second = session.Offers[1];

			LaboratorySelectionResult firstResult = session.TrySelect(selected);
			LaboratorySelectionResult secondResult = session.TrySelect(second);

			Assert.That(firstResult.Success, Is.True);
			Assert.That(firstResult.Configuration.HasModule(selected), Is.True);
			Assert.That(secondResult.Status, Is.EqualTo(LaboratorySelectionStatus.AlreadySelected));
			Assert.That(firstResult.Configuration.HasModule(second), Is.False);
		}

		[Test]
		public void SelectionRejectsModuleThatWasNotOffered()
		{
			AddCompatibleModule();
			SpellModuleDefinition notOffered = Create<SizeModifierModuleDefinition>();
			LaboratoryMajorRewardSession session = CreateSession();
			session.GenerateOffers();

			LaboratorySelectionResult result = session.TrySelect(notOffered);

			Assert.That(result.Status, Is.EqualTo(LaboratorySelectionStatus.InvalidOffer));
			Assert.That(result.Configuration.HasModule(notOffered), Is.False);
		}

		[Test]
		public void SelectionRevalidatesAnOfferedModuleBeforeApplyingIt()
		{
			SpellModuleDefinition offered = AddCompatibleModule();
			LaboratoryMajorRewardSession session = CreateSession();
			session.GenerateOffers();
			modification.TryAddModule(session.ActiveConfiguration, offered);

			LaboratorySelectionResult result = session.TrySelect(offered);

			Assert.That(result.Status, Is.EqualTo(LaboratorySelectionStatus.OfferNoLongerCompatible));
			Assert.That(session.SelectionSucceeded, Is.False);
			Assert.That(session.ActiveConfiguration.modules, Has.Count.EqualTo(1));
		}

		private LaboratoryMajorRewardSession CreateSession()
		{
			return new LaboratoryMajorRewardSession(spellCaster, catalog, rewards, modification);
		}

		private SizeModifierModuleDefinition AddCompatibleModule()
		{
			SizeModifierModuleDefinition module = Create<SizeModifierModuleDefinition>();
			catalog.modules.Add(module);
			return module;
		}

		private void InitializeActiveSpell()
		{
			BeamBehaviorDefinition behavior = Create<BeamBehaviorDefinition>();
			SpellDefinition definition = Create<SpellDefinition>();
			definition.behavior = behavior;
			definition.modules = new SpellModuleDefinition[0];
			spellCaster.InitializeLoadout(new List<SpellDefinition> { definition });
		}

		private T Create<T>() where T : ScriptableObject
		{
			T instance = ScriptableObject.CreateInstance<T>();
			createdObjects.Add(instance);
			return instance;
		}
	}
}
