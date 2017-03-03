using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;
using UnityEngine;
using UnityEngineInternal.Input;

namespace UnityEngine.Experimental.Input
{
	public class Keyboard : InputDevice
	{
		public static Keyboard current { get { return InputSystem.GetCurrentDeviceOfType<Keyboard>(); } }

		public Keyboard() : this("Keyboard") { }

		public Keyboard(string displayName) : base(displayName) { }

		public override void AddStandardControls(ControlSetup setup)
		{
			int controlCount = EnumHelpers.GetValueCount<KeyCode>();
			for (var i = 0; i < controlCount; ++ i)
			{
				setup.AddControl(SupportedControl.Get<ButtonControl>(((KeyCode)i).ToString()));
			}
		}

		public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			if (base.ProcessEventIntoState(inputEvent, intoState))
				return true;
			
			var keyEvent = inputEvent as KeyboardEvent;
			if (keyEvent == null)
				return false;
			
			var control = intoState.controls[(int)keyEvent.key] as ButtonControl;
			if (!control.enabled)
				return false;
			
			control.SetValue(keyEvent.isDown ? 1 : 0);
			return true;
		}
		
		// Return the identifier of the current keyboard layout. The identifier is made up
		// of a locale name (like "en_US") and optionally a qualifier in parenthesis.
		// Example: "uk_UA" for Ukranian and "uk_UA (Extended)" for extended layout.
		public virtual string GetCurrentLayout()
		{
			throw new System.NotImplementedException();
		}

		public virtual bool GetKeyCodeInfoForCurrentLayout(KeyCode key, out KeyCodeInfo info)
		{
			if (nativeDeviceId != 0)
			{
				var controlIndex = (int) key;
				var controlConfiguration = NativeInputSystem.GetControlConfiguration(nativeDeviceId, controlIndex);

				if (!string.IsNullOrEmpty(controlConfiguration))
				{
					// Have to go through boxing to accomodate the JsonUtility API.
					object keyCodeInfo = new KeyCodeInfo();
					JsonUtility.FromJsonOverwrite(controlConfiguration, keyCodeInfo);
					info = (KeyCodeInfo) keyCodeInfo;
					return true;
				}
			}

			info = new KeyCodeInfo();
			return false;
		}

		public ButtonControl upArrow
		{
			get { return (ButtonControl) this[(int) KeyCode.UpArrow]; }
		}
		public ButtonControl downArrow
		{
			get { return (ButtonControl) this[(int) KeyCode.DownArrow]; }
		}
		public ButtonControl leftArrow
		{
			get { return (ButtonControl) this[(int) KeyCode.LeftArrow]; }
		}
		public ButtonControl rightArrow
		{
			get { return (ButtonControl) this[(int) KeyCode.RightArrow]; }
		}
		public ButtonControl aKey
		{
			get { return (ButtonControl) this[(int) KeyCode.A]; }
		}
		public ButtonControl dKey
		{
			get { return (ButtonControl) this[(int) KeyCode.D]; }
		}
		public ButtonControl sKey
		{
			get { return (ButtonControl) this[(int) KeyCode.S]; }
		}
		public ButtonControl wKey
		{
			get { return (ButtonControl) this[(int) KeyCode.W]; }
		}

		public ButtonControl leftControl
		{
			get { return (ButtonControl) this[(int) KeyCode.LeftControl]; }
		}
	}
}
