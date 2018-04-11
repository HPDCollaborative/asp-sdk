//
// Copyright (C) 2018 Toxnot PBC
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN // ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.toxnot
{
    //
    // See http://json.org/
    //

    //=========================================================================================

    public interface IJsonSerializable
    {
        string SerializeAsJsonFragment();
        void DeserializeFromJsonFragment(string fragment);
    }

    //=========================================================================================

    public class JsonObject : IJsonSerializable, IEnumerable, IEnumerable<JsonObject>
    {
        //=========================================================================================

        public JsonObject()
        {
            m_impl = null;
        }

        //=========================================================================================

        public JsonObject(object o)
        {
            m_impl = JsonUtil.CreateJsonCompatibleObject(o);
        }

        //=========================================================================================

        private static JsonObject WithImpl(object o)
        {
            JsonObject j = new JsonObject();

            j.m_impl = o;

            return j;
        }

        //=========================================================================================

        public Dictionary<string, object> GetImplDictionary()
        {
            if (!(m_impl is Dictionary<string, object>))
            {
                throw new InvalidOperationException();
            }

            return (Dictionary<string, object>)m_impl;
        }

        //=========================================================================================

        public static JsonObject ParseJsonFragment(string jsonFragmentText)
        {
            JsonObject j = new JsonObject();

            j.m_impl = JsonUtil.ParseAsJsonFragment(jsonFragmentText);

            return j;
        }

        //=========================================================================================

        public string ToJsonFragment()
        {
            return JsonUtil.ToJsonFragment(m_impl);
        }

        //=========================================================================================

        public string ToMultilineJsonFragment()
        {
            return JsonUtil.ToMultilineJsonFragment(m_impl);
        }

        //=========================================================================================

        public bool IsNull
        {
            get
            {
                return m_impl == null;
            }
        }

        //=========================================================================================

        public bool ContainsKey(string key)
        {
            if (!(m_impl is Dictionary<string, object>))
            {
                throw new InvalidOperationException();
            }

            return ((Dictionary<string, object>)m_impl).ContainsKey(key);
        }

        //=========================================================================================

        public IEnumerable<string> Keys
        {
            get
            {
                if (!(m_impl is Dictionary<string, object>))
                {
                    throw new InvalidOperationException();
                }

                return ((Dictionary<string, object>)m_impl).Keys;
            }
        }

        //=========================================================================================

        public int Count
        {
            get
            {
                if (m_impl is List<object>)
                {
                    return ((List<object>)m_impl).Count;
                }
                else if (m_impl is Dictionary<string, object>)
                {
                    return ((Dictionary<string, object>)m_impl).Count;
                }
                else
                {
                    return 1;
                }
            }
        }

        //=========================================================================================

        public JsonObject this[int index]
        {
            get
            {
                if (m_impl is List<object>)
                {
                    return WithImpl(((List<object>)m_impl)[index]);
                }
                else if (m_impl is Dictionary<string, object>)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    if ((index < 0) || (index > 0))
                    {
                        throw new ArgumentOutOfRangeException();
                    }

                    return WithImpl(m_impl);
                }
            }
        }

        //=========================================================================================

        public JsonObject this[string key]
        {
            get
            {
                if (!(m_impl is Dictionary<string, object>))
                {
                    throw new InvalidOperationException();
                }

                object ret;

                bool keyExists = ((Dictionary<string, object>)m_impl).TryGetValue(key, out ret);

                return keyExists ? WithImpl(ret) : null;
            }
        }

        //=========================================================================================

        private class JsonObjectEnumerator : IEnumerator<JsonObject>
        {
            public JsonObjectEnumerator(JsonObject jo)
            {
                m_jo = jo;
                m_i = -1;
                m_n = jo.Count;
            }

            public bool MoveNext()
            {
                ++m_i;

                return m_i < m_n;
            }

            public void Reset()
            {
                m_i = -1;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public JsonObject Current
            {
                get { return m_jo[m_i]; }
            }

            void IDisposable.Dispose() { }

            private JsonObject m_jo;
            private int m_i;
            private int m_n;
        }

        //=========================================================================================

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new JsonObjectEnumerator(this);
        }

        //=========================================================================================

        IEnumerator<JsonObject> IEnumerable<JsonObject>.GetEnumerator()
        {
            return new JsonObjectEnumerator(this);
        }

        //=========================================================================================

        public List<T> ToList<T>(Func<string, T> valueParser)
        {
            if (!(m_impl is List<object>))
            {
                throw new InvalidOperationException();
            }

            List<T> ret = new List<T>();

            foreach (object oi in ((List<object>)m_impl))
            {
                ret.Add(valueParser(oi.ToString()));
            }

            return ret;
        }

        //=========================================================================================

        public Tuple<string, string> To2Tuple()
        {
            if ( (!(m_impl is List<object>)) || (((List<object>)m_impl).Count != 2) )
            {
                throw new InvalidOperationException();
            }

            List<object> a = (List<object>)m_impl;
            
            return Tuple.Create(
                a[0] == null ? (string)null : a[0].ToString(),
                a[1] == null ? (string)null : a[1].ToString());
        }

        //=========================================================================================

        public Tuple<string, string, string> To3Tuple()
        {
            if ((!(m_impl is List<object>)) || (((List<object>)m_impl).Count != 3))
            {
                throw new InvalidOperationException();
            }

            List<object> a = (List<object>)m_impl;

            return Tuple.Create(
                a[0] == null ? (string)null : a[0].ToString(),
                a[1] == null ? (string)null : a[1].ToString(),
                a[2] == null ? (string)null : a[2].ToString());
        }

        //=========================================================================================

        public Tuple<string, string, string, string> To4Tuple()
        {
            if ((!(m_impl is List<object>)) || (((List<object>)m_impl).Count != 4))
            {
                throw new InvalidOperationException();
            }

            List<object> a = (List<object>)m_impl;

            return Tuple.Create(
                a[0] == null ? (string)null : a[0].ToString(),
                a[1] == null ? (string)null : a[1].ToString(),
                a[2] == null ? (string)null : a[2].ToString(),
                a[3] == null ? (string)null : a[3].ToString());
        }

        //=========================================================================================

        public Dictionary<string, TValue> ToDictionary<TValue>(Func<string, TValue> valueParser)
        {
            if (!(m_impl is Dictionary<string, object>))
            {
                throw new InvalidOperationException("The underlying object is not a dictionary");
            }

            return ((Dictionary<string, object>)m_impl).ToDictionary(kvp => kvp.Key, kvp => valueParser(kvp.Value.ToString()));
        }

        //=========================================================================================

        public override string ToString()
        {
            return m_impl != null ? m_impl.ToString() : null;
        }

        //=========================================================================================

        public bool ToBool()
        {
            if (m_impl == null)
            {
                throw new NullReferenceException();
            }

            return m_impl.ToString().ToLower() == "true";
        }

        //=========================================================================================

        public int ToInt()
        {
            if (m_impl == null)
            {
                throw new NullReferenceException();
            }

            return Int32.Parse(m_impl.ToString());
        }

        //=========================================================================================

        public int ToIntOrDefault(int suppliedDefault)
        {
            if (m_impl == null)
            {
                return suppliedDefault;
            }

            int result;

            bool success = Int32.TryParse(m_impl.ToString(), out result);

            return success ? result : suppliedDefault;
        }

        //=========================================================================================

        public double ToDouble()
        {
            if (m_impl == null)
            {
                throw new NullReferenceException();
            }

            if (m_impl is double)
            {
                return (double)m_impl;
            }
            else
            {
                return Double.Parse(m_impl.ToString());
            }
        }

        //=========================================================================================

        public double ToDoubleOrDefault(double suppliedDefault)
        {
            if (m_impl == null)
            {
                return suppliedDefault;
            }

            if (m_impl is double)
            {
                return (double)m_impl;
            }
            else
            {
                double result;

                bool success = Double.TryParse(m_impl.ToString(), out result);

                return success ? result : suppliedDefault;
            }
        }

        //=========================================================================================

        public string SerializeAsJsonFragment()
        {
            return this.ToJsonFragment();
        }

        //=========================================================================================

        public void DeserializeFromJsonFragment(string fragment)
        {
            m_impl = JsonUtil.ParseAsJsonFragment(fragment);
        }

        //=========================================================================================

        private object m_impl;
    }

    //=========================================================================================

    public static class JsonUtil
    {
        //=========================================================================================

        private const int MaxNestingDepth = 1024;

        //=========================================================================================

        public static object ParseAsJsonFragment(string s)
        {
            int i = 0;

            object r = ParseValue(s, ref i, 0);

            if (i != s.Length)
            {
                throw new ArgumentException("invalid string");
            }

            return r;
        }

        //=========================================================================================

        private static string Spacer(int size)
        {
            return "".PadRight(size);
        }

        //=========================================================================================

        private static string ToJsonFragment(object o, bool multiline, int depth)
        {
            if (depth >= MaxNestingDepth)
            {
                throw new ArgumentException("object exceeds nesting depth threshold");
            }

            int indentSize = multiline ? 4 : 0;
            int indent = depth * indentSize;

            if (o == null)
            {
                return "null";
            }
            else if (o is IDictionary)
            {
                List<string> itemFragments = new List<string>();

                foreach (string s in ((IDictionary)o).Keys)
                {
                    itemFragments.Add(
                        Spacer(indent + indentSize) +
                        String.Format(
                            "\"{0}\" : {1}",
                            JsonStringEncode((string)s),
                            ToJsonFragment(((IDictionary)o)[s], multiline, depth + 1)));
                }

                return
                    "{" +
                    (multiline ? "\r\n" : " ") +
                    String.Join(
                        String.Format(
                            ",{0}",
                            (multiline ? "\r\n" : " ")),
                            itemFragments) +
                    (multiline ? "\r\n" : " ") +
                    Spacer(indent) + "}";
            }
            else if (o is Enum)
            {
                return String.Format("\"{0}\"", JsonStringEncode(o.ToString()));
            }
            else if (o is string)
            {
                return String.Format("\"{0}\"", JsonStringEncode((string)o));
            }
            else if (o is bool)
            {
                return ((bool)o) ? "true" : "false";
            }
            else if ((o is double) || (o is byte) || (o is int) || (o is uint) || (o is long) || (o is ulong))
            {
                return o.ToString();
            }
            else if (o is Guid)
            {
                return String.Format("\"{0}\"", JsonStringEncode(o.ToString()));
            }
            else if (o is IJsonSerializable)
            {
                return ((IJsonSerializable)o).SerializeAsJsonFragment();
            }
            else if ((o is List<object>) || (o is IList) || (o is Array) || (o is IEnumerable))
            {
                List<string> itemFragments = new List<string>();
                foreach (object oi in ((IEnumerable)o))
                {
                    itemFragments.Add(Spacer(indent + indentSize) + ToJsonFragment(oi, multiline, depth + 1));
                }

                return
                    "[" +
                    (multiline ? "\r\n" : " ") +
                    String.Join(
                        String.Format(
                            ",{0}",
                            (multiline ? "\r\n" : " ")),
                        itemFragments) +
                    (multiline ? "\r\n" : " ") +
                    Spacer(indent) + "]";
            }
            else
            {
                throw new ArgumentException(String.Format("Type {0} is not supported for conversion to json fragment.", o.GetType().ToString()));
            }
        }

        //=========================================================================================

        public static string ToJsonListFragment(params object[] objects)
        {
            return ToJsonFragment(objects);
        }

        //=========================================================================================

        public static string ToJasonTupleFragment<T1, T2>(Tuple<T1, T2> tuple)
        {
            return ToJsonListFragment(tuple.Item1, tuple.Item2);
        }

        //=========================================================================================

        public static string ToJasonTupleFragment<T1, T2, T3>(Tuple<T1, T2, T3> tuple)
        {
            return ToJsonListFragment(tuple.Item1, tuple.Item2, tuple.Item3);
        }

        //=========================================================================================

        public static string ToJasonTupleFragment<T1, T2, T3, T4>(Tuple<T1, T2, T3, T4> tuple)
        {
            return ToJsonListFragment(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

        //=========================================================================================

        public static string ToJsonFragment(object o)
        {
            return ToJsonFragment(o, false, 0);
        }

        //=========================================================================================

        public static string ToMultilineJsonFragment(object o)
        {
            return ToJsonFragment(o, true, 0);
        }

        //=========================================================================================

        private static object CreateJsonCompatibleObject(object o, int depth)
        {
            if (depth >= MaxNestingDepth)
            {
                throw new ArgumentException("object exceeds nesting depth threshold");
            }

            if (o == null)
            {
                return o;
            }
            else if (o is IDictionary)
            {
                var dret = new Dictionary<string, object>();

                foreach (object key in ((IDictionary)o).Keys)
                {
                    dret[key.ToString()] = CreateJsonCompatibleObject(((IDictionary)o)[key], depth + 1);
                }

                return dret;
            }
            else if (o is String)
            {
                return o;
            }
            else if (o is bool)
            {
                return o;
            }
            else if ((o is double) || (o is byte) || (o is int) || (o is uint) || (o is long) || (o is ulong) || (o is Enum))
            {
                return o;
            }
            else if (o is IEnumerable)
            {
                var lret = new List<object>();

                foreach (object oi in (IEnumerable)o)
                {
                    lret.Add(CreateJsonCompatibleObject(oi, depth + 1));
                }

                return lret;
            }
            else if (o is IJsonSerializable)
            {
                return o;
            }
            else
            {
                return o.ToString();
            }
        }

        //=========================================================================================

        public static object CreateJsonCompatibleObject(object o)
        {
            return CreateJsonCompatibleObject(o, 0);
        }

        //=========================================================================================

        private static string JsonStringEncode(string s)
        {
            // TODO: stringbuilder to avoid object allocs

            s = s.Replace("\\", "\\\\");
            s = s.Replace("\"", "\\\"");
            s = s.Replace("/", "\\/");
            s = s.Replace("\b", "\\b");
            s = s.Replace("\f", "\\f");
            s = s.Replace("\n", "\\n");
            s = s.Replace("\r", "\\r");
            s = s.Replace("\t", "\\t");

            return s;
        }

        //=========================================================================================

        private static char FourHexDigitsToUnicodeChar(char c1, char c2, char c3, char c4)
        {
            // f(c) converts the hex digit c into the int value of the hex digit (int.MaxValue returned on c invalid)
            Func<char, int> f =
                c =>
                    (('0' <= c) && (c <= '9'))
                        ? (c - '0')
                        : ((('A' <= c) && (c <= 'F'))
                            ? (c - 'A')
                            : ((('a' <= c) && (c <= 'f'))
                                ? (c - 'a')
                                : int.MaxValue));

            int d1 = f(c1);
            int d2 = f(c2);
            int d3 = f(c3);
            int d4 = f(c4);

            if ((d1 == int.MaxValue) || (d2 == int.MaxValue) || (d3 == int.MaxValue) || (d4 == int.MaxValue))
            {
                throw new ArgumentException("invalid hex digits");
            }

            UInt16 r = (UInt16)(d4 + (16 * d3) + (16 * 16 * d2) + (16 * 16 * 16 * d1));

            return (char)r;
        }

        //=========================================================================================

        private static void SkipWhitespace(string s, ref int i)
        {
            while ((i < s.Length) && Char.IsWhiteSpace(s[i]))
            {
                ++i;
            }
        }

        //=========================================================================================

        private static object ParseAtomicObject(string s, ref int i, string keyword, int depth)
        {
            if (depth >= MaxNestingDepth)
            {
                throw new ArgumentException("json string exceeds nesting depth threshold");
            }

            if (((s.Length - i) >= keyword.Length) && (s.Substring(i, keyword.Length) == keyword))
            {
                i += keyword.Length;

                switch (keyword)
                {
                    case "true": return true;
                    case "false": return false;
                    case "null": return null;
                }
            }
            
            throw new ArgumentException("invalid string");
        }

        //=========================================================================================

        private static object ParseNumber(string s, ref int i, int depth)
        {
            if (depth >= MaxNestingDepth)
            {
                throw new ArgumentException("json string exceeds nesting depth threshold");
            }

            if ((i >= s.Length) || ((!Char.IsDigit(s[i]) && (s[i] != '-'))))
            {
                throw new ArgumentException("invalid string");
            }

            int startIndex = i;

            if ( (i < s.Length) && (s[i] == '-') )
            {
                ++i;
            }

            if ( (i < s.Length) && (s[i] == '0') )
            {
                ++i;
            }
            else if ( (i < s.Length) && (Char.IsDigit(s[i])))
            {
                ++i;

                while ((i < s.Length) && (Char.IsDigit(s[i])))
                {
                    ++i;
                }
            }
            else
            {
                throw new ArgumentException("invalid string");
            }

            if ( (i < s.Length) && (s[i] == '.') )
            {
                ++i;

                if ((i >= s.Length) || (!Char.IsDigit(s[i])))
                {
                    throw new ArgumentException("invalid string");
                }

                while ((i < s.Length) && (Char.IsDigit(s[i])))
                {
                    ++i;
                }
            }

            if ( (i < s.Length) && ((s[i] == 'e') || (s[i] == 'E')) )
            {
                ++i;

                if ( (i < s.Length) && ((s[i] == '+') || (s[i] == '-')) )
                {
                    ++i;
                }

                if ( (i >= s.Length) || (!Char.IsDigit(s[i])) )
                {
                    throw new ArgumentException("invalid string");
                }

                while ((i < s.Length) && (Char.IsDigit(s[i])))
                {
                    ++i;
                }
            }

            System.Diagnostics.Debug.Assert(i <= s.Length);

            double d;
            bool validDouble = Double.TryParse(s.Substring(startIndex, i - startIndex), out d);

            if (!validDouble)
            {
                throw new ArgumentException("invalid string");
            }

            return d;
        }

        //=========================================================================================

        private static object ParseString(string s, ref int i, int depth)
        {
            if (depth >= MaxNestingDepth)
            {
                throw new ArgumentException("json string exceeds nesting depth threshold");
            }

            if ( (i >= s.Length) || (s[i] != '\"'))
            {
                throw new ArgumentException("invalid string");
            }

            ++i;

            StringBuilder value = new StringBuilder();

            while ((i < s.Length) && (s[i] != '\"'))
            {
                if (s[i] == '\\')
                {
                    if ((i + 1) >= s.Length)
                    {
                        throw new ArgumentException("invalid string");
                    }

                    ++i;

                    char decodedChar;

                    switch (s[i])
                    {
                        case '\"': decodedChar = '\"'; break;
                        case '\\': decodedChar = '\\'; break;
                        case '/': decodedChar = '/'; break;
                        case 'b': decodedChar = '\b'; break;
                        case 'f': decodedChar = '\f'; break;
                        case 'n': decodedChar = '\n'; break;
                        case 'r': decodedChar = '\r'; break;
                        case 't': decodedChar = '\t'; break;
                        case 'u':
                            if ((i + 4) >= s.Length)
                            {
                                throw new ArgumentException("invalid string");
                            }

                            decodedChar = FourHexDigitsToUnicodeChar(s[i+1], s[i+2], s[i+3], s[i+4]);
                            i += 4;
                            break;
                        default: throw new ArgumentException("invalid string");
                    }

                    value.Append(decodedChar);
                }
                else
                {
                    value.Append(s[i]);
                }

                ++i;
            }

            if ((i >= s.Length) || (s[i] != '\"'))
            {
                throw new ArgumentException("invalid string");
            }

            ++i;

            return value.ToString();
        }

        //=========================================================================================

        private static object ParseList(string s, ref int i, int depth)
        {
            if (depth >= MaxNestingDepth)
            {
                throw new ArgumentException("json string exceeds nesting depth threshold");
            }

            if ((i >= s.Length) || (s[i] != '['))
            {
                throw new ArgumentException("invalid string");
            }

            ++i;

            SkipWhitespace(s, ref i);

            List<object> node = new List<object>();

            while ((i < s.Length) && (s[i] != ']'))
            {
                object subNode = ParseValue(s, ref i, depth + 1);
                node.Add(subNode);

                SkipWhitespace(s, ref i);

                if ((i < s.Length) && (s[i] == ','))
                {
                    ++i;

                    SkipWhitespace(s, ref i);
                        
                    if ((i < s.Length) && (s[i] == ']'))
                    {
                        throw new ArgumentException("invalid string");
                    }
                }
            }

            if ((i >= s.Length) || (s[i] != ']'))
            {
                throw new ArgumentException("invalid string");
            }

            ++i;

            return node;
        }

        //=========================================================================================

        private static object ParseDictionary(string s, ref int i, int depth)
        {
            if (depth >= MaxNestingDepth)
            {
                throw new ArgumentException("json string exceeds nesting depth threshold");
            }

            if ((i >= s.Length) || (s[i] != '{'))
            {
                throw new ArgumentException("invalid string");
            }

            ++i;

            Dictionary<string, object> node = new Dictionary<string, object>();

            while ((i < s.Length) && (s[i] != '}'))
            {
                SkipWhitespace(s, ref i);

                if ((i < s.Length) && (s[i] != '}'))
                {
                    string keyString = (string) ParseString(s, ref i, depth + 1);

                    SkipWhitespace(s, ref i);

                    if ((i >= s.Length) || (s[i] != ':'))
                    {
                        throw new ArgumentException("invalid string");
                    }

                    ++i;

                    SkipWhitespace(s, ref i);

                    object valueNode = ParseValue(s, ref i, depth + 1);

                    node[keyString] = valueNode;

                    SkipWhitespace(s, ref i);

                    if ((i < s.Length) && (s[i] == ','))
                    {
                        ++i;

                        if ((i < s.Length) && (s[i] == '}'))
                        {
                            throw new ArgumentException("invalid string");
                        }
                    }
                }
            }

            if ((i >= s.Length) || (s[i] != '}'))
            {
                throw new ArgumentException("invalid string");
            }

            ++i;

            return node;
        }

        //=========================================================================================

        private static object ParseValue(string s, ref int i, int depth)
        {
            if (depth >= MaxNestingDepth)
            {
                throw new ArgumentException("json string exceeds nesting depth threshold");
            }

            SkipWhitespace(s, ref i);

            if (i >= s.Length)
            {
                throw new ArgumentException("invalid string");
            }

            object ret;

            switch (s[i])
            {
                case '\"': ret = ParseString(s, ref i, depth + 1); break;
                case '[':  ret = ParseList(s, ref i, depth + 1); break;
                case '{':  ret = ParseDictionary(s, ref i, depth + 1); break;
                case 't':  ret = ParseAtomicObject(s, ref i, "true", depth + 1); break;
                case 'f':  ret = ParseAtomicObject(s, ref i, "false", depth + 1); break;
                case 'n':  ret = ParseAtomicObject(s, ref i, "null", depth + 1); break;
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':  ret = ParseNumber(s, ref i, depth + 1); break;
                default:   throw new ArgumentException("invalid string");
            }

            SkipWhitespace(s, ref i);

            return ret;
        }
    }
}
