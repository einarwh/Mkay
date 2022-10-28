using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Mkay
{
    public class SimpleMethodCallFactory : IMethodCallFactory
    {
        private readonly Func<MethodInfo> _lookup;

        private readonly Func<object, string, OpData<Expression>, MethodInfo, MethodCallExpression> _callFactory;

        public SimpleMethodCallFactory(
            Func<MethodInfo> lookup, 
            Func<object, string, OpData<Expression>, MethodInfo, MethodCallExpression> callFactory)
        {
            _lookup = lookup;
            _callFactory = callFactory;
        }

        public MethodCallExpression CreateCall(object subject, string property, OpData<Expression> data)
        {
            var method = _lookup();
            Debug.Print("Create call to " + method.Name);
            var call = _callFactory(subject, property, data, method);
            return call;
        }
    }
}