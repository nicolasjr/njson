/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace NPlugins.Json {
    // Example usage:
    //
    //  using UnityEngine;
    //  using System.Collections;
    //  using System.Collections.Generic;
    //  using MiniJSON;
    //
    //  public class MiniJSONTest : MonoBehaviour {
    //      void Start () {
    //          var jsonString = "{ \"array\": [1.44,2,3], " +
    //                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
    //                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
    //                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
    //                          "\"int\": 65536, " +
    //                          "\"float\": 3.1415926, " +
    //                          "\"bool\": true, " +
    //                          "\"null\": null }";
    //
    //          var dict = MiniJson.Deserialize(jsonString) as Dictionary<string,object>;
    //
    //          Debug.Log("deserialized: " + dict.GetType());
    //          Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
    //          Debug.Log("dict['string']: " + (string) dict["string"]);
    //          Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
    //          Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
    //          Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
    //
    //          var str = MiniJson.Serialize(dict);
    //
    //          Debug.Log("serialized: " + str);
    //      }
    //  }

    public partial class Json
    {
        /// <summary>
        /// This class encodes and decodes JSON strings.
        /// Spec. details, see http://www.json.org/
        ///
        /// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
        /// All numbers are parsed to doubles.
        /// </summary>
        private static class MiniJson
        {
            /// <summary>
            /// Parses the string json into a value
            /// </summary>
            /// <param name="json">A JSON string.</param>
            /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>
            public static object Deserialize(string json)
            {
                // save the string for debug information
                if (json == null)
                {
                    return null;
                }

                return Parser.Parse(json);
            }

            private sealed class Parser : IDisposable
            {
                private const string WordBreak = "{}[],:\"";

                private static bool IsWordBreak(char c)
                {
                    return Char.IsWhiteSpace(c) || WordBreak.IndexOf(c) != -1;
                }

                private enum Token
                {
                    None,
                    CurlyOpen,
                    CurlyClose,
                    SquaredOpen,
                    SquaredClose,
                    Colon,
                    Comma,
                    String,
                    Number,
                    True,
                    False,
                    Null
                };

                private StringReader json;

                private Parser(string jsonString)
                {
                    json = new StringReader(jsonString);
                }

                public static object Parse(string jsonString)
                {
                    using (var instance = new Parser(jsonString))
                    {
                        return instance.ParseValue();
                    }
                }

                public void Dispose()
                {
                    json.Dispose();
                    json = null;
                }

                private Dictionary<string, object> ParseObject()
                {
                    Dictionary<string, object> table = new Dictionary<string, object>();

                    // ditch opening brace
                    json.Read();

                    // {
                    while (true)
                    {
                        switch (NextToken)
                        {
                            case Token.None:
                                return null;
                            case Token.Comma:
                                continue;
                            case Token.CurlyClose:
                                return table;
                            default:
                                // name
                                string name = ParseString();
                                if (name == null)
                                {
                                    return null;
                                }

                                // :
                                if (NextToken != Token.Colon)
                                {
                                    return null;
                                }
                                // ditch the colon
                                json.Read();

                                // value
                                table[name] = ParseValue();
                                break;
                        }
                    }
                }

                private List<object> ParseArray()
                {
                    List<object> array = new List<object>();

                    // ditch opening bracket
                    json.Read();

                    // [
                    var parsing = true;
                    while (parsing)
                    {
                        Token nextToken = NextToken;

                        switch (nextToken)
                        {
                            case Token.None:
                                return null;
                            case Token.Comma:
                                continue;
                            case Token.SquaredClose:
                                parsing = false;
                                break;
                            default:
                                object value = ParseByToken(nextToken);

                                array.Add(value);
                                break;
                        }
                    }

                    return array;
                }

                private object ParseValue()
                {
                    Token nextToken = NextToken;
                    return ParseByToken(nextToken);
                }

                private object ParseByToken(Token token)
                {
                    switch (token)
                    {
                        case Token.String:
                            return ParseString();
                        case Token.Number:
                            return ParseNumber();
                        case Token.CurlyOpen:
                            return ParseObject();
                        case Token.SquaredOpen:
                            return ParseArray();
                        case Token.True:
                            return true;
                        case Token.False:
                            return false;
                        case Token.Null:
                            return null;
                        default:
                            return null;
                    }
                }

                private string ParseString()
                {
                    StringBuilder s = new StringBuilder();

                    // ditch opening quote
                    json.Read();

                    bool parsing = true;
                    while (parsing)
                    {

                        if (json.Peek() == -1)
                            break;

                        char c = NextChar;
                        switch (c)
                        {
                            case '"':
                                parsing = false;
                                break;
                            case '\\':
                                if (json.Peek() == -1)
                                {
                                    parsing = false;
                                    break;
                                }

                                c = NextChar;
                                switch (c)
                                {
                                    case '"':
                                    case '\\':
                                    case '/':
                                        s.Append(c);
                                        break;
                                    case 'b':
                                        s.Append('\b');
                                        break;
                                    case 'f':
                                        s.Append('\f');
                                        break;
                                    case 'n':
                                        s.Append('\n');
                                        break;
                                    case 'r':
                                        s.Append('\r');
                                        break;
                                    case 't':
                                        s.Append('\t');
                                        break;
                                    case 'u':
                                        var hex = new char[4];

                                        for (int i = 0; i < 4; i++)
                                        {
                                            hex[i] = NextChar;
                                        }

                                        s.Append((char) Convert.ToInt32(new string(hex), 16));
                                        break;
                                }
                                break;
                            default:
                                s.Append(c);
                                break;
                        }
                    }

                    return s.ToString();
                }

                private object ParseNumber()
                {
                    string number = NextWord;

                    if (number.IndexOf('.') == -1 && number.IndexOf(',') == -1)
                    {
                        long parsedInt;
                        Int64.TryParse(number, out parsedInt);
                        return parsedInt;
                    }

                    double parsedDouble;
                    Double.TryParse(number, out parsedDouble);
                    return float.Parse(number, CultureInfo.InvariantCulture);//parsedDouble;
                }

                private void EatWhitespace()
                {
                    while (Char.IsWhiteSpace(PeekChar))
                    {
                        json.Read();

                        if (json.Peek() == -1)
                        {
                            break;
                        }
                    }
                }

                private char PeekChar
                {
                    get { return Convert.ToChar(json.Peek()); }
                }

                private char NextChar
                {
                    get { return Convert.ToChar(json.Read()); }
                }

                private string NextWord
                {
                    get
                    {
                        StringBuilder word = new StringBuilder();

                        while (!IsWordBreak(PeekChar))
                        {
                            word.Append(NextChar);

                            if (json.Peek() == -1)
                            {
                                break;
                            }
                        }

                        return word.ToString();
                    }
                }

                private Token NextToken
                {
                    get
                    {
                        EatWhitespace();

                        if (json.Peek() == -1)
                        {
                            return Token.None;
                        }

                        switch (PeekChar)
                        {
                            case '{':
                                return Token.CurlyOpen;
                            case '}':
                                json.Read();
                                return Token.CurlyClose;
                            case '[':
                                return Token.SquaredOpen;
                            case ']':
                                json.Read();
                                return Token.SquaredClose;
                            case ',':
                                json.Read();
                                return Token.Comma;
                            case '"':
                                return Token.String;
                            case ':':
                                return Token.Colon;
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                            case '-':
                                return Token.Number;
                        }

                        switch (NextWord)
                        {
                            case "false":
                                return Token.False;
                            case "true":
                                return Token.True;
                            case "null":
                                return Token.Null;
                        }

                        return Token.None;
                    }
                }
            }
        }
    }
}
