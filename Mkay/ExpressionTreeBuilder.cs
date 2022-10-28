using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mkay
{
    public class ExpressionTreeBuilder : ITreeBuilder<Expression>
    {
        private readonly object _subject;
        private readonly string _propertyName;
        private readonly string[] _methodCallExpressionSymbols;
        private readonly Unifier _unifier = new Unifier();
        private readonly HashSet<string> _seen;

        public ExpressionTreeBuilder(object subject, string propertyName, HashSet<string> seen)
        {
            _subject = subject;
            _propertyName = propertyName;
            _seen = seen;
            _methodCallExpressionSymbols = new []
                {
                    "len",
                    "now",
                    "max",
                    "min",
                    "time",
                    "eval",
                    "~"
                };
        }

        public Expression Build<T>(Atom<T> atom)
        {
            var val = atom.Value;
            return GetConstantExpression(val) ?? GetPropertyAccessExpression(val as string);
        }
 
        public Expression<Func<bool>> DeriveFunc(Expression body) 
        {
            var result = Expression.Lambda<Func<bool>>(body, new ParameterExpression[0]);
            return result;
        }

        private Expression GetPropertyAccessExpression(string val)
        {
            string propName = val == "." ? _propertyName : val;

            var it = Expression.Constant(_subject);
            
            return Expression.PropertyOrField(it, propName);
        }

        private Expression GetConstantExpression<T>(T val)
        {
            if (val is int)
            {
                var ival = (int)(object)val;
                return Expression.Constant(ival);
            }

            if (val is double)
            {
                var dval = (double)(object)val;
                return Expression.Constant(dval);
            }

            if (val is string)
            {
                var sval = val as string;
                
                if (sval == "true")
                {
                    return Expression.Constant(true);
                }

                if (sval == "false")
                {
                    return Expression.Constant(false);
                }

                if (sval.Length >= 2 && sval.StartsWith("\"") && sval.EndsWith("\""))
                {
                    var s = sval.Substring(1, sval.Length - 2);
                    return Expression.Constant(s);
                }
            }

            return null;
        }

        public Expression Build(OpData<Expression> data)
        {
            if (IsMethod(data.Operator))
            {
                var it = new MethodCallExpressionBuilder(_subject, _propertyName, _unifier, _seen).Build(data);
                return it;
            }

            return new BinaryExpressionBuilder(_unifier).Build(data);
        }

        private bool IsMethod(string op)
        {
            return _methodCallExpressionSymbols.Contains(op);
        }
    }
}