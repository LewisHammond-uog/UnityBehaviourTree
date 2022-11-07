using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Blackboard
{
	internal class Entry
	{
		private string name;
		private float lastUpdateTime;
		private object value;
		private Type type;

		public Entry(string name, object value)
		{
			this.name = name;
			UpdateValue(value);
		}

		public void UpdateValue(object value)
		{
			this.value = value;
			this.type = value.GetType();
			lastUpdateTime = Time.time;
		}

		public float GetAge()
		{
			return Time.time - lastUpdateTime;
		}

		public T GetValueAs<T>()
		{
			return (T) value;
		}

		public bool IsValueOfType<T>()
		{
			return type == typeof(T);
		}

		public bool IsValueOfType(Type type)
		{
			return type == this.type;
		}
	}
}