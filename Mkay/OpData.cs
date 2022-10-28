using System.Collections.Generic;

namespace Mkay
{
    public class OpData<T>
    {
        private readonly string _op;
        private readonly List<T> _operands = new List<T>();

        public OpData(string op)
        {
            _op = op;
        }

        public string Operator
        {
            get { return _op; }
        }

        public IEnumerable<T> Operands
        {
            get { return _operands; }
        }

        public void AddOperand(T o)
        {
            _operands.Add(o);
        }
    }
}