using System;

namespace Mkay
{
    public class MkayException : Exception
    {
        public MkayException(string message) : base(message)
        {
        }
    }
}