﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Utilities
{
	public static class SerializationHelper
	{
		[Serializable]
		public struct TypeSerializationInfo
		{
			[SerializeField]
			public string fullName;

			[SerializeField]
			public string assemblyName;

			public bool IsValid()
			{
				return !string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(assemblyName);
			}

			public string SearchString()
			{
				if (!IsValid())
					return string.Empty;

				return string.Format("{0}, {1}", fullName, assemblyName);
			}
		}

		[Serializable]
		public struct JSONSerializedElement
		{
			[SerializeField]
			public TypeSerializationInfo typeInfo;

			[SerializeField]
			public string JSONnodeData;
		}

		private static TypeSerializationInfo GetTypeSerializableAsString(Type type)
		{
			return new TypeSerializationInfo
			{
				fullName = type.FullName,
				assemblyName = type.Assembly.GetName().Name
			};
		}

		private static Type GetTypeFromSerializedString(TypeSerializationInfo typeInfo)
		{
			if (!typeInfo.IsValid())
				return null;

			return Type.GetType(typeInfo.SearchString());
		}

		public static List<JSONSerializedElement> Serialize<T>(IEnumerable<T> list)
		{
			var result = new List<JSONSerializedElement>();
			if (list == null)
				return result;

			foreach (var element in list)
			{
				if (element == null)
				{
					result.Add(new JSONSerializedElement());
					continue;
				}

				var typeInfo = GetTypeSerializableAsString(element.GetType());
				var data = JsonUtility.ToJson(element, true);

				if (string.IsNullOrEmpty(data))
				{
					result.Add(new JSONSerializedElement());
					continue;
				}

				result.Add(new JSONSerializedElement()
				{
					typeInfo = typeInfo,
					JSONnodeData = data
				});
			}
			return result;
		}

		public static List<T> Deserialize<T>(IEnumerable<JSONSerializedElement> list, params object[] constructorArgs) where T : class 
		{
			var result = new List<T>();
			if (list == null)
				return result;

			foreach (var element in list) 
			{
				if (!element.typeInfo.IsValid() || string.IsNullOrEmpty(element.JSONnodeData))
				{
					result.Add(null);
					continue;
				}

				var type = GetTypeFromSerializedString(element.typeInfo);
				if (type == null)
				{
					Debug.LogWarningFormat("Could not find node of type {0} in loaded assemblies", element.typeInfo.SearchString());
					result.Add(null);
					continue;
				}

				T instance;
				try
				{ 
					instance = Activator.CreateInstance(type, constructorArgs) as T;
				}
				catch (Exception e)
				{
					Debug.LogWarningFormat("Could not construct instance of: {0} - {1}", type, e);
					result.Add(null);
					continue;
				}

				if (instance != null)
				{
					JsonUtility.FromJsonOverwrite(element.JSONnodeData, instance);
					result.Add(instance);
				}
			}
			return result;  
		}

		public static JSONSerializedElement SerializeObj<T>(T obj)
		{
			if (obj == null)
				return new JSONSerializedElement();

			var typeInfo = GetTypeSerializableAsString(obj.GetType());
			var data = JsonUtility.ToJson(obj, true);

			if (string.IsNullOrEmpty(data))
				return new JSONSerializedElement();

			return new JSONSerializedElement()
			{
				typeInfo = typeInfo,
				JSONnodeData = data
			};
		}

		public static T DeserializeObj<T>(JSONSerializedElement obj, params object[] constructorArgs) where T : class 
		{
			if (!obj.typeInfo.IsValid() || string.IsNullOrEmpty(obj.JSONnodeData))
				return null;

			var type = GetTypeFromSerializedString(obj.typeInfo);
			if (type == null)
			{
				Debug.LogWarningFormat("Could not find node of type {0} in loaded assemblies", obj.typeInfo.SearchString());
				return null;
			}

			T instance;
			try
			{ 
				instance = Activator.CreateInstance(type, constructorArgs) as T;
			}
			catch (Exception e)
			{
				Debug.LogWarningFormat("Could not construct instance of: {0} - {1}", type, e);
				return null;
			}

			if (instance != null)
			{
				JsonUtility.FromJsonOverwrite(obj.JSONnodeData, instance);
				return instance;
			}
			
			return null;
		} 
	}
}
