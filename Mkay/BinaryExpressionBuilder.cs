using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mkay
{
    public class BinaryExpressionBuilder
    {
        private readonly Unifier _unifier;

        public BinaryExpressionBuilder(Unifier unifier)
        {
            _unifier = unifier;
        }

        public Expression Build(OpData<Expression> data)
        {
            var exp = GetExpression(data);
            return exp;
        }

        private Expression GetExpression(OpData<Expression> data)
        {
            switch (data.Operator)
            {
                case "+":
                    return CreateRightSkewedTreeExpression(data, Unified(Expression.Add));
                case "*":
                    return CreateRightSkewedTreeExpression(data, Unified(Expression.Multiply));
                case "==":
                    return CreateComparisonExpression(data, Expression.Equal);
                case "!=":
                    return CreateComparisonExpression(data, Expression.NotEqual);
                case ">":
                    return CreateComparisonExpression(data, Expression.GreaterThan);
                case ">=":
                    return CreateComparisonExpression(data, Expression.GreaterThanOrEqual);
                case "<":
                    return CreateComparisonExpression(data, Expression.LessThan);
                case "<=":
                    return CreateComparisonExpression(data, Expression.LessThanOrEqual);
                case "and":
                case "&&":
                case "all":
                    return CreateRightSkewedTreeExpression(data, Expression.AndAlso);
                case "or":
                case "||":
                case "any":
                    return CreateRightSkewedTreeExpression(data, Expression.OrElse);
            }

            throw new ExpressionBuilderException("Unknown operator " + data.Operator);
        }

        private Func<Expression, Expression, BinaryExpression> Unified(Func<Expression, Expression, BinaryExpression> func)
        {
            return (left, right) =>
                {
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

                    return func(left, right);
                };
        }

        private Expression ParseToDateTime(Expression strExp)
        {
            var method = typeof(DateTime).GetMethods().First(m => m.Name == "Parse");
            return Expression.Call(method, strExp);
        }

        private Expression ParseToInt(Expression strExp)
        {
            var method = typeof(int).GetMethods().First(m => m.Name == "Parse" && m.GetParameters().Count() == 1);
            return Expression.Call(method, strExp);
        }

        private Expression CreateComparisonExpression(OpData<Expression> data, Func<Expression, Expression, BinaryExpression> f)
        {
            var func = Unified(f);

            var q = new Queue<Expression>(data.Operands);

            var exps = new Queue<Expression>();

            var first = q.Dequeue();

            while (q.Count > 0)
            {
                var next = q.Dequeue();
                if (first.Type != next.Type)
                {
                    // TODO: Clean up this mess.
                    if (first.Type == typeof(int) && next.Type == typeof(string))
                    {
                        next = ParseToInt(next);
                    }
                    else if (next.Type == typeof(int) && first.Type == typeof(string))
                    {
                        first = ParseToInt(first);
                    }

                    if (first.Type == typeof(DateTime) && next.Type == typeof(string))
                    {
                        next = ParseToDateTime(next);
                    }
                    else if (next.Type == typeof(DateTime) && first.Type == typeof(string))
                    {
                        first = ParseToDateTime(first);
                    }

                    var t = _unifier.Unify(first.Type, next.Type);
                    if (t != null)
                    {
                        if (t != first.Type)
                        {
                            first = Expression.Convert(first, t);
                        }

                        if (t != next.Type)
                        {
                            next = Expression.Convert(next, t);
                        }                        
                    }
                }

                var exp = func(first, next);
                exps.Enqueue(exp);
            }

            var result = exps.Dequeue();

            while (exps.Count > 0)
            {
                var next = exps.Dequeue();
                result = Expression.And(result, next);
            }

            return result;
        }

        private Expression CreateRightSkewedTreeExpression(OpData<Expression> data, Func<Expression, Expression, BinaryExpression> func)
        {
            var stack = new Stack<Expression>();
            foreach (var operand in data.Operands)
            {
                stack.Push(operand);
            }

            var current = stack.Pop();

            while (stack.Count > 0)
            {
                var next = stack.Pop();
                current = func(next, current);
            }

            return current;
        }
    }
}