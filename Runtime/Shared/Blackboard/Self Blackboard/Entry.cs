using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Entry
{
	private string name;
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
	}

	public T GetValueAs<T>()
	{
		return (T)value;
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
