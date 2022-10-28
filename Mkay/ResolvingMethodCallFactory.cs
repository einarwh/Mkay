using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Mkay
{
    public class ResolvingMethodCallFactory : IMethodCallFactory
    {
        private readonly Func<object, string, OpData<Expression>, IEnumerable<MethodInfo>, MethodCallExpression> _factory;

        private readonly Func<IEnumerable<MethodInfo>> _lookup;

        public ResolvingMethodCallFactory(
            Func<IEnumerable<MethodInfo>> lookup, 
            Func<object, string, OpData<Expression>, IEnumerable<MethodInfo>, MethodCallExpression> factory 
            )
        {
            _lookup = lookup;
            _factory = factory;
        }

        public MethodCallExpression CreateCall(object subject, string property, OpData<Expression> data)
        {
            var overloads = _lookup();
            var call = _factory(subject, property, data, overloads);
            return call;
        }
    }
}