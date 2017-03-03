using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityEngine.Experimental.Input
{
	internal class InputDeviceProfileManager
	{
		private List<InputDeviceProfile> m_Profiles = new List<InputDeviceProfile>();
		public IEnumerable<InputDeviceProfile> profiles { get { return m_Profiles; } }

		public void RegisterProfile(InputDeviceProfile profile)
		{
			if (!m_Profiles.Contains(profile))
				m_Profiles.Add(profile);
		}

		/// <summary>
		/// Find the profile that should be used for a device with the given device string.
		/// </summary>
		public InputDeviceProfile FindProfileByDeviceString(string deviceString)
		{
			////TODO: add matching for deviceNames

			// Check for exact match from device names.
			foreach (var profile in m_Profiles)
			{
				var deviceNameRegexes = profile.matchingDeviceRegexes;
				if (!string.IsNullOrEmpty(profile.neverMatchDeviceRegex)
					&& Regex.IsMatch(deviceString, profile.neverMatchDeviceRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline))
					continue;
				if (deviceNameRegexes != null)
				{
					foreach (var regex in deviceNameRegexes)
					{
						if (Regex.IsMatch(deviceString, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline))
							return profile;
					}
				}
			}

			// Find a profile with a last resort match.
			foreach (var profile in m_Profiles)
			{
				var lastResortRegex = profile.lastResortDeviceRegex;
				if (!string.IsNullOrEmpty(lastResortRegex))
				{
					if (!string.IsNullOrEmpty(profile.neverMatchDeviceRegex)
						&& Regex.IsMatch(deviceString, profile.neverMatchDeviceRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline))
						continue;

					if (Regex.IsMatch(deviceString, lastResortRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline))
						return profile;
				}
			}

			// No match. Normally means we should ignore the device.
			return null;
		}

		// When resetting the input system, we pass on profiles from the old profile manager
		// to the new one.
		internal void StealProfilesFrom(InputDeviceProfileManager manager)
		{
			m_Profiles = manager.m_Profiles;
			manager.m_Profiles = null;
		}
	}
}
