using NUnit.Framework;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Tests
{
	public class GameEventBusTests
	{
		[Test]
		public void PublishInvokesSubscribedListener()
		{
			GameEventBus eventBus = new();
			int receivedValue = 0;

			eventBus.Subscribe<int>(value => receivedValue = value);
			eventBus.Publish(42);

			Assert.That(receivedValue, Is.EqualTo(42));
		}

		[Test]
		public void UnsubscribeStopsInvokingListener()
		{
			GameEventBus eventBus = new();
			int invocationCount = 0;
			System.Action<int> listener = _ => invocationCount++;

			eventBus.Subscribe(listener);
			eventBus.Unsubscribe(listener);
			eventBus.Publish(42);

			Assert.That(invocationCount, Is.Zero);
		}

		[Test]
		public void ListenerCanUnsubscribeDuringPublish()
		{
			GameEventBus eventBus = new();
			int invocationCount = 0;
			System.Action<int> listener = null;
			listener = _ =>
			{
				invocationCount++;
				eventBus.Unsubscribe(listener);
			};

			eventBus.Subscribe(listener);
			eventBus.Publish(1);
			eventBus.Publish(2);

			Assert.That(invocationCount, Is.EqualTo(1));
		}
	}
}
