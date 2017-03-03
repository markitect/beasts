using System;

namespace UnityEngine.Experimental.Input
{
	[Serializable]
	internal class InputEventManager : IInputEventManager
	{
		[SerializeField]
		private InputEventQueue m_Queue = new InputEventQueue();
		public InputEventQueue queue { get { return m_Queue; } }
		IInputEventQueue IInputEventManager.queue { get { return queue; } }

		// Not serialized. Pool will start cold after each domain load.
		private InputEventPool m_Pool = new InputEventPool();
		public InputEventPool pool { get { return m_Pool; } }
		IInputEventPool IInputEventManager.pool { get { return pool; } }

		// Not serialized. Handlers will have to re-register themselves.
		public InputHandlerNode handlerRoot { get; private set; }

		// TODO: Review. Move these somewhere else so they're not part of core system?
		public InputHandlerNode consumers { get; private set; }
		public InputHandlerNode assignedPlayers { get; private set; }
		public InputHandlerNode globalPlayers { get; private set; }

		public InputStats stats { get; set; }

		public InputEventManager()
		{
			handlerRoot = new InputHandlerNode();

			consumers = new InputHandlerNode();
			handlerRoot.children.Add(consumers);

			assignedPlayers = new InputHandlerNode();
			consumers.children.Add(assignedPlayers);

			// Global consumers should be processed last.
			globalPlayers = new InputHandlerNode();
			consumers.children.Add(globalPlayers);
		}

		public void ExecuteEvents(double upToAndIncludingTime)
		{
			var processedEventCount = 0;
			var playerHasFocus = Application.isFocused;

			InputEvent inputEvent;
			while (m_Queue.Dequeue(upToAndIncludingTime, out inputEvent))
			{
				if (!playerHasFocus || handlerRoot.ProcessEvent(inputEvent))
					pool.Return(inputEvent);
				++processedEventCount;
			}

			if (stats != null && stats.enabled)
				stats.currentNumEventsProcessed += processedEventCount;
		}

		public void ExecuteEventsByType<T>(double upToAndIncludingTime)
		{
			var processedEventCount = 0;
			var playerHasFocus = Application.isFocused;

			InputEvent inputEvent;
			while (m_Queue.DequeueByType<T>(upToAndIncludingTime, out inputEvent))
			{
				if (!playerHasFocus || handlerRoot.ProcessEvent(inputEvent))
					pool.Return(inputEvent);
				++processedEventCount;
			}

			if (stats != null && stats.enabled)
				stats.currentNumEventsProcessed += processedEventCount;
		}
	}
}
