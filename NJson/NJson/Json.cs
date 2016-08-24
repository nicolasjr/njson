using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NPlugins.Json
{
    /// <summary>
    ///     This Library encodes and decodes JSON strings from and to any given data structure.<br></br>
    ///     It's has support to only few UnityEngine class, but all native Data Types.<br></br>
    ///     It's also important to note that the properties of custom class are required to be Public.
    /// </summary>
    /// <remarks>
    ///     <br>Supported Data Types:</br>
    ///         <pre>   * Integer</pre>
    ///         <pre>   * Float</pre>
    ///         <pre>   * Double</pre>
    ///         <pre>   * String</pre>
    ///         <pre>   * Boolean</pre>
    ///         <pre>   * DateTime</pre>
    ///         <pre>   * List</pre>
    ///         <pre>   * Array</pre>
    ///         <pre>   * Array of Arrays</pre>
    ///         <pre>   * List of Lists</pre>
    ///         <pre>   * List of Arrays</pre>
    ///         <pre>   * Arrays of List</pre>
    ///         <pre>   * Vector2</pre>
    ///         <pre>   * Vector3</pre>
    ///         <pre>   * Vector4</pre>
    ///         <pre>   * Quaternion</pre>
    ///         <pre>   * Rect</pre>
    ///         <pre>   * Color (rgba)</pre>
    ///         <pre>   * Any custom class with the above data types.</pre>
    ///
    ///     <br>Unsupported Data Types: (support will be added)</br>
    ///         <pre>   * Dictionaries</pre>
    ///         <pre>   * Other UnityEngine Classes (Transform, GameObject, Material, etc.)</pre>
    /// </remarks>
    public static partial class Json
    {
        /// <summary>
        ///     Create an instance of any given class with the values received in a JSON string
        ///     To work properly, the provided template class has to have the same structure as
        ///     the JSON string, with properties having same names as JSON keys.
        /// </summary>
        /// <typeparam name="T"><br>Class to receive JSON parsed values</br></typeparam>
        /// <param name="json"><br>A JSON string to be deserialized</br></param>
        /// <returns>An instance of template class with values set according to JSON string</returns>
        public static T Deserialize<T>(string json)
        {
            var dictionary = MiniJson.Deserialize(json) as Dictionary<string, object>;

            T data = Deserializer.Instance.Deserialize<T>(dictionary);

            return data;
        }

        /// <summary>
        ///     Creates a JSON string based on any given class instance's properties and values.
        /// </summary>
        /// <typeparam name="T"><br>Type to be transformed in JSON string</br></typeparam>
        /// <param name="data"><br>Instance of class to be transformed in a JSON string</br></param>
        /// <returns>JSON string</returns>
        public static string Serialize<T>(T data)
        {
            return Serializer.Instance.SerializeClass(data);
        }

        /// <summary>
        ///     Allows you to change the format of a DateTime property to be serialized to JSON.
        ///     Visit https://msdn.microsoft.com/en-us/library/zdtaw1bw(v=vs.110).aspx for references of formats.
        /// </summary>
        /// <param name="format"><br>Format of the expected Date Time string output.</br></param>
        public static void SetDateTimeSerializerFormat(string format)
        {
            Serializer.Instance.SetDateTimeFormat(format);
        }

        private static IEnumerable<PropertyDescriptor> GetTemplateProperties<T>()
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));

            return properties.Cast<PropertyDescriptor>().ToList();
        }

        private static bool PropertyIsList(Type property)
        {
            var instance = Activator.CreateInstance(property, null);

            return instance is IList;
        }

        private static bool PropertyIsDictionary(Type property)
        {
            var instance = Activator.CreateInstance(property, null);

            return instance is IDictionary;
        }
    }
}