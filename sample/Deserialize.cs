/*
*
*	Deserialize.cs
*	NJson
*
*	Created by Nícolas Reichert.
*
*/

using UnityEngine;
using System.Collections;
using NPlugins.Json;
using System.Collections.Generic;
using System;

/// <summary>
/// 	Simple implementation for deserializing a JSON into a custom class.
/// 
/// 	Note that it's crucial that the JSON's keys has the exact same
/// 	name (including upper/lower case) as the class' properties.
/// </summary>
public class Deserialize : MonoBehaviour
{
	void Start()
	{
		Debug.Log(json);
		
		Model model = Json.Deserialize<Model>(json);
		
		Debug.Log(model.MySmallClassListValue.Count);
	}

	private string json = 
		"{" +
			"\"MyIntValue\":12,"+
			"\"MyFloatValue\":16.7777,"+
			"\"MyStringValue\":\"This is a string\","+
			"\"MyBoolValue\":\"false\"," +
			"\"MyVector3\":{\"x\":2,\"y\":0,\"z\":1}," +
			"\"MyListOfStringValue\":["+
				"\"string 1\","+
				"\"string 2\","+
				"\"string 3\","+
				"\"string 4\""+
			"],"+
			"\"MySmallClassListValue\":[{ "+ 
				"\"MyIntValue\":3,"+
				"\"MyFloatValue\":4.5," +
				"\"MyDateTimeValue\":\"04/03/2010\""+
			"}," +
			"{" + 
				"\"MyIntValue\":3,"+
				"\"MyFloatValue\":4.5," +
				"\"MyDateTimeValue\":\"04/03/2011\""+
			"}," +
			"{" + 
				"\"MyIntValue\":3,"+
				"\"MyFloatValue\":4.5," +
				"\"MyDateTimeValue\":\"04/03/2012\""+
			"}," +
			"{" + 
				"\"MyIntValue\" : 3,"+
				"\"MyFloatValue\" : 4.5," +
				"\"MyDateTimeValue\" : \"04/03/2013\""+
			"}," +
			"{" + 
				"\"MyIntValue\":3,"+
				"\"MyFloatValue\":4.5," +
				"\"MyDateTimeValue\":\"04/03/2014\""+
			"}]," + 
			"\"MySmallClassValue\":{"+
				"\"MyIntValue\":3,"+
				"\"MyFloatValue\":4.5," +
				"\"MyDateTimeValue\":\"04/03/2015\""+
		    "}"+
		"}";

	private class Model
	{
		public int MyIntValue { get; set; }
		
		public float MyFloatValue { get; set; }
		
		public string MyStringValue { get; set; }

		public bool MyBoolValue { get; set; } 

		public Vector3 MyVector3 { get; set; }
		
		public string[] MyListOfStringValue { get; set; }

		public List<SmallClass> MySmallClassListValue { get; set; }

		public SmallClass MySmallClassValue { get; set; }
	}
	
	private class SmallClass
	{
		public int MyIntValue { get; set; }
		
		public float MyFloatValue { get; set; }
		
		public DateTime MyDateTimeValue { get; set; }
	}
}
