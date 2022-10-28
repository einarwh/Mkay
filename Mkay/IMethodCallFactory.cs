using System.Linq.Expressions;

namespace Mkay
{
    public interface IMethodCallFactory
    {
        MethodCallExpression CreateCall(object subject, string property, OpData<Expression> data);
    }
}
