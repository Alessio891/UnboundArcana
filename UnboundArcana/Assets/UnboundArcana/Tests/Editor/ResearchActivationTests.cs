using NUnit.Framework;
using UnboundArcana.Core.Research;
using UnboundArcana.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Tests
{
	public class ResearchActivationTests
	{
		private ResearchDefinition firstDefinition;
		private ResearchDefinition secondDefinition;

		[SetUp]
		public void SetUp()
		{
			firstDefinition = CreateDefinition(RunModifierStat.SpellDamage, 10f);
			secondDefinition = CreateDefinition(RunModifierStat.SpellSpeed, 20f);
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(firstDefinition);
			Object.DestroyImmediate(secondDefinition);
		}

		[Test]
		public void ClearingExpeditionProgressRemovesAccumulatedMinorRewards()
		{
			PlayerState player = new(null);
			player.AddMinorReward(firstDefinition);
			player.AddMinorReward(secondDefinition);

			player.ClearExpeditionProgress();

			Assert.That(player.Modifiers, Is.Empty);
			Assert.That(player.Researches, Is.Empty);
			Assert.That(player.Knowledge, Is.Zero);
		}

		[Test]
		public void MinorRewardAppliesImmediatelyWithoutResearchProgress()
		{
			PlayerState player = new(null);

			player.AddMinorReward(firstDefinition);

			Assert.That(player.Modifiers, Has.Count.EqualTo(1));
			Assert.That(player.Modifiers[0].Stat, Is.EqualTo(RunModifierStat.SpellDamage));
			Assert.That(player.Modifiers[0].Source, Is.SameAs(firstDefinition));
			Assert.That(player.Researches, Is.Empty);
		}

		[Test]
		public void RepeatedActivationAppliesCompletedResearchOnce()
		{
			PlayerState player = new(null);
			ResearchInstance research = AddCompletedResearch(player, firstDefinition);

			Assert.That(player.TryActivateResearch(research), Is.True);
			Assert.That(player.TryActivateResearch(research), Is.False);
			Assert.That(player.TryActivateResearch(research), Is.False);
			Assert.That(player.Modifiers, Has.Count.EqualTo(1));
			Assert.That(player.Modifiers[0].Source, Is.SameAs(research));
		}

		[Test]
		public void DistinctCompletedResearchesEachApplyOnce()
		{
			PlayerState player = new(null);
			ResearchInstance first = AddCompletedResearch(player, firstDefinition);
			ResearchInstance second = AddCompletedResearch(player, secondDefinition);

			Assert.That(player.TryActivateResearch(first), Is.True);
			Assert.That(player.TryActivateResearch(second), Is.True);
			Assert.That(player.TryActivateResearch(first), Is.False);
			Assert.That(player.TryActivateResearch(second), Is.False);
			Assert.That(player.Modifiers, Has.Count.EqualTo(2));
		}

		[Test]
		public void NewlyCompletedResearchActivatesAfterEarlierRepeatedCalls()
		{
			PlayerState player = new(null);
			ResearchInstance first = AddCompletedResearch(player, firstDefinition);
			ResearchInstance second = new(secondDefinition);
			player.Researches.Add(second);

			Assert.That(player.TryActivateResearch(first), Is.True);
			Assert.That(player.TryActivateResearch(second), Is.False);
			Assert.That(player.TryActivateResearch(first), Is.False);

			second.AddKnowledge(secondDefinition.RequiredKnowledge);

			Assert.That(player.TryActivateResearch(second), Is.True);
			Assert.That(player.Modifiers, Has.Count.EqualTo(2));
		}

		[Test]
		public void NewPlayerStateHasNoPreviousResearchModifiers()
		{
			PlayerState previousRun = new(null);
			ResearchInstance research = AddCompletedResearch(previousRun, firstDefinition);
			previousRun.TryActivateResearch(research);

			PlayerState newRun = new(null);

			Assert.That(previousRun.Modifiers, Has.Count.EqualTo(1));
			Assert.That(newRun.Researches, Is.Empty);
			Assert.That(newRun.Modifiers, Is.Empty);
		}

		private static ResearchInstance AddCompletedResearch(PlayerState player, ResearchDefinition definition)
		{
			ResearchInstance research = new(definition);
			research.AddKnowledge(definition.RequiredKnowledge);
			player.Researches.Add(research);
			return research;
		}

		private static ResearchDefinition CreateDefinition(RunModifierStat stat, float value)
		{
			ResearchDefinition definition = ScriptableObject.CreateInstance<ResearchDefinition>();
			SerializedObject serialized = new(definition);
			serialized.FindProperty("requiredKnowledge").intValue = 100;
			serialized.FindProperty("modifierStat").enumValueIndex = (int)stat;
			serialized.FindProperty("modifierOperation").enumValueIndex = (int)RunModifierOperation.Percent;
			serialized.FindProperty("modifierValue").floatValue = value;
			serialized.ApplyModifiedPropertiesWithoutUndo();
			return definition;
		}
	}
}
