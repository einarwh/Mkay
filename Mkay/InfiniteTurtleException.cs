using System;

namespace Mkay
{
    public class InfiniteTurtleException : MkayException
    {
        private readonly string _property;

        private readonly string _rule;

        public InfiniteTurtleException(string property, string rule)
            : base("Don't disturb the turtles, mkay?")
        {
            _property = property;
            _rule = rule;
        }

        public string Rule
        {
            get
            {
                return _rule;
            }
        }

        public string Property
        {
            get
            {
                return _property;
            }
        }
    }
}