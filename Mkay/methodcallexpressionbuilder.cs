using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mkay
{
    public class MethodCallExpressionBuilder
    {
        private readonly Dictionary<string, IMethodCallFactory> _factories = new Dictionary<string, IMethodCallFactory>();

        private readonly object _subject;

        private readonly string _propertyName;

        private readonly Unifier _unifier;

        private readonly HashSet<string> _seen;

        public MethodCallExpressionBuilder(object subject, string propertyName, Unifier unifier, HashSet<string> seen)
        {   
            _subject = subject;
            _propertyName = propertyName;
            _unifier = unifier;
            _seen = seen;

            _factories["len"] =
                new SimpleMethodCallFactory(
                    () => typeof(MethodLibrary).GetMethods().First(m => m.Name == "Length"),
                    (s, p, d, m) => CreateStaticMethodCall(d, m));
            _factories["rev"] =
                new SimpleMethodCallFactory(
                    () => typeof(MethodLibrary).GetMethods().First(m => m.Name == "Reverse"),
                    (s, p, d, m) => CreateStaticMethodCall(d, m));
            _factories["cut"] =
                new SimpleMethodCallFactory(
                    () => typeof(MethodLibrary).GetMethods().First(m => m.Name == "Cut"),
                    (s, p, d, m) => CreateStaticMethodCall(d, m));
            _factories["max"] =
                new ResolvingMethodCallFactory(
                    () => typeof(Math).GetMethods().Where(m => m.Name == "Max"),
                    (s, p, d, methods) => CreateMathMethodCall(d, methods));
            _factories["min"] =
                new ResolvingMethodCallFactory(
                    () => typeof(Math).GetMethods().Where(m => m.Name == "Min"),
                    (s, p, d, methods) => CreateMathMethodCall(d, methods));
            _factories["now"] =
                new SimpleMethodCallFactory(
                    () => typeof(DateTime).GetMethods().First(m => m.Name == "get_Now"),
                    (s, p, d, m) => CreateStaticMethodCall(d, m));
            _factories["eval"] =
                new SimpleMethodCallFactory(
                    () => typeof(MkayAttribute).GetMethods().First(m => m.Name == "Evalidate"),
                    (s, p, d, m) => CreateEvalMethodCall(s, p, d, m));
            _factories["~"] = 
                new SimpleMethodCallFactory(
                    () => typeof(MethodLibrary).GetMethods().First(m => m.Name == "IsMatch"),
                    (s, p, d, m) => CreateStaticMethodCall(d, m));
        }

        private MethodCallExpression CreateMathMethodCall(OpData<Expression> d, IEnumerable<MethodInfo> methods)
        {
                var opers = d.Operands.ToList();
                if (opers.Count() != 2)
                {
                    throw new ExpressionBuilderException("Wrong number of arguments to max function!");
                }
                var left = opers[0];
                var right = opers[1];

                if (left.Type != right.Type)
                {
                    var t = _unifier.Unify(left.Type, right.Type);
                    if (t != null)
                    {
                        if (t != left.Type)
                        {
                            left = Expression.Convert(left, t);
                        }

                        if (t != right.Type)
                        {
                            right = Expression.Convert(right, t);
                        }
                    }
                }

                var method = methods.First(m => m.GetParameters().First().ParameterType == left.Type);
                var args = new[] { left, right };
                var exp = Expression.Call(method, args);
                return exp;
        }

        public Expression Build(OpData<Expression> data)
        {
            var factory = _factories[data.Operator];
            var exp = factory.CreateCall(_subject, _propertyName, data);
            return exp;
        }

        private MethodCallExpression CreateEvalMethodCall(object subject, string propertyName, OpData<Expression> data, MethodInfo method)
        {
            var self = Expression.Constant(subject);
            var prop = Expression.Constant(propertyName);
            var seen = Expression.Constant(_seen);
            var args = new [] { self, prop, data.Operands.First(), seen };
            var exp = Expression.Call(method, args);
            return exp;
        }

        private static MethodCallExpression CreateStaticMethodCall(OpData<Expression> data, MethodInfo method)
        {
            var exp = Expression.Call(method, data.Operands.ToArray());
            return exp;
        }
    }
}