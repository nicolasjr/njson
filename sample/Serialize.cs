/*
*
*	Serialize.cs
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

public class Serialize : MonoBehaviour
{

	void Start()
	{
		// Create instance of SmallClass
		SmallClass mySmallClass = new SmallClass
									  {
										  MyIntValue = 3,
									   	  MyFloatValue = 4.5f,
										  MyVector3 = new Vector3(2, 0, 1),
									  	  MyDateTimeValue = DateTime.Now
									  };  

		// Create instace of Model 
		Model model = new Model
						  {
							  MyIntValue = 12,
							  MyBoolValue = true,
							  MyFloatValue = 16.7777f,
							  MyStringValue = "This is a string",
							  MyListOfStringValue = new [] { "string 1", "string 2", "string 3", "string 4" },
							  MySmallClassValue = mySmallClass
						  };

		Debug.Log(Json.Serialize(model));

	}

	private class Model
	{
		public int MyIntValue { get; set; }

		public bool MyBoolValue { get; set; }

		public float MyFloatValue { get; set; }

		public string MyStringValue { get; set; }

		public string[] MyListOfStringValue { get; set; }

		public SmallClass MySmallClassValue { get; set; }
	}

	private class SmallClass
	{
		public int MyIntValue { get; set; }
		
		public float MyFloatValue { get; set; }

		public Vector3 MyVector3 { get; set; } 

		public DateTime MyDateTimeValue { get; set; }
	}
	
}
