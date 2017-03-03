namespace UnityEngine.Experimental.Input
{
	[System.Serializable]
	public struct SupportedControl
	{
		public SerializableType controlType;
		public string standardName;
		public int hash;

		public static readonly SupportedControl none = new SupportedControl() { standardName = "None", hash = -1 };

		private SupportedControl(System.Type type, string standardName)
		{
			this.controlType = type;
			this.standardName = standardName;
			hash = standardName.GetHashCode() ^ type.Name.GetHashCode();
		}

		public override int GetHashCode()
		{
			return hash;
		}

		public override string ToString ()
		{
			return string.Format("{0} ({1})", standardName, controlType == null ? "null" : controlType.value.Name);
		}

		public static SupportedControl Get<T>(string standardName) where T : InputControl
		{
			return new SupportedControl(typeof(T), standardName);
		}
	}
}
