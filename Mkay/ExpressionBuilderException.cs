using System;

namespace Mkay
{
    internal class ExpressionBuilderException : MkayException
    {
        public ExpressionBuilderException(string message): base(message)
        {
        }
    }
}