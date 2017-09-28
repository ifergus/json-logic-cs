using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonLogicCSharp
{
    public static class JsonLogic
    {

        static class OperationHandler
        {

            private static Dictionary<(int, string), Func<JArray, IEnumerable<dynamic>, JToken, dynamic>> Operations = null;

            static OperationHandler()
            {
                Operations = new Dictionary<(int, string), Func<JArray, IEnumerable<dynamic>, JToken, dynamic>>()
                {

                    [(1, "missing")] = (v, p, d) =>
                    {
                        var v1 = v.Select(x => x);
                        var d1 = d.Select(x => x.Path);
                        return new JArray(v1.Where(s => !d1.Any(t => t.ToString() == s.ToString())));
                    },

                    [(1, "missing_some")] = (v, p, d) => (Operations[(1, "missing")](v, p, d) as JArray).Count > 0,

                    [(1, "and")] = (v, p, d) => !v.Any(x => !Truthy(ApplyInternal(x, d))),

                    [(1, "or")] = (v, p, d) => v.Any(x => Truthy(ApplyInternal(x, d))),

                    [(1, "in")] = (v, p, d) =>
                    {
                        if (v.Where(x => x is JArray).Count() > 0)
                        {
                            return v.Last.Select(x => x).Contains(v.First);
                        }

                        return ((string)v.Last).Contains((string)v.First);
                    },

                    [(1, "if")] = (v, p, d) =>
                    {
                        int i;
                        for (i = 0; i < v.Count - 1; i += 2)
                        {
                            if (Truthy(ApplyInternal(v[i], d)))
                            {
                                return ApplyInternal(v[i + 1], d);
                            }
                        }

                        if (v.Count == i + 1)
                        {
                            return ApplyInternal(v[i], d);
                        }

                        return null;
                    },

                    [(1, "?:")] = (v, p, d) => Operations[(1, "if")](v, p, d),

                    [(1, "var")] = (v, p, d) =>
                    {
                        if (v.First.Type == JTokenType.String)
                        {
                            return d.SelectToken((string)v.First);
                        }
                        else if (v.First.Type == JTokenType.Integer)
                        {
                            var index = (int)v.First;
                            return (d.First.Last as JArray)[index];
                        }

                        return null;
                    },


                    [(1, "merge")] = (v, p, d) => new JArray(v.SelectMany(x => x).Select(y => (y as JValue).Value)),

                    [(1, "cat")] = (v, p, d) => v.Aggregate((i, j) => i + "" + j),

                    [(1, "log")] = (v, p, d) =>
                    {
                        Console.WriteLine(v.First);
                        return v;
                    },

                    [(1, "!")] = (v, p, d) => !Truthy(v.First),

                    [(1, "!!")] = (v, p, d) => Truthy(v.First),

                    [(2, "==")] = (v, p, d) => p.First() == p.Last(),

                    [(2, "===")] = (v, p, d) => p.First() == p.Last() && p.First().GetHashCode() == p.Last().GetHashCode(),

                    [(2, "!=")] = (v, p, d) => p.First() != p.Last(),

                    [(2, ">")] = (v, p, d) => !p.Zip(p.Skip(1), (a, b) => a > b).Contains(false),

                    [(2, ">=")] = (v, p, d) => p.First() >= p.Last(),

                    [(2, "<")] = (v, p, d) => !p.Zip(p.Skip(1), (a, b) => a < b).Contains(false),

                    [(2, "<=")] = (v, p, d) => p.First() <= p.Last(),

                    [(2, "%")] = (v, p, d) => p.First() % p.Last(),

                    [(2, "/")] = (v, p, d) => p.First() / p.Last(),

                    [(2, "-")] = (v, p, d) => p.Count() == 1 ? p.First() * -1 : p.First() - p.Last(),

                    [(2, "*")] = (v, p, d) => p.Aggregate((a, b) => a * b),

                    [(2, "+")] = (v, p, d) => p.Sum(a => a),

                    [(2, "min")] = (v, p, d) => p.Min(),

                    [(2, "max")] = (v, p, d) => p.Max()

                };
            }

            public static dynamic PerformOperation(string opSymbol, int order, JArray values, JToken data) => Operations[(order, opSymbol)](values, values, data);

            public static bool IsOperationCode(string opSymbol) => Operations.ContainsKey((1, opSymbol)) || Operations.ContainsKey((2, opSymbol));

            public static bool IsOperationInvokable(string opSymbol, int order) => Operations.ContainsKey((order, opSymbol));

        };


        public static dynamic Apply(string jsonExpression, string jsonData = null)
        {
            JObject expressionObj = JsonConvert.DeserializeObject<JObject>(jsonExpression);
            JObject dataObj = (jsonData == null) ? null : JsonConvert.DeserializeObject<JObject>(jsonData);

            var jsonResult = ApplyInternal(expressionObj.First, dataObj);

            // convert result to standard .Net type, if possible. 
            var dotNetResult = ConvertToDotNetType(jsonResult);

            return dotNetResult;
        }


        public static bool Truthy(JToken token)
        {
            switch (token)
            {
                case null:

                    return false;

                case JValue val when val.Type == JTokenType.String:

                    return !String.IsNullOrEmpty((string)val);

                case JValue val when val.Type == JTokenType.Integer:

                    return (int)val != 0;

                case JValue val when val.Type == JTokenType.Float:

                    return (double)val != 0.0d;

                case JValue val when val.Type == JTokenType.Boolean:

                    return (bool)val;

                case JArray array:

                    return array.HasValues;

                case JObject obj:

                    return obj.HasValues;

                default:

                    return false;
            }
        }

        private static bool IsLogicalExpression(JToken token) => (token is JProperty prop) && prop != null && OperationHandler.IsOperationCode(prop.Name);



        private static string ExtractOpSymbol(JToken token)
        {
            if (token is JProperty prop)
            {
                return prop.Name;
            }

            throw new InvalidOperationException("Unable to extract Op symbol");
        }

        private static JArray ExtractValues(JToken token)
        {

            if (token == null)
            {
                return new JArray();

            }
            else if (token.First is JArray array)
            {
                return array;
            }

            return new JArray() { token.First };
        }


        private static dynamic ApplyInternal(JToken logic, JToken data)
        {
            if (logic is JArray array)
            {
                return new JArray() { array.Select(x => ApplyInternal(x, data)) };
            }

            if (logic is JObject obj)
            {
                logic = obj.Properties().First();
            }

            if (!IsLogicalExpression(logic))
            {
                return logic;
            }

            if (!Truthy(data))
            {
                data = new JArray();
            }

            var opSymbol = ExtractOpSymbol(logic);
            var values = ExtractValues(logic);


            if (OperationHandler.IsOperationInvokable(opSymbol, 1))
            {
                return OperationHandler.PerformOperation(opSymbol, 1, values, data);
            }

            values = new JArray() { values.Select(x => ApplyInternal(x, data)).ToArray() };

            if (OperationHandler.IsOperationInvokable(opSymbol, 2))
            {
                return OperationHandler.PerformOperation(opSymbol, 2, values, data);
            }

            throw new KeyNotFoundException("Operator not defined: " + opSymbol);
        }


        public static dynamic ConvertToDotNetType(dynamic token)
        {

            switch (token)
            {
                case null:

                    return null;

                case JValue val when val.Type == JTokenType.String:

                    return (string)val;

                case JValue val when val.Type == JTokenType.Integer:

                    return (int)val;

                case JValue val when val.Type == JTokenType.Float:

                    return (double)val;

                case JValue val when val.Type == JTokenType.Boolean:

                    return (bool)val;

                case JArray array:

                    try
                    {
                        if (array.Count > 0)
                        {
                            var arrayType = array[0].Type;

                            switch (arrayType)
                            {
                                case JTokenType.Integer:
                                    return array.Select(x => (int)x).ToArray(); ;
                                case JTokenType.Float:
                                    return array.Select(x => (double)x).ToArray(); ;
                                case JTokenType.String:
                                    return array.Select(x => (string)x).ToArray(); ;
                                case JTokenType.Boolean:
                                    return array.Select(x => (bool)x).ToArray();
                                default:
                                    return array;
                            }
                        }
                    }
                    catch
                    {
                        // do nothing;
                    }

                    return array;

                case JObject obj:

                    return obj;

                default:

                    return token;
            }
        }
    }
}
