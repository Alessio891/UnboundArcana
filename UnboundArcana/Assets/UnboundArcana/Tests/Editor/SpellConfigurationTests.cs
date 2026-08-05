using System.Collections.Generic;
using NUnit.Framework;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Behaviors.Beam;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Modules.SizeModifier;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Services;
using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Tests
{
	public class SpellConfigurationTests
	{
		private const string TemporaryFolder = "Assets/UnboundArcana/Tests/TempM03";
		private readonly List<Object> createdObjects = new();

		[SetUp]
		public void SetUp()
		{
			DeleteTemporaryFolder();
		}

		[TearDown]
		public void TearDown()
		{
			DeleteTemporaryFolder();

			foreach (Object createdObject in createdObjects)
			{
				if (createdObject != null)
				{
					Object.DestroyImmediate(createdObject);
				}
			}

			createdObjects.Clear();
		}

		[Test]
		public void EmptyConfigurationReturnsReadableValidationReason()
		{
			SpellConfigurationValidationResult result = new SpellConfiguration(null).Validate();

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Error, Is.EqualTo(SpellConfigurationValidationError.MissingBehavior));
			Assert.That(result.Reason, Is.EqualTo("Behavior slot is empty."));
		}

		[Test]
		public void BehaviorOnlyConfigurationIsValid()
		{
			SpellConfiguration configuration = CreateBehaviorOnlyConfiguration();

			Assert.That(configuration.Validate().IsValid, Is.True);
			Assert.That(configuration.ModuleCount, Is.EqualTo(0));
		}

		[Test]
		public void CompleteConfigurationIsValid()
		{
			SpellConfiguration configuration = CreateCompleteConfiguration();

			Assert.That(configuration.Validate().IsValid, Is.True);
			Assert.That(configuration.ModuleCount, Is.EqualTo(4));
		}

		[Test]
		public void DuplicateCategoryAdditionIsRejected()
		{
			SpellDefinition definition = Create<SpellDefinition>();
			definition.behavior = Create<BeamBehaviorDefinition>();
			definition.principle = CreateModule(SpellModuleType.Principle);
			SpellConfiguration configuration = new(definition);
			SpellModificationService modification = new(new GameEventBus());
			SpellModuleDefinition secondPrinciple = CreateModule(SpellModuleType.Principle);

			Assert.That(modification.TryAddModule(configuration, secondPrinciple), Is.False);
			Assert.That(configuration.Principle, Is.EqualTo(definition.principle));
		}

		[Test]
		public void WrongCategoryInSlotIsRejectedWithReadableReason()
		{
			SpellDefinition definition = Create<SpellDefinition>();
			definition.behavior = Create<BeamBehaviorDefinition>();
			definition.principle = CreateModule(SpellModuleType.Principle);
			definition.catalystA = CreateModule(SpellModuleType.Flux);

			SpellConfigurationValidationResult result = new SpellConfiguration(definition).Validate();

			Assert.That(result.Error, Is.EqualTo(SpellConfigurationValidationError.InvalidCatalystACategory));
			Assert.That(result.Reason, Is.EqualTo("Catalyst A slot requires a Catalyst module."));
		}

		[Test]
		public void ReplacementChangesOnlyTheRequestedSameCategorySlot()
		{
			SpellConfiguration configuration = CreateCompleteConfiguration();
			SpellModuleDefinition originalCatalystB = configuration.CatalystB;
			SpellModuleDefinition replacement = CreateModule(SpellModuleType.Catalyst);
			SpellModificationService modification = new(new GameEventBus());

			Assert.That(modification.TryReplaceModule(configuration, SpellConfigurationSlot.CatalystA, replacement), Is.True);
			Assert.That(configuration.CatalystA, Is.EqualTo(replacement));
			Assert.That(configuration.CatalystB, Is.EqualTo(originalCatalystB));
			Assert.That(configuration.Validate().IsValid, Is.True);
		}

		[Test]
		public void ReplacementRejectsADifferentCategory()
		{
			SpellConfiguration configuration = CreateCompleteConfiguration();
			SpellModuleDefinition replacement = CreateModule(SpellModuleType.Flux);
			SpellModificationService modification = new(new GameEventBus());

			Assert.That(modification.TryReplaceModule(configuration, SpellConfigurationSlot.CatalystA, replacement), Is.False);
		}

		[Test]
		public void EverySlotSurvivesAssetSerializationAndReload()
		{
			AssetDatabase.CreateFolder("Assets/UnboundArcana/Tests", "TempM03");
			BeamBehaviorDefinition behavior = CreateAsset<BeamBehaviorDefinition>("Behavior.asset");
			SpellModuleDefinition principle = CreateAsset<SizeModifierModuleDefinition>("Principle.asset", SpellModuleType.Principle);
			SpellModuleDefinition catalystA = CreateAsset<SizeModifierModuleDefinition>("CatalystA.asset", SpellModuleType.Catalyst);
			SpellModuleDefinition catalystB = CreateAsset<SizeModifierModuleDefinition>("CatalystB.asset", SpellModuleType.Catalyst);
			SpellModuleDefinition flux = CreateAsset<SizeModifierModuleDefinition>("Flux.asset", SpellModuleType.Flux);
			SpellDefinition definition = CreateAsset<SpellDefinition>("Spell.asset");
			definition.behavior = behavior;
			definition.principle = principle;
			definition.catalystA = catalystA;
			definition.catalystB = catalystB;
			definition.flux = flux;
			EditorUtility.SetDirty(definition);
			AssetDatabase.SaveAssets();
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(definition));

			SpellDefinition reloaded = AssetDatabase.LoadAssetAtPath<SpellDefinition>(TemporaryFolder + "/Spell.asset");
			SpellConfiguration configuration = new(reloaded);

			Assert.That(configuration.Behavior, Is.EqualTo(behavior));
			Assert.That(configuration.Principle, Is.EqualTo(principle));
			Assert.That(configuration.CatalystA, Is.EqualTo(catalystA));
			Assert.That(configuration.CatalystB, Is.EqualTo(catalystB));
			Assert.That(configuration.Flux, Is.EqualTo(flux));
			Assert.That(configuration.Validate().IsValid, Is.True);
		}

		[Test]
		public void FactoryBuildsBehaviorOnlyConfigurationAndRejectsMissingBehavior()
		{
			SpellConfiguration configuration = CreateBehaviorOnlyConfiguration();
			GameObject owner = new("SpellOwner");
			createdObjects.Add(owner);

			SpellInstance instance = SpellFactory.Create(configuration, new SpellRuntimeContext(null, new GameEventBus()), owner);
			SpellInstance invalidInstance = SpellFactory.Create(new SpellConfiguration(null), new SpellRuntimeContext(null, new GameEventBus()), owner);

			Assert.That(instance, Is.Not.Null);
			Assert.That(instance.modules, Is.Empty);
			Assert.That(invalidInstance, Is.Null);
			instance.Destroy();
		}

		private SpellConfiguration CreateBehaviorOnlyConfiguration()
		{
			SpellDefinition definition = Create<SpellDefinition>();
			definition.behavior = Create<BeamBehaviorDefinition>();
			return new SpellConfiguration(definition);
		}

		private SpellConfiguration CreateCompleteConfiguration()
		{
			SpellDefinition definition = Create<SpellDefinition>();
			definition.behavior = Create<BeamBehaviorDefinition>();
			definition.principle = CreateModule(SpellModuleType.Principle);
			definition.catalystA = CreateModule(SpellModuleType.Catalyst);
			definition.catalystB = CreateModule(SpellModuleType.Catalyst);
			definition.flux = CreateModule(SpellModuleType.Flux);
			return new SpellConfiguration(definition);
		}

		private SpellModuleDefinition CreateModule(SpellModuleType type)
		{
			SizeModifierModuleDefinition module = Create<SizeModifierModuleDefinition>();
			SetModuleType(module, type);
			return module;
		}

		private T Create<T>() where T : ScriptableObject
		{
			T instance = ScriptableObject.CreateInstance<T>();
			createdObjects.Add(instance);
			return instance;
		}

		private T CreateAsset<T>(string fileName, SpellModuleType? type = null) where T : ScriptableObject
		{
			T instance = Create<T>();
			if (instance is SpellModuleDefinition module && type.HasValue)
			{
				SetModuleType(module, type.Value);
			}

			AssetDatabase.CreateAsset(instance, TemporaryFolder + "/" + fileName);
			return instance;
		}

		private void SetModuleType(SpellModuleDefinition module, SpellModuleType type)
		{
			SerializedObject serialized = new(module);
			serialized.FindProperty("category").enumValueIndex = (int)type;
			serialized.ApplyModifiedPropertiesWithoutUndo();
		}

		private void DeleteTemporaryFolder()
		{
			if (AssetDatabase.IsValidFolder(TemporaryFolder))
			{
				AssetDatabase.DeleteAsset(TemporaryFolder);
				AssetDatabase.Refresh();
			}
		}
	}
}
