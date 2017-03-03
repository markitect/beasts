using System;
using UnityEngineInternal.Input;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
	// Listens for native events and converts them into managed InputEvent instances.
	internal class NativeInputEventManager
	{
		internal IInputEventManager m_EventManager;
		internal INativeInputDeviceManager m_NativeDeviceManager;
		private bool m_IsInitialized;

		public InputStats stats { get; set; }
		public Action onReceivedEvents { get; set; }
		public Action onReceivedEventsLateUpdate { get; set; }

		internal void Initialize(IInputEventManager eventManager, INativeInputDeviceManager nativeDeviceManager)
		{
			if (m_IsInitialized)
				return;

			m_EventManager = eventManager;
			m_NativeDeviceManager = nativeDeviceManager;

			NativeInputSystem.onEvents += OnReceiveEvents;        


			m_IsInitialized = true;
		}

		internal void Uninitialize()
		{
			if (!m_IsInitialized)
				return;

			m_EventManager = null;
			m_NativeDeviceManager = null;
			NativeInputSystem.onEvents -= OnReceiveEvents;            

			m_IsInitialized = false;
		}

		// This method reads out NativeInputEvents directly from unmanaged memory
		// and turns them into InputEvent instances and puts them on the InputEventQueue.
		internal void ProcessEvents(int eventCount, IntPtr eventData)
		{
			var queue = m_EventManager.queue;
			var pool = m_EventManager.pool;
			var zeroTime = NativeInputSystem.zeroEventTime;
			var currentTime = Time.time;
	#if UNITY_EDITOR
			var currentRealTime = EditorApplication.timeSinceStartup;
	#else
			var currentRealTime = Time.realtimeSinceStartup;
	#endif

			////TODO: disconnect/reconnect events
			////TODO: text events

			var currentDataPtr = eventData;
			for (var i = 0; i < eventCount; ++i)
			{
				unsafe
				{
					NativeInputEvent* eventPtr = (NativeInputEvent*)currentDataPtr;

					// In the editor, we have jumps in time progression as time will reset when going in and out of play mode.
					// This means that when adjusting from real time to game time here, we may end up with events that have happened
					// "before time started." We simply discard those events.

					var eventTime = eventPtr->time;
					var time = eventTime - zeroTime;
					var device = m_NativeDeviceManager.FindInputDeviceByNativeDeviceId(eventPtr->deviceId);

					////REVIEW: This is a downside of the current event representation. Relying primarily on type+index allows events
					////   to be routed through the system regardless of whether the endpoint exists or not -- which IMO is a good thing.
					if (device != null && time >= 0.0)
					{
						switch (eventPtr->type)
						{
							// KeyboardEvent.
							case NativeInputEventType.KeyDown:
							case NativeInputEventType.KeyUp:
							case NativeInputEventType.KeyRepeat:
								{
									NativeKeyEvent* nativeKeyEvent = (NativeKeyEvent*)eventPtr;
									var inputEvent = pool.ReuseOrCreate<KeyboardEvent>();
									inputEvent.time = time;
									inputEvent.device = device;
									inputEvent.key = nativeKeyEvent->key;
									inputEvent.isDown = (eventPtr->type == NativeInputEventType.KeyDown || eventPtr->type == NativeInputEventType.KeyRepeat);
									inputEvent.isRepeat = (eventPtr->type == NativeInputEventType.KeyRepeat);
									inputEvent.modifiers = (ModifierKeys)nativeKeyEvent->modifiers;
									queue.Enqueue(inputEvent);
								}
								break;

							// PointerDownEvent, PointerMoveEvent, PointerUpEvent.
							case NativeInputEventType.PointerDown:
								{
									NativePointerEvent* nativePointerEvent = (NativePointerEvent*)eventPtr;
									var inputEvent = pool.ReuseOrCreate<PointerDownEvent>();
									inputEvent.time = time;
									inputEvent.device = device;
									inputEvent.pointerId = nativePointerEvent->pointerId;
									inputEvent.position = nativePointerEvent->position;
									inputEvent.pressure = nativePointerEvent->pressure;
									inputEvent.rotation = nativePointerEvent->rotation;
									inputEvent.tilt = nativePointerEvent->tilt;
									inputEvent.radius = nativePointerEvent->radius;
									inputEvent.distance = nativePointerEvent->distance;
									inputEvent.displayIndex = nativePointerEvent->displayIndex;
									queue.Enqueue(inputEvent);
								}
								break;
							case NativeInputEventType.PointerUp:
								{
									NativePointerEvent* nativePointerEvent = (NativePointerEvent*)eventPtr;
									var inputEvent = pool.ReuseOrCreate<PointerUpEvent>();
									inputEvent.time = time;
									inputEvent.device = device;
									inputEvent.pointerId = nativePointerEvent->pointerId;
									inputEvent.position = nativePointerEvent->position;
									inputEvent.pressure = nativePointerEvent->pressure;
									inputEvent.rotation = nativePointerEvent->rotation;
									inputEvent.tilt = nativePointerEvent->tilt;
									inputEvent.radius = nativePointerEvent->radius;
									inputEvent.distance = nativePointerEvent->distance;
									inputEvent.displayIndex = nativePointerEvent->displayIndex;
									queue.Enqueue(inputEvent);
								}
								break;
							case NativeInputEventType.PointerMove:
								{
									NativePointerEvent* nativePointerEvent = (NativePointerEvent*)eventPtr;
									var inputEvent = pool.ReuseOrCreate<PointerMoveEvent>();
									inputEvent.time = time;
									inputEvent.device = device;
									inputEvent.pointerId = nativePointerEvent->pointerId;
									inputEvent.position = nativePointerEvent->position;
									inputEvent.delta = nativePointerEvent->delta;
									inputEvent.pressure = nativePointerEvent->pressure;
									inputEvent.rotation = nativePointerEvent->rotation;
									inputEvent.tilt = nativePointerEvent->tilt;
									inputEvent.radius = nativePointerEvent->radius;
									inputEvent.distance = nativePointerEvent->distance;
									inputEvent.displayIndex = nativePointerEvent->displayIndex;
									queue.Enqueue(inputEvent);
								}
								break;

							// GenericControlEvent.
							case NativeInputEventType.Generic:
								{
									NativeGenericEvent* nativeGenericEvent = (NativeGenericEvent*)eventPtr;

									var inputEvent = pool.ReuseOrCreate<GenericControlEvent<float>>();
									inputEvent.time = time;
									inputEvent.device = device;
									inputEvent.controlIndex = nativeGenericEvent->controlIndex;
									////REVIEW: really just take scaled value and discard raw value? also float and not double?
									inputEvent.value = (float)nativeGenericEvent->scaledValue;
									queue.Enqueue(inputEvent);

								}
								break;

							// TrackingEvent.
							case NativeInputEventType.Tracking:
								{
									NativeTrackingEvent* nativeTrackingEvent = (NativeTrackingEvent*)eventPtr;
									var inputEvent = pool.ReuseOrCreate<TrackingEvent>();
									inputEvent.time = time;
									inputEvent.device = device;
									inputEvent.nodeId = nativeTrackingEvent->nodeId;
									inputEvent.localPosition = nativeTrackingEvent->localPosition;
									inputEvent.localRotation = nativeTrackingEvent->localRotation;
									queue.Enqueue(inputEvent);
								}
								break;

							// Unrecognized event. Skip.
							default:
								break;
						}
					}

					currentDataPtr = new IntPtr(currentDataPtr.ToInt64() + eventPtr->sizeInBytes);
				}
			}

			if (stats != null && stats.enabled)
			{
				stats.currentNumNativeEvents += eventCount;
				stats.currentSizeofNativeEventDataInBytes += (int)(currentDataPtr.ToInt64() - eventData.ToInt64());
			}
		}

		internal void OnReceiveEvents(int eventCount, IntPtr eventData)
		{
			ProcessEvents(eventCount, eventData);

			if (onReceivedEvents != null)
				onReceivedEvents();
		}
	}
}

