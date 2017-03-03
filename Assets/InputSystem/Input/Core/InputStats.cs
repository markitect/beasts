using System.Linq;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Input
{
	public class InputStats
	{
		public void Enable()
		{
			enabled = true;
		}
		
		public void EnableForNFrames()
		{
		}

		public void Disable()
		{
			enabled = false;
		}

		public void Clear()
		{
			m_Updates.Clear();
			m_FixedUpdates.Clear();
		}

		public void WriteToFileAsCSV(string path)
		{
		}

		internal void BeginNewUpdate(bool fixedUpdate)
		{
			if (!enabled)
				return;

			var list = fixedUpdate ? m_FixedUpdates : m_Updates;
			var currentRealTime = Time.realtimeSinceStartup;

			var realTimeSinceLastUpdate = 0.0;
			if (list.Count > 0)
				realTimeSinceLastUpdate = currentRealTime - list[list.Count - 1].realTimeSinceLastUpdate;

			var update = new PerUpdate
			{
				time = Time.time,
				realTime = currentRealTime,
				realTimeSinceLastUpdate = realTimeSinceLastUpdate
			};

			list.Add(update);
			m_CurrentUpdates = list;
		}

		public struct PerUpdate
		{
			public double time;
			public double realTime;
			public double realTimeSinceLastUpdate;
			public double minTimeInQueue;
			public double maxTimeInQueue;
			public double averageTimeInQueue;
			public int numEventsQueued;
			public int numEventsProcessed;
			public int numEventsInPool;
			public int numNativeEvents;
			public int sizeofNativeEventDataInBytes;

			internal PerUpdate SetNumEventsQueued(int value)
			{
				numEventsQueued = value;
				return this;
			}
			internal PerUpdate SetNumEventsProcessed(int value)
			{
				numEventsProcessed = value;
				return this;
			}
			internal PerUpdate SetNumNativeEvents(int value)
			{
				numNativeEvents = value;
				return this;
			}
			internal PerUpdate SetSizeofNativeEventDataInBytes(int value)
			{
				sizeofNativeEventDataInBytes = value;
				return this;
			}
		}

		public bool enabled { get; private set; }

		private List<PerUpdate> m_FixedUpdates = new List<PerUpdate>();
		public IEnumerable<PerUpdate> fixedUpdates
		{
			get { return m_FixedUpdates ?? Enumerable.Empty<PerUpdate>(); }
		}

		private List<PerUpdate> m_Updates = new List<PerUpdate>();
		public IEnumerable<PerUpdate> updates
		{
			get { return m_Updates ?? Enumerable.Empty<PerUpdate>(); }
		}

		private List<PerUpdate> m_CurrentUpdates;

		public PerUpdate currentUpdate
		{
			get
			{
				if (!enabled || m_CurrentUpdates.Count == 0)
					return new PerUpdate();

				return m_CurrentUpdates[m_CurrentUpdates.Count - 1];
			}
			private set
			{
				m_CurrentUpdates[m_CurrentUpdates.Count - 1] = value;
			}
		}

		public double currentTime
		{
			get { return currentUpdate.time; }
		}
		public double currentRealTime
		{
			get { return currentUpdate.realTime; }
		}
		public double currentRealTimeSinceLastUpdate
		{
			get { return currentUpdate.realTimeSinceLastUpdate; }
		}
		public int currentNumEventsQueued
		{
			get { return currentUpdate.numEventsQueued; }
			set { currentUpdate = currentUpdate.SetNumEventsQueued(value); }
		}
		public int currentNumEventsProcessed
		{
			get { return currentUpdate.numEventsProcessed; }
			set { currentUpdate = currentUpdate.SetNumEventsProcessed(value); }
		}
		public int currentNumNativeEvents
		{
			get { return currentUpdate.numNativeEvents; }
			set { currentUpdate = currentUpdate.SetNumNativeEvents(value); }
		}
		public int currentSizeofNativeEventDataInBytes
		{
			get { return currentUpdate.sizeofNativeEventDataInBytes;  }
			set { currentUpdate = currentUpdate.SetSizeofNativeEventDataInBytes(value); }
		}
	}
}
