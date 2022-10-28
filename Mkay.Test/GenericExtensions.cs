using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkay.Test
{
    public static class GenericExtensions
    {
        public static Tuple<T, string> Property<T>(this T subject, Expression<Func<T, object>> exp)
        {
            var body = exp.Body;
            if (body is MemberExpression)
            {
                return PickProperty(subject, (MemberExpression)body);
            }

            var unary = (UnaryExpression)body;
            return PickProperty(subject, (MemberExpression)unary.Operand);
        }

        private static Tuple<T, string> PickProperty<T>(T subject, MemberExpression exp)
        {
            var property = exp.Member.Name;
            return new Tuple<T, string>(subject, property);
        }

        public static void Obeys<T>(this Tuple<T, string> tup, string rule)
        {
            var result = Check(tup.Item1, tup.Item2, rule);
            Assert.IsTrue(result);
        }

        public static void Violates<T>(this Tuple<T, string> tup, string rule)
        {
            var result = Check(tup.Item1, tup.Item2, rule);
            Assert.IsFalse(result);
        }

        private static bool Check(object subject, string property, string rule)
        {
            var cell = new ExpParser(rule).Parse();

            var builder = new ExpressionTreeBuilder(subject, property, new HashSet<string>());
            var viz = new ExpVisitor<Expression>(builder);

            cell.Accept(viz);

            var exp = viz.GetResult();
            var func = builder.DeriveFunc(exp).Compile();
            var result = func();

            return result;
        }
    }
}