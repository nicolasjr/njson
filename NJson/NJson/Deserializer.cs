
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

        private class Deserializer
        {
            public static Deserializer Instance { get { return instance ?? (instance = new Deserializer()); } }
            private static Deserializer instance;

            private Deserializer()
            {
            } 

            public T Deserialize<T>(Dictionary<string, object> dictionary)
            {
                // return null, if there's no dictionary
                if (dictionary == null)
                    return default(T);

                // instantiate class to be deserialized
                T data = (T) Activator.CreateInstance(typeof (T), null);

                // find all properties in the given class
                IEnumerable<PropertyDescriptor> properties = GetTemplateProperties<T>();

                // try to find the keys for every property in the class
                foreach (var property in properties)
                {
                    // if could not be found, jump to the next one
                    if (!dictionary.ContainsKey(property.Name))
                        continue;

                    // value of property, according to JSON
                    var value = dictionary[property.Name];

                    // handle data based on its data type
                    value = HandleData(property.PropertyType, value);
                 
                    // pointer to the property
                    PropertyInfo prop = typeof (T).GetProperty(property.Name);

                    // set the value to the property in the correct instance of the class
                    prop.SetValue(data, value, null);
                }

                // returns the result
                return data;
            }

            private object HandleData(Type propertyType, object value)
            {
                if (propertyType == typeof (DateTime))
                    return DateTime.Parse(value.ToString());

                if (propertyType == typeof (string))
                    return value.ToString();

                if (propertyType == typeof (float))
                    return float.Parse(value.ToString());

                if (propertyType == typeof (double))
                    return double.Parse(value.ToString());

                if (propertyType == typeof (int))
                    return int.Parse(value.ToString());

                if (propertyType.IsEnum)
                    return Enum.Parse(propertyType, value.ToString());

                if (propertyType == typeof (bool))
                    return bool.Parse(value.ToString());

                #region UnityEngine Classes

                if (propertyType == typeof (Color))
                {
                    var dic = value as Dictionary<string, object>;

                    if (dic == null || !dic.ContainsKey("r") || !dic.ContainsKey("g") || !dic.ContainsKey("b") ||
                        !dic.ContainsKey("a"))
                        return null;

                    return new Color(float.Parse(dic["r"].ToString()), float.Parse(dic["g"].ToString()),
                        float.Parse(dic["b"].ToString()), float.Parse(dic["a"].ToString()));
                }

                if (propertyType == typeof (Rect))
                {
                    var dic = value as Dictionary<string, object>;

                    if (dic == null || !dic.ContainsKey("x") || !dic.ContainsKey("y") || !dic.ContainsKey("width") ||
                        !dic.ContainsKey("height"))
                        return null;

                    return new Rect(float.Parse(dic["x"].ToString()), float.Parse(dic["y"].ToString()),
                        float.Parse(dic["width"].ToString()), float.Parse(dic["height"].ToString()));
                }

                if (propertyType == typeof (Vector3))
                {
                    var dic = value as Dictionary<string, object>;

                    if (dic == null || !dic.ContainsKey("x") || !dic.ContainsKey("y") || !dic.ContainsKey("z"))
                        return null;

                    return new Vector3(float.Parse(dic["x"].ToString()), float.Parse(dic["y"].ToString()),
                        float.Parse(dic["z"].ToString()));
                }

                if (propertyType == typeof (Vector2))
                {
                    var dic = value as Dictionary<string, object>;

                    if (dic == null || !dic.ContainsKey("x") || !dic.ContainsKey("y"))
                        return null;

                    return new Vector2(float.Parse(dic["x"].ToString()), float.Parse(dic["y"].ToString()));
                }

                if (propertyType == typeof (Vector4))
                {
                    var dic = value as Dictionary<string, object>;

                    if (dic == null || !dic.ContainsKey("x") || !dic.ContainsKey("y") || !dic.ContainsKey("z") ||
                        !dic.ContainsKey("w"))
                        return null;

                    return new Vector4(float.Parse(dic["x"].ToString()), float.Parse(dic["y"].ToString()),
                        float.Parse(dic["z"].ToString()), float.Parse(dic["w"].ToString()));
                }

                if (propertyType == typeof (Quaternion))
                {
                    var dic = value as Dictionary<string, object>;

                    if (dic == null || !dic.ContainsKey("x") || !dic.ContainsKey("y") || !dic.ContainsKey("z") ||
                        !dic.ContainsKey("w"))
                        return null;

                    return new Quaternion(float.Parse(dic["x"].ToString()), float.Parse(dic["y"].ToString()),
                        float.Parse(dic["z"].ToString()), float.Parse(dic["w"].ToString()));
                }

                #endregion

                if (propertyType.IsArray)
                    return HandleArray(propertyType, value);

                if (PropertyIsList(propertyType))
                    return HandleList(propertyType, value);

                if (propertyType.IsClass)
                    return HandleCustomClass(propertyType, value);

                return value;
            }

            private object HandleCustomClass(Type propertyType, object value)
            {
                MethodInfo method = typeof(Deserializer).GetMethod("Deserialize");

                MethodInfo genericMethod = method.MakeGenericMethod(new[] { propertyType });

                return genericMethod.Invoke(instance, new[] { value });
            }

            private object HandleList(Type propertyType, object value)
            {
                if (!(value is List<object>))
                    return null;

                Type type = propertyType.GetGenericArguments()[0];

                Type listType = typeof(List<>);

                var constructedListType = listType.MakeGenericType(type);

                var listInstance = Activator.CreateInstance(constructedListType);

                IList list = (IList)listInstance;

                foreach (var objValue in value as List<object>)
                    list.Add(HandleData(type, objValue));

                return list;
            }

            private object HandleArray(Type propertyType, object value)
            {
                if (!(value is List<object>))
                    return null;

                Type type = propertyType.GetElementType();

                if (type == null)
                    return null;

                var array = Array.CreateInstance(type, (value as IList).Count);

                int i = 0;
                foreach (var objValue in value as List<object>)
                    array.SetValue(HandleData(type, objValue), i++);

                return array;
            }
        }
    }
}
