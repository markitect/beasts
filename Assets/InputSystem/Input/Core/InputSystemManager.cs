using System;
using UnityEngineInternal.Input;

namespace UnityEngine.Experimental.Input
{
	// Contains the global state for the input system. Must be able
	// to survive a domain reload while carrying its state over intact.
	internal class InputSystemManager : ScriptableObject
	{
		public InputDeviceManager deviceManager;
		public InputEventManager eventManager;
		public NativeInputDeviceManager nativeDeviceManager;

		private NativeInputUpdateType updateType { get; set; }

		// Device profiles register themselves on every domain load. This happens before
		// heap state gets restored in the editor. Given that persisting the profile
		// manager would require making all profiles serializable and given that profiles
		// register themselves on startup, we don't make the profile manager part of the
		// serialized input system state and rather just create it on demand.
		[NonSerialized] 
		private InputDeviceProfileManager m_ProfileManager;
		public InputDeviceProfileManager profileManager
		{
			get
			{
				if (m_ProfileManager == null)
					m_ProfileManager = new InputDeviceProfileManager();
				return m_ProfileManager;
			}
		}

		// This one does not contain any state we need to persist across reloads.
		[NonSerialized]
		public NativeInputEventManager nativeEventManager;

		[NonSerialized]
		private InputStats m_InputStats;
		public InputStats stats
		{
			get
			{
				if (m_InputStats == null)
				{
					m_InputStats = new InputStats();
					eventManager.stats = m_InputStats;
					nativeEventManager.stats = m_InputStats;
				}

				return m_InputStats;
			}
		}

		[NonSerialized]
		private bool m_IsInitialized;

		// The amount of time the virtual unscaled time lags behind the current real time.
		// Events are executed up to and including the current unscaled time,
		// meaning those with a timestamp higher than the unscaled time are postponed.
		// We do this to get proper distribution over multiple FixedUpdate calls.
		// However, since unscaled time lags behind realtime, we can get a little lag this way
		// (though it's typically a small fraction of a frame; much less than the lag from
		// Update callbacks to rendering.)
		// Nevertheless, we keep track of the lag and offset the timestamps by this lag
		// when comparing against unscaled time, such that we can eliminate this lag entirely.
		// We record what the lag is just before processing dynamic update events, such that
		// we're guaranteed to catch all events in the dynamic update input processing call,
		// which is the last one of the frame.
		// The same recorded lag is used in the subsequent fixed update calls of the next
		// frame. This won't give a 100% correct distribution if the next frame delta time
		// turns out to be shorter or longer, but it's a decent estimate and the real thing
		// can't be known in advance.
		private float m_VirtualTimeLag;

		public void OnEnable()
		{
			if (deviceManager == null)
				deviceManager = new InputDeviceManager();

			if (eventManager == null)
				eventManager = new InputEventManager();

			if (nativeDeviceManager == null)
				nativeDeviceManager = new NativeInputDeviceManager();

			if (!m_IsInitialized)
			{
				// Reconnect devices to their profiles.
				foreach (var device in deviceManager.devices)
				{
					if (device.profile == null)
					{
						InputDeviceProfile profile = profileManager.FindProfileByDeviceString(device.displayName);
						if (profile != null)
							device.SetupFromProfile(profile);
						else
							device.SetupWithoutProfile();
					}
				}

				nativeDeviceManager.Initialize(deviceManager, profileManager);

				nativeEventManager = new NativeInputEventManager();
				nativeEventManager.Initialize(eventManager, nativeDeviceManager);
				nativeEventManager.onReceivedEvents += OnProcessEvents;                

				eventManager.handlerRoot.children.Insert(0,
					new InputHandlerCallback { processEvent = deviceManager.RemapEvent });
				eventManager.handlerRoot.children.Insert(1, deviceManager);

				NativeInputSystem.onUpdate += OnUpdate;

				updateType = NativeInputUpdateType.EndBeforeRender;
				m_IsInitialized = true;
			}
		}

		private void OnUpdate(NativeInputUpdateType requestedUpdateType)
		{
			updateType = requestedUpdateType;
			switch (updateType)
			{
				case NativeInputUpdateType.BeginFixed:
					eventManager.handlerRoot.BeginUpdate();
					break;

				case NativeInputUpdateType.EndFixed:
					eventManager.handlerRoot.EndUpdate();
					break;

				case NativeInputUpdateType.BeginDynamic:
					eventManager.handlerRoot.BeginUpdate();
					break;

				case NativeInputUpdateType.EndDynamic:
					eventManager.handlerRoot.EndUpdate();
					break;
				case NativeInputUpdateType.BeginBeforeRender:       
					// we only require that the update type is correctly set duiring late begin/end pairs.             
					break;
				case NativeInputUpdateType.EndBeforeRender:                    
					// we only require that the update type is correctly set during late begin/end pairs
					break;
			}
		}

		private void OnProcessEvents()
		{
			if (!Time.inFixedTimeStep)
				m_VirtualTimeLag = Time.realtimeSinceStartup - Time.unscaledTime;

			if(updateType != NativeInputUpdateType.BeginBeforeRender)
			{
				eventManager.ExecuteEvents(Time.unscaledTime + m_VirtualTimeLag);                
			}
			else
			{
				// we only want to update tracking events during the late update.
				eventManager.ExecuteEventsByType<TrackingEvent>(Time.unscaledTime + m_VirtualTimeLag);
			}
		}
	}
}
