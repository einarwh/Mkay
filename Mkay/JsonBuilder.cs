using System;
using System.Collections.Generic;
using System.Globalization;

using Newtonsoft.Json.Linq;

namespace Mkay
{
    public class JsonBuilder : ITreeBuilder<JObject>
    {
        private readonly string _propertyName;
        private readonly HashSet<string> _methods;
        private readonly HashSet<string> _comparisons;
        private readonly HashSet<string> _logicals;

        public JsonBuilder(string propertyName)
        {
            _propertyName = propertyName;
            _methods = new HashSet<string> { "len", "max", "now", "+", "*", "eval", "~", "rev", "cut" };
            _comparisons = new HashSet<string> { "<", ">", "<=", ">=", "==", "!=" };
            _logicals = new HashSet<string> { "&&", "and", "all", "||", "or", "any" };
        }

        public JObject Build<T>(Atom<T> atom)
        {
            var val = atom.Value;
            return GetValue(val) ?? GetPropertyAccess(val as string);
        }

        public JObject Build(OpData<JObject> data)
        {
            var opType = GetOperatorType(data.Operator);
            var result = Get(opType, data.Operator, data.Operands);
            return result;
        }

        private JObject Get(string opType, string val, IEnumerable<JObject> operands = null)
        {
            var result = new JObject(new JProperty("type", opType), new JProperty("value", val));

            if (operands != null)
            {
                result.Add(new JProperty("operands", new JArray(operands)));
            }

            return result;
        }

        private string GetOperatorType(string op)
        {
            if (_methods.Contains(op) || _comparisons.Contains(op) || _logicals.Contains(op))
            {
                return "call";
            }

            throw new ArgumentException("Unknown operation '{0}'.".With(op));
        }

        private JObject GetValue<T>(T val)
        {
            if (val is int)
            {
                var ival = (int)(object)val;
                var iresult = Get("integer", ival.ToString(CultureInfo.InvariantCulture));
                return iresult;
            }

            if (val is double)
            {
                var dval = (double)(object)val;
                var dresult = Get("float", dval.ToString(CultureInfo.InvariantCulture));
                return dresult;
            }

            if (val is string)
            {
                var sval = (string)(object)val;
                
                if (sval == "true" || sval == "false")
                {
                    return Get("boolean", sval);
                }

                if (sval.Length >= 2 && sval.StartsWith("\"") && sval.EndsWith("\""))
                {
                    var s = sval.Substring(1, sval.Length - 2);
                    var sresult = Get("string", s);
                    return sresult;
                }
            }

            return null;
        }

        private JObject GetPropertyAccess(string val)
        {
            string propName = val == "." ? _propertyName : val;
            var result = Get("property", propName);
            return result;
        }
    }
}