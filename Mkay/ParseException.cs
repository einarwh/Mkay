using System;

namespace Mkay
{
    public class ParseException : Exception
    {
        public ParseException(string rule, string message = null) 
            : base("The string '{0}' is not a well-formed Mkay rule.{1}".With(rule, message == null ? string.Empty : " " + message))
        {            
        }
    }
}
