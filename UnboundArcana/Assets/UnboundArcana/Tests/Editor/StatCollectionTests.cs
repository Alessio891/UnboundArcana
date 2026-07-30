using NUnit.Framework;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Tests
{
	public class StatCollectionTests
	{
		[Test]
		public void GetAppliesModifiersInInsertionOrder()
		{
			StatCollection stats = new();
			object source = new();

			stats.AddBase("damage", 10f, source);
			stats.AddModifier(new StatModifier("damage", 5f, ModifierOperation.Flat, source));
			stats.AddModifier(new StatModifier("damage", 0.5f, ModifierOperation.Percent, source));
			stats.AddModifier(new StatModifier("damage", 2f, ModifierOperation.Multiplier, source));

			Assert.That(stats.Get("damage"), Is.EqualTo(45f));
		}

		[Test]
		public void GetIgnoresModifiersForOtherStats()
		{
			StatCollection stats = new();
			object source = new();

			stats.AddBase("damage", 10f, source);
			stats.AddModifier(new StatModifier("speed", 5f, ModifierOperation.Flat, source));

			Assert.That(stats.Get("damage"), Is.EqualTo(10f));
		}

		[Test]
		public void RemoveModifiersFromSourceRemovesBaseAndRuntimeModifiers()
		{
			StatCollection stats = new();
			object removedSource = new();
			object retainedSource = new();

			stats.AddBase("damage", 10f, removedSource);
			stats.AddBase("damage", 3f, retainedSource);
			stats.AddModifier(new StatModifier("damage", 5f, ModifierOperation.Flat, removedSource));
			stats.AddModifier(new StatModifier("damage", 2f, ModifierOperation.Flat, retainedSource));

			stats.RemoveModifiersFromSource(removedSource);

			Assert.That(stats.Get("damage"), Is.EqualTo(5f));
		}
	}
}
