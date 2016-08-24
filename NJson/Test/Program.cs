using System;
using System.Collections.Generic;
using NPlugins.Json;
using UnityEngine;

namespace Test
{
    class Program
    {
        static void Main()
        {
            string json = "{\"MyIntValue\":1250,\"MyBoolValue\":\"False\",\"MyFloatValue\":1.245,\"MyStringValue\":\"n" + 
"ull\",\"MyListOfStringValue\":[[\"a\",\"b\",\"c\"],[\"d\",\"e\",\"f\"],[\"g\",\"h\",\"i\"]],\"MySmallClassValue\":[[{\"MyIntValue\":20,\"MyFloatValue\":20.5,\"MyDateTimeValue\":\"2015-04-07" + 
" 00:20:12\"}],[{\"MyIntValue\":20,\"MyFloatValue\":20.5,\"MyDateTimeValue\":\"2015-04-07" + 
" 00:20:12\"}],[{\"MyIntValue\":20,\"MyFloatValue\":20.5,\"MyDateTimeValue\":\"2015-04-07" + 
" 00:20:12\"}]],\"Vector3\":{\"x\":1.2,\"y\":2.3,\"width\":3.4,\"height\":4.5}}";

            var model = Json.Deserialize<Model>(json);

            Console.WriteLine(model.Vector3);

            Console.ReadLine();
        }
    }

    public class Model
    {
        public int MyIntValue { get; set; }

        public bool MyBoolValue { get; set; }

        public float MyFloatValue { get; set; }

        public string MyStringValue { get; set; }

   		public List<List<string>> MyListOfStringValue { get; set; }

        public Rect Vector3 { get; set; }
    }

    public class SmallClass
    {
        public int MyIntValue { get; set; }

        public float MyFloatValue { get; set; }

        public DateTime MyDateTimeValue { get; set; }
    }
}
