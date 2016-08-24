using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace NPlugins.Json
{
    public partial class Json
    {
        private class Serializer
        {
            public static Serializer Instance { get { return instance ?? (instance = new Serializer()); } }
            private static Serializer instance;

            private string dateTimeFormat;

            private Serializer()
            {
                this.dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            }

            public string SerializeClass<T>(T data)
            {
                // instantiate starting string with json
                string json = "{";

                // find all properties in the given class
                IEnumerable<PropertyDescriptor> properties = GetTemplateProperties<T>();

                // creates keys for every property
                foreach (PropertyDescriptor property in properties)
                {
                    // add key name to json
                    json += "\"" + property.Name + "\":";

                    // get value for property
                    object value = data.GetType().GetProperty(property.Name).GetValue(data, null);

                    // check if value is null
                    if (value == null)
                    {
                        json += "\"null\",";
                        continue;
                    }

                    // add to string substring based on data type
                    json += HandleValue(property.PropertyType, value);
                    
                    // add a comma after property
                    json += ",";
                }

                // remove last comma
                json = json.Remove(json.Length - 1) + "}";

                // returns the result
                return json;
            }

            private string HandleValue(Type propertyType, object value)
            {
                string json = "";
                if (value == null)
                {
                    json += "\"null\",";
                    return json;
                }

                if (propertyType == typeof(DateTime))
                {
                    value = ((DateTime)value).ToString(this.dateTimeFormat);//("u");
                    json += "\"" + value + "\"";
                }

                else if (propertyType == typeof(int) || propertyType == typeof(float) ||
                         propertyType == typeof(double))
                    json += value;

                #region UnityEngine Classes
                else if (propertyType == typeof(Color))
                {
                    Color val = (Color)value;

                    json += "{\"r\":" + val.r + ",\"g\":" + val.g + ",\"b\":" + val.b + ",\"a\":" + val.a + "}";
                }

                else if (propertyType == typeof(Rect))
                {
                    Rect val = (Rect)value;

                    json += "{\"x\":" + val.x + ",\"y\":" + val.y + ",\"width\":" + val.width + ",\"height\":" + val.height + "}";
                }

                else if (propertyType == typeof(Vector3))
                {
                    Vector3 val = (Vector3)value;

                    json += "{\"x\":" + val.x + ",\"y\":" + val.y + ",\"z\":" + val.z + "}";
                }

                else if (propertyType == typeof(Vector2))
                {
                    Vector2 val = (Vector2)value;

                    json += "{\"x\":" + val.x + ",\"y\":" + val.y + "}";
                }

                else if (propertyType == typeof(Vector4))
                {
                    Vector4 val = (Vector4)value;

                    json += "{\"x\":" + val.x + ",\"y\":" + val.y + ",\"z\":" + val.z + ",\"w\":" + val.w + "}";
                }

                else if (propertyType == typeof(Quaternion))
                {
                    Quaternion val = (Quaternion)value;

                    json += "{\"x\":" + val.x + ",\"y\":" + val.y + ",\"z\":" + val.z + ",\"w\":" + val.w + "}";
                }
                #endregion

                else if (propertyType == typeof(string))
                    json += "\"" + value + "\"";

                else if (propertyType.IsArray)
                    json += HandleArray(propertyType, value);

                else if (PropertyIsList(propertyType))
                    json += HandleList(propertyType, value);

                else if (PropertyIsDictionary(propertyType))
                    json += HandleDictionary(propertyType, value);

                else if (propertyType.IsClass)
                    json += HandleCustomClass(propertyType, value);

                else
                    json += "\"" + value + "\"";

                return json;
            }

            private string HandleCustomClass(Type propertyType, object value)
            {
                MethodInfo method = typeof(Serializer).GetMethod("SerializeClass");

                MethodInfo genericMethod = method.MakeGenericMethod(new[] { propertyType });

                return (string)genericMethod.Invoke(this, new[] { value });
            }

            private string HandleArray(Type propertyType, object value)
            {
                string json = "";  
                Type type = propertyType.GetElementType();

                if (type == null)
                    return json;

                var array = value as Array;
                json += "[";
                foreach (var val in array)
                {
                    json += HandleValue(type, val);

                    json += ",";
                }

                return json.Remove(json.Length - 1) + "]";
            }

            private string HandleList(Type propertyType, object value)
            {
                string json = "";
                Type type = propertyType.GetGenericArguments()[0];

                IList list = (IList)value;
                json += "[";

                int i = 0;
                foreach (object val in list)
                {
                    if(i++ > 0)
                        json += ",";
                    json += HandleValue(type, val);
                }

                return json + "]";
            }

            private string HandleDictionary(Type propertyType, object value)
            {
                string json = "{";

                var dictionary = value as Dictionary<object, object>;
                if (dictionary != null)
                    foreach (var pair in dictionary)
                    {
                        json += "{\"" + pair.Key + "\"=\"" + pair.Value + "\"}";
                    }

                return json + "}";
            } 

            public void SetDateTimeFormat(string format)
            {
                this.dateTimeFormat = format;
            }
        }
    }
}